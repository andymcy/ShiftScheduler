using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftScheduler.Data;
using ShiftScheduler.Models;

namespace ShiftScheduler.Controllers;

[Route("[controller]")]
public class ScheduleController : Controller
{
    private readonly ShiftSchedulerContext _db;
    public ScheduleController(ShiftSchedulerContext db) => _db = db;

    // GET /Schedule
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var monday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek +
                                            (int)DayOfWeek.Monday);

        var shifts = await _db.Shifts
            .Include(s => s.RequiredSkills).ThenInclude(rs => rs.Skill)
            .Include(s => s.Assignments).ThenInclude(a => a.MainEmployee)
            .Include(s => s.Assignments).ThenInclude(a => a.BackupEmployee)
            .Where(s => s.Assignments.Any(a =>
                       a.Date >= monday && a.Date <  monday.AddDays(7)))
            .ToListAsync();

        // ShiftId, DayOfWeek, ShiftTime, Skills, MainName, BackupName
        var rows = shifts.Select(s =>
        {
            var a = s.Assignments.First();
            return new ShiftRow(
                s.ShiftId,
                s.DayOfWeek,
                s.ShiftTime,
                string.Join(", ", s.RequiredSkills.Select(rs => rs.Skill.Name)),
                a.MainEmployee?.Name ?? "",
                a.BackupEmployee?.Name ?? "");
        })
        .OrderBy(r => r.DayOfWeek)
        .ThenBy(r => r.ShiftTime)
        .ToList();

        var vm = new ScheduleViewModel
        {
            WeekStartDate = monday,
            Shifts        = rows
        };
        return View(vm);
    }

    // GET /Schedule/Edit/id
    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var shift = await _db.Shifts
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.ShiftId == id);

        if (shift is null) return NotFound();

        var employees  = await _db.Employees.OrderBy(e => e.Name).ToListAsync();
        var assignment = shift.Assignments.FirstOrDefault();

        var vm = new EditShiftViewModel
        {
            ShiftId          = shift.ShiftId,
            DayOfWeek        = shift.DayOfWeek,
            ShiftTime        = shift.ShiftTime,
            MainEmployeeId   = assignment?.MainEmployeeId,
            BackupEmployeeId = assignment?.BackupEmployeeId,
            EmployeeList     = employees.Select(e =>
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(
                    e.Name, e.EmployeeId.ToString())).ToList()
        };
        return View(vm);
    }

    // POST /Schedule/Edit
    [HttpPost("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditShiftViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var assignment = await _db.ShiftAssignments
            .FirstOrDefaultAsync(a => a.ShiftId == vm.ShiftId);
        if (assignment is null) return NotFound();

        if (vm.MainEmployeeId.HasValue)
            assignment.MainEmployeeId = vm.MainEmployeeId.Value;

        assignment.BackupEmployeeId = vm.BackupEmployeeId;   // may be null
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
