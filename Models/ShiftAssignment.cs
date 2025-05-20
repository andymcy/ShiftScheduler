using System;

namespace ShiftScheduler.Models
{
    public class ShiftAssignment
    {
        // ---------- Primary key ----------
        public int AssignmentId { get; set; }

        // ---------- FK to Shift ----------
        public int  ShiftId { get; set; }
        public Shift Shift  { get; set; } = null!;

        // ---------- Date of this specific assignment ----------
        public DateTime Date { get; set; }

        // ---------- Main employee ----------
        public int?      MainEmployeeId { get; set; }
        public Employee? MainEmployee   { get; set; } = null!;

        // ---------- Backup employee ----------
        public int?      BackupEmployeeId { get; set; }
        public Employee? BackupEmployee   { get; set; } = null!;

        // ---------- WEEKLY SCHEDULE  (new) ----------
        public int?            ScheduleId     { get; set; }          // FK column
        public WeeklySchedule? WeeklySchedule { get; set; } = null!; // navigation
    }
}
