using System.Text.Json.Serialization;

namespace ShiftScheduler.Models
{
    public class ShiftRequiredSkill
    {
        // composite key (ShiftId + SkillId) configured in DbContext
        public int ShiftId { get; set; }

        [JsonIgnore]                 // ← prevents Shift⇄ShiftRequiredSkill cycle
        public Shift Shift { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}
