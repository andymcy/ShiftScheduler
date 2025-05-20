using Microsoft.EntityFrameworkCore;
using ShiftScheduler.Models.Entities;

namespace ShiftScheduler.Models
{
    public class ShiftSchedulerContext : DbContext
    {
        public ShiftSchedulerContext(DbContextOptions<ShiftSchedulerContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; }
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ShiftAssignment> ShiftAssignments { get; set; }
        public DbSet<ShiftRequiredSkill> ShiftRequiredSkills { get; set; }
        public DbSet<WeeklySchedule> WeeklySchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite primary keys for join tables
            modelBuilder.Entity<EmployeeSkill>()
                .HasKey(es => new { es.EmployeeId, es.SkillId });
            modelBuilder.Entity<ShiftRequiredSkill>()
                .HasKey(sr => new { sr.ShiftId, sr.SkillId });

            // EmployeeSkill relationships
            modelBuilder.Entity<EmployeeSkill>()
                .HasOne(es => es.Employee)
                .WithMany(e => e.EmployeeSkills)
                .HasForeignKey(es => es.EmployeeId);
            modelBuilder.Entity<EmployeeSkill>()
                .HasOne(es => es.Skill)
                .WithMany(s => s.EmployeeSkills)
                .HasForeignKey(es => es.SkillId);

            // ShiftRequiredSkill relationships
            modelBuilder.Entity<ShiftRequiredSkill>()
                .HasOne(sr => sr.Shift)
                .WithMany(s => s.ShiftRequiredSkills)
                .HasForeignKey(sr => sr.ShiftId);
            modelBuilder.Entity<ShiftRequiredSkill>()
                .HasOne(sr => sr.Skill)
                .WithMany(s => s.ShiftRequiredSkills)
                .HasForeignKey(sr => sr.SkillId);

            // ShiftAssignment relationships
            modelBuilder.Entity<ShiftAssignment>()
                .HasOne(sa => sa.WeeklySchedule)
                .WithMany(ws => ws.ShiftAssignments)
                .HasForeignKey(sa => sa.ScheduleId);

            modelBuilder.Entity<ShiftAssignment>()
                .HasOne(sa => sa.Shift)
                .WithMany(sh => sh.ShiftAssignments)
                .HasForeignKey(sa => sa.ShiftId);

            modelBuilder.Entity<ShiftAssignment>()
                .HasOne(sa => sa.MainEmployee)
                .WithMany(e => e.MainShiftAssignments)
                .HasForeignKey(sa => sa.MainEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShiftAssignment>()
                .HasOne(sa => sa.BackupEmployee)
                .WithMany(e => e.BackupShiftAssignments)
                .HasForeignKey(sa => sa.BackupEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
