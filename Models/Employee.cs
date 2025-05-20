using System.Collections.Generic;

namespace ShiftScheduler.Models
{
    public class Employee
    {
        public int    EmployeeId     { get; set; }
        public string Name           { get; set; } = "";
        public int    EmploymentType { get; set; }    // 0=part-time,1=full-time…
        public bool   PrefersMorning { get; set; }
        public bool   PrefersEvening { get; set; }
        public bool   AvoidsWeekends { get; set; }

        public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
        public ICollection<ShiftAssignment> MainAssignments    { get; set; } = new List<ShiftAssignment>();
        public ICollection<ShiftAssignment> BackupAssignments  { get; set; } = new List<ShiftAssignment>();
    }
}
