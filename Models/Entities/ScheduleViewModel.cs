using System.Collections.Generic;
using ShiftScheduler.Models.Entities;   //  <-- add this line!

namespace ShiftScheduler.Models
{
    public class ScheduleViewModel
    {
        public List<Shift>    Shifts    { get; set; } = new();
        public List<Employee> Employees { get; set; } = new();
    }
}
