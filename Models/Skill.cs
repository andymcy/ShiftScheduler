using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShiftScheduler.Models
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
        public ICollection<ShiftRequiredSkill> ShiftRequiredSkills { get; set; } = new List<ShiftRequiredSkill>();
    }
}