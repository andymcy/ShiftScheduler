using System.ComponentModel.DataAnnotations.Schema;
using ShiftScheduler.Models.Entities;

namespace ShiftScheduler.Models.Entities
{
    public partial class ShiftAssignment
    {
        // Just a readable alias for easier LINQ
        //[NotMapped] public Employee MainEmployee   => EmployeeMain;
        //[NotMapped] public Employee BackupEmployee => EmployeeBackup;
    }
}
