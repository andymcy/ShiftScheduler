using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShiftScheduler.Models
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; } = string.Empty;

        // we don’t need to serialize these back‐links
        [JsonIgnore]
        public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();

        [JsonIgnore]
        public ICollection<ShiftRequiredSkill> ShiftRequiredSkills { get; set; } = new List<ShiftRequiredSkill>();
    }
}
