using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShiftScheduler.Models
{
    // one row in the schedule table
    public record ShiftRow(
        int    ShiftId,
        string DayOfWeek,
        string ShiftTime,
        string RequiredSkills,
        string MainEmployee,
        string BackupEmployee);

    // page-level model for /Schedule/Index
    public class ScheduleViewModel
    {
        public DateTime        WeekStartDate { get; init; }
        public List<ShiftRow>  Shifts        { get; init; } = new();
    }

    //  used in the Edit page
    public class EditShiftViewModel
    {
        public int    ShiftId          { get; set; }
        public string DayOfWeek        { get; set; } = "";
        public string ShiftTime        { get; set; } = "";

        public int?    MainEmployeeId   { get; set; }
        public int?    BackupEmployeeId { get; set; }

        // rendered as <select> options
        public List<SelectListItem> EmployeeList { get; set; } = new();
    }
}
