using Microsoft.EntityFrameworkCore;
using ShiftScheduler.Data;
using ShiftScheduler.Services;
using ShiftScheduler.Models;
using System.Text.Json.Serialization;                // ➊ NEW

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────── services ───────────────────────

// MVC (+ Razor) ➋ ADD IgnoreCycles so JSON never crashes
builder.Services
       .AddControllersWithViews()
       .AddJsonOptions(o =>
           o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<ShiftSchedulerContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ShiftDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ShiftDb"))));

builder.Services.AddScoped<ScheduleSolver>();

var app = builder.Build();

// ─────────────────────── pipeline ───────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ─────────────────────── GA demo endpoint ───────────────
app.MapGet("/solve", async (ShiftSchedulerContext db, ScheduleSolver solver) =>
{
    var monday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

    // ensure week row exists
    var week = await db.WeeklySchedules
                       .FirstOrDefaultAsync(w => w.WeekStartDate == monday)
               ?? new WeeklySchedule { WeekStartDate = monday };
    if (week.ScheduleId == 0) db.WeeklySchedules.Add(week);

    var employees = await db.Employees
                            .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                            .ToListAsync();
    var shifts    = await db.Shifts
                            .Include(s => s.RequiredSkills).ThenInclude(rs => rs.Skill)
                            .ToListAsync();

    var assignments = solver.GenerateSchedule(employees, shifts);

    foreach (var a in assignments)
    {
        a.ScheduleId = week.ScheduleId;
        a.Date       = monday;     // simple demo: all on Monday
    }

    db.ShiftAssignments.AddRange(assignments);
    await db.SaveChangesAsync();   // ← now succeeds, no JSON loop
    return Results.Ok(assignments);
});

// MVC default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Schedule}/{action=Index}/{id?}");

app.Run();
