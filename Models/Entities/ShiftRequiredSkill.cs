using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftScheduler.Models.Entities
{
    public class ShiftRequiredSkill
    {
        public int ShiftId { get; set; }
        public int SkillId { get; set; }

        // Navigation properties
        public Shift Shift { get; set; }
        public Skill Skill { get; set; }
    }
}
