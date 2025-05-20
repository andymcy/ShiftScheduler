
using Microsoft.EntityFrameworkCore;
using ShiftScheduler.Data;
using ShiftScheduler.Services;
using ShiftScheduler.Models;          // ← add this


var builder = WebApplication.CreateBuilder(args);

// ──────────────── Services ────────────────

// MVC (controllers + Razor views)
builder.Services.AddControllersWithViews();

// DbContext  (Pomelo + MySQL)
builder.Services.AddDbContext<ShiftSchedulerContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ShiftDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ShiftDb"))));

// Genetic-algorithm scheduler
builder.Services.AddScoped<ScheduleSolver>();

var app = builder.Build();

// ──────────────── Middleware pipeline ────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();      // wwwroot, css, js, etc.
app.UseRouting();
app.UseAuthorization();

// ──────────────── Minimal-API endpoint ────────────────
app.MapGet("/solve", async (ShiftSchedulerContext db, ScheduleSolver solver) =>
{
    var monday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

    // try to load an existing week row; otherwise create one
    var week = await db.WeeklySchedules
                       .FirstOrDefaultAsync(w => w.WeekStartDate == monday)
               ?? new WeeklySchedule { WeekStartDate = monday };

    if (week.ScheduleId == 0) db.WeeklySchedules.Add(week);

    // load data for the GA
    var employees = await db.Employees
                            .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                            .ToListAsync();
    var shifts    = await db.Shifts
                            .Include(s => s.RequiredSkills).ThenInclude(rs => rs.Skill)
                            .ToListAsync();

    // run GA
    var assignments = solver.GenerateSchedule(employees, shifts);

    // attach the correct ScheduleId
    foreach (var a in assignments)
    {
        a.ScheduleId = week.ScheduleId;
        a.Date       = monday;          // or any logic per shift
    }

    db.ShiftAssignments.AddRange(assignments);
    await db.SaveChangesAsync();
    return Results.Ok(assignments);
});


// ──────────────── MVC routing ────────────────
// Default route: root URL goes to Schedule/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Schedule}/{action=Index}/{id?}");

app.Run();
