using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftScheduler.Models.Entities
{
    public partial class ShiftAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        public int ScheduleId { get; set; }
        public int ShiftId { get; set; }
        public DateTime Date { get; set; }

        public int MainEmployeeId { get; set; }
        public int? BackupEmployeeId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ScheduleId))]
        public WeeklySchedule WeeklySchedule { get; set; }

        [ForeignKey(nameof(ShiftId))]
        public Shift Shift { get; set; }

        [ForeignKey(nameof(MainEmployeeId))]
        public Employee MainEmployee { get; set; }

        [ForeignKey(nameof(BackupEmployeeId))]
        public Employee BackupEmployee { get; set; }
    }
}
