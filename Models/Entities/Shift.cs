using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftScheduler.Models.Entities
{
    public partial class Shift
    {
        [Key]
        public int ShiftId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DayOfWeek { get; set; }

        public ShiftTime ShiftTime { get; set; }

        public int RequiredEmployees { get; set; }

        // Navigation properties
        public ICollection<ShiftAssignment> ShiftAssignments { get; set; }
        public ICollection<ShiftRequiredSkill> ShiftRequiredSkills { get; set; }

        public Shift()
        {
            ShiftAssignments = new HashSet<ShiftAssignment>();
            ShiftRequiredSkills = new HashSet<ShiftRequiredSkill>();
        }
    }
}
