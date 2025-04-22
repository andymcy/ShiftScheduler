using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShiftScheduler.Models;

namespace ShiftScheduler.Controllers
{
    public class ScheduleController : Controller
    {
        private static readonly List<Employee> _employees = new List<Employee>();
        private static readonly List<Shift>    _shifts    = new List<Shift>();
        private static bool                    _generated = false;

        static ScheduleController()
        {
            // ─── Employees ───────────────────────────────────────────
            var alice = new Employee
            {
                Id             = 1,
                Name           = "Alice",
                EmploymentType = EmploymentType.Permanent,
                PrefersMorning = true,
                PrefersEvening = false,
                AvoidsWeekends = true,
                Skills         = new List<Skill> { Skill.Manager }
            };
            alice.Availability.AddRange(new[]
            {
                new AvailabilitySlot(DayOfWeek.Monday,    ShiftTime.Morning),
                new AvailabilitySlot(DayOfWeek.Tuesday,   ShiftTime.Morning),
                new AvailabilitySlot(DayOfWeek.Wednesday, ShiftTime.Morning),
                new AvailabilitySlot(DayOfWeek.Thursday,  ShiftTime.Morning),
            });
            _employees.Add(alice);

            var bob = new Employee
            {
                Id             = 2,
                Name           = "Bob",
                EmploymentType = EmploymentType.Permanent,
                PrefersMorning = false,
                PrefersEvening = true,
                AvoidsWeekends = true,
                Skills         = new List<Skill> { Skill.Sales }
            };
            bob.Availability.AddRange(new[]
            {
                new AvailabilitySlot(DayOfWeek.Monday,    ShiftTime.Evening),
                new AvailabilitySlot(DayOfWeek.Wednesday, ShiftTime.Evening),
            });
            _employees.Add(bob);

            var charlie = new Employee
            {
                Id             = 3,
                Name           = "Charlie",
                EmploymentType = EmploymentType.Temporary,
                PrefersMorning = false,
                PrefersEvening = false,
                AvoidsWeekends = false,
                Skills         = new List<Skill> { Skill.Sales, Skill.Training }
            };
            foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
            {
                charlie.Availability.Add(new AvailabilitySlot(d, ShiftTime.Morning));
                charlie.Availability.Add(new AvailabilitySlot(d, ShiftTime.Evening));
            }
            _employees.Add(charlie);

            var dina = new Employee
            {
                Id             = 4,
                Name           = "Dina",
                EmploymentType = EmploymentType.Permanent,
                PrefersMorning = true,
                PrefersEvening = true,
                AvoidsWeekends = false,
                Skills         = new List<Skill> { Skill.Spa }
            };
            foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
            {
                dina.Availability.Add(new AvailabilitySlot(d, ShiftTime.Morning));
                dina.Availability.Add(new AvailabilitySlot(d, ShiftTime.Evening));
            }
            _employees.Add(dina);

            // ─── Shifts ───────────────────────────────────────────────
            _shifts.Add(new Shift
            {
                Id                = 101,
                Name              = "Monday Morning",
                Day               = DayOfWeek.Monday,
                ShiftTime         = ShiftTime.Morning,
                RequiredEmployees = 2,
                RequiredSkills    = new List<Skill> { Skill.Manager, Skill.Sales }
            });
            _shifts.Add(new Shift
            {
                Id                = 102,
                Name              = "Monday Evening",
                Day               = DayOfWeek.Monday,
                ShiftTime         = ShiftTime.Evening,
                RequiredEmployees = 1,
                RequiredSkills    = new List<Skill> { Skill.Sales }
            });
            _shifts.Add(new Shift
            {
                Id                = 103,
                Name              = "Saturday Morning",
                Day               = DayOfWeek.Saturday,
                ShiftTime         = ShiftTime.Morning,
                RequiredEmployees = 1,
                RequiredSkills    = new List<Skill> { Skill.Manager }
            });
            _shifts.Add(new Shift
            {
                Id                = 104,
                Name              = "Saturday Evening Spa",
                Day               = DayOfWeek.Saturday,
                ShiftTime         = ShiftTime.Evening,
                RequiredEmployees = 1,
                RequiredSkills    = new List<Skill> { Skill.Spa }
            });
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!_generated)
            {
                GeneticScheduler.GenerateSchedule(_employees, _shifts);
                _generated = true;
            }

            var vm = new ScheduleViewModel
            {
                Shifts    = _shifts.OrderBy(s => s.Day).ThenBy(s => s.ShiftTime).ToList(),
                Employees = _employees
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult EditSchedule()
        {
            var vm = new ScheduleViewModel
            {
                Shifts    = _shifts.OrderBy(s => s.Day).ThenBy(s => s.ShiftTime).ToList(),
                Employees = _employees
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult SaveSchedule(IFormCollection form)
        {
            foreach (var shift in _shifts)
            {
                var mainKey = $"main_{shift.Id}";
                if (form.TryGetValue(mainKey, out var mainVals)
                    && int.TryParse(mainVals.FirstOrDefault(), out var mainId))
                {
                    shift.AssignedEmployee = _employees.FirstOrDefault(e => e.Id == mainId);
                }

                var backupKey = $"backup_{shift.Id}";
                if (form.TryGetValue(backupKey, out var backVals)
                    && int.TryParse(backVals.FirstOrDefault(), out var backId))
                {
                    shift.BackupEmployee = _employees.FirstOrDefault(e => e.Id == backId);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
