using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftScheduler.Models.Entities
{
    public class EmployeeSkill
    {
        public int EmployeeId { get; set; }
        public int SkillId { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
        public Skill Skill { get; set; }
    }
}
