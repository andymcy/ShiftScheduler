using System.ComponentModel.DataAnnotations.Schema;
using ShiftScheduler.Models.Entities;

namespace ShiftScheduler.Models.Entities
{
    public partial class Shift
    {
        /* ----  simple columns ---- */
        [NotMapped] public int    Id  => ShiftId;            // old name
        [NotMapped] public string Day => DayOfWeek;          // “Sunday”, “Monday”, …

        /* ----  navigation aliases ---- */

        // The list of required skills for this shift
        [NotMapped]
        public IEnumerable<Skill> RequiredSkills =>
            ShiftRequiredSkills.Select(srs => srs.Skill);

        // convenience: the MAIN employee (if any)        
        [NotMapped]
        public Employee? AssignedEmployee =>
            ShiftAssignments.FirstOrDefault(sa => sa.Role == "Main")?.MainEmployee;

        // convenience: the BACKUP employee (if any)
        [NotMapped]
        public Employee? BackupEmployee =>
            ShiftAssignments.FirstOrDefault(sa => sa.Role == "Backup")?.BackupEmployee;
    }
}
