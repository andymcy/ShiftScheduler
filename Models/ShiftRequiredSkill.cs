namespace ShiftScheduler.Models
{
    public class ShiftRequiredSkill
    {
        public int ShiftId { get; set; }
        public Shift Shift { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}
