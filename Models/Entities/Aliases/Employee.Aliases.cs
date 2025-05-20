using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ShiftScheduler.Models.Entities;   // <— bring the entities in

namespace ShiftScheduler.Models.Entities.Aliases
{
    public partial class Employee
    {
        [NotMapped]
        public IEnumerable<AvailabilitySlot> Availability =>   // ← match spelling
            AvailabilitySlots;                                 // ← match EF nav name
    }
}
