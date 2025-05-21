using System.Text.Json.Serialization;

namespace ShiftScheduler.Models
{
    public class EmployeeSkill
    {
        // composite key (EmployeeId + SkillId) configured in DbContext
        public int EmployeeId { get; set; }

        [JsonIgnore]                 // ← prevents Employee⇄EmployeeSkill cycle
        public Employee Employee { get; set; } = null!;

        public int SkillId { get; set; }

       [JsonIgnore]
        public Skill Skill { get; set; } = null!;
    }
}
