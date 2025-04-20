using System.Collections.Generic;

namespace ShiftScheduler.Models
{
    public class ShiftAssignment
    {
        public Shift Shift { get; set; } = null!;
        public List<Employee> AssignedEmployees { get; set; } = new List<Employee>();
    }
}
