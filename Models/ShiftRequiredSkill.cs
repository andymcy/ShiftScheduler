using System.Text.Json.Serialization;

namespace ShiftScheduler.Models
{
    public class ShiftRequiredSkill
    {
        //   ShiftId + SkillId configured in DbContext
        public int ShiftId { get; set; }

        [JsonIgnore]                
        public Shift Shift { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}
