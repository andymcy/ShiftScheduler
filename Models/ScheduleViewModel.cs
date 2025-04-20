using System.Collections.Generic;

namespace ShiftScheduler.Models
{
    public class ScheduleViewModel
    {
        public List<Shift>    Shifts    { get; set; } = new List<Shift>();
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
