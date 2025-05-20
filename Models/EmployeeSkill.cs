namespace ShiftScheduler.Models
{
    public class EmployeeSkill
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}
