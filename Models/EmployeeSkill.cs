using System.Text.Json.Serialization;

namespace ShiftScheduler.Models
{
    public class EmployeeSkill
    {
        public int EmployeeId { get; set; }

        [JsonIgnore]                 
        public Employee Employee { get; set; } = null!;

        public int SkillId { get; set; }

       [JsonIgnore]
        public Skill Skill { get; set; } = null!;
    }
}
