using Microsoft.EntityFrameworkCore;
using ShiftScheduler.Data;
using ShiftScheduler.Services;

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
    // load all required data once, hand to GA
    var employees = await db.Employees
                            .Include(e => e.EmployeeSkills)
                                .ThenInclude(es => es.Skill)
                            .ToListAsync();

    var shifts = await db.Shifts
                         .Include(s => s.RequiredSkills)
                             .ThenInclude(rs => rs.Skill)
                         .ToListAsync();

    var assignments = solver.GenerateSchedule(employees, shifts);

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
