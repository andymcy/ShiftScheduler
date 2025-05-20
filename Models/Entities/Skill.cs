using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftScheduler.Models.Entities
{
    public class Skill
    {
        [Key]
        public int SkillId { get; set; }

        [Required]
        public string Name { get; set; }

        // Navigation properties
        public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
        public ICollection<ShiftRequiredSkill> ShiftRequiredSkills { get; set; }

        public Skill()
        {
            EmployeeSkills = new HashSet<EmployeeSkill>();
            ShiftRequiredSkills = new HashSet<ShiftRequiredSkill>();
        }
    }
}
