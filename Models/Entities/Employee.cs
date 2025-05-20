using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftScheduler.Models.Entities
{
    public partial class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public string Name { get; set; }

        public EmploymentType EmploymentType { get; set; }

        public bool PrefersMorning { get; set; }

        public bool PrefersEvening { get; set; }

        public bool AvoidsWeekends { get; set; }

        // Navigation properties
        public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
        public ICollection<ShiftAssignment> MainShiftAssignments { get; set; }
        public ICollection<ShiftAssignment> BackupShiftAssignments { get; set; }

        public Employee()
        {
            EmployeeSkills = new HashSet<EmployeeSkill>();
            MainShiftAssignments = new HashSet<ShiftAssignment>();
            BackupShiftAssignments = new HashSet<ShiftAssignment>();
        }
    }
}
