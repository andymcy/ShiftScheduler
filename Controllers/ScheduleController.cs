using Microsoft.AspNetCore.Mvc;
using ShiftScheduler.Models;
using System;
using System.Collections.Generic;

namespace ShiftScheduler.Controllers
{
    public class ScheduleController : Controller
    {
        public IActionResult Index()
        {
            // === 1. Dummy Employees ===
            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "Alice",
                    Type = EmploymentType.Permanent,
                    Skills = new List<string> { "Reception", "Sales" },
                    AvailableDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday }
                },
                new Employee
                {
                    Id = 2,
                    Name = "Bob",
                    Type = EmploymentType.Temporary,
                    Skills = new List<string> { "Cleaning", "Spa" },
                    AvailableDays = new List<DayOfWeek> { DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday }
                },
                new Employee
                {
                    Id = 3,
                    Name = "Charlie",
                    Type = EmploymentType.Permanent,
                    Skills = new List<string> { "Trainer" },
                    AvailableDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Thursday }
                },
                new Employee
                {
                    Id = 4,
                    Name = "Diana",
                    Type = EmploymentType.Temporary,
                    Skills = new List<string> { "Sales", "Trainer" },
                    AvailableDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Friday }
                }
            };

            // === 2. Dummy Shifts ===
            var shifts = new List<Shift>
            {
                new Shift
                {
                    Id = 101,
                    Date = new DateTime(2025, 4, 21), // Monday
                    StartTime = TimeSpan.FromHours(8),
                    EndTime = TimeSpan.FromHours(12),
                    RequiredEmployees = 2,
                    RequiredSkills = new List<string> { "Reception" }
                },
                new Shift
                {
                    Id = 102,
                    Date = new DateTime(2025, 4, 22), // Tuesday
                    StartTime = TimeSpan.FromHours(12),
                    EndTime = TimeSpan.FromHours(16),
                    RequiredEmployees = 2,
                    RequiredSkills = new List<string> { "Trainer", "Sales" }
                },
                new Shift
                {
                    Id = 103,
                    Date = new DateTime(2025, 4, 23), // Wednesday
                    StartTime = TimeSpan.FromHours(9),
                    EndTime = TimeSpan.FromHours(13),
                    RequiredEmployees = 2,
                    RequiredSkills = new List<string> { "Cleaning", "Spa" }
                }
            };

            // === 3. Run Genetic Algorithm ===
            var bestSchedule = GeneticScheduler.GenerateSchedule(employees, shifts);

            // === 4. Send to View ===
            return View(bestSchedule);
        }
    }
}
