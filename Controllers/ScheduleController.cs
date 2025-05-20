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