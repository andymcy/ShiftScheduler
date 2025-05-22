using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;         
using ShiftScheduler.Data;
using ShiftScheduler.Services;
using ShiftScheduler.Models;

var builder = WebApplication.CreateBuilder(args);

//  Services 
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(opt =>
    {
        // skip over object cycles instead of blowing up at depth 64
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // don’t suppress any other properties by default
        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

// DbContext  (Pomelo + MySQL)
builder.Services.AddDbContext<ShiftSchedulerContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ShiftDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ShiftDb"))));

// Genetic‐algorithm scheduler
builder.Services.AddScoped<ScheduleSolver>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

//  API: /solve 
app.MapGet("/solve", async (ShiftSchedulerContext db, ScheduleSolver solver) =>
{
    var monday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek +
                                        (int)DayOfWeek.Monday);

    var week = await db.WeeklySchedules
                       .FirstOrDefaultAsync(w => w.WeekStartDate == monday)
               ?? new WeeklySchedule { WeekStartDate = monday };

    if (week.ScheduleId == 0) db.WeeklySchedules.Add(week);

    var employees = await db.Employees
                            .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                            .ToListAsync();

    var shifts = await db.Shifts
                         .Include(s => s.RequiredSkills).ThenInclude(rs => rs.Skill)
                         .ToListAsync();

    var assignments = solver.GenerateSchedule(employees, shifts);

    foreach (var a in assignments)
    {
        a.ScheduleId = week.ScheduleId;
        a.Date       = monday;
    }

    db.ShiftAssignments.AddRange(assignments);
    await db.SaveChangesAsync();

    return Results.Ok(assignments);
});

// default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Schedule}/{action=Index}/{id?}");

app.Run();
