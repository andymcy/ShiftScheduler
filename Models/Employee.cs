using System;
using System.Collections.Generic;

namespace ShiftScheduler.Models
{



    public class Employee
    {
        public int    Id             { get; set; }
        public string Name           { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public bool   PrefersMorning { get; set; }
        public bool   PrefersEvening { get; set; }
        public bool   AvoidsWeekends { get; set; }
        public List<AvailabilitySlot> Availability { get; set; } = new List<AvailabilitySlot>();
        public List<Skill>           Skills       { get; set; } = new List<Skill>();
    }
}
