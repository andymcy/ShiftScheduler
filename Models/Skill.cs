namespace ShiftScheduler.Models
{
    public class Skill
    {
        public int    SkillId { get; set; }
        public string Name    { get; set; } = "";

        public ICollection<EmployeeSkill>     EmployeeSkills     { get; set; } = new List<EmployeeSkill>();
        public ICollection<ShiftRequiredSkill> ShiftRequiredSkills{ get; set; } = new List<ShiftRequiredSkill>();
    }
}
