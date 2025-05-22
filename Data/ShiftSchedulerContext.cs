using Microsoft.EntityFrameworkCore;
using ShiftScheduler.Models;

namespace ShiftScheduler.Data
{
    public class ShiftSchedulerContext : DbContext
    {
        public ShiftSchedulerContext(DbContextOptions<ShiftSchedulerContext> options)
            : base(options) { }

        // ────────── DbSets ──────────
        public DbSet<Employee>           Employees           => Set<Employee>();
        public DbSet<Skill>              Skills              => Set<Skill>();
        public DbSet<EmployeeSkill>      EmployeeSkills      => Set<EmployeeSkill>();
        public DbSet<Shift>              Shifts              => Set<Shift>();
        public DbSet<ShiftRequiredSkill> ShiftRequiredSkills => Set<ShiftRequiredSkill>();
        public DbSet<ShiftAssignment>    ShiftAssignments    => Set<ShiftAssignment>();
        public DbSet<WeeklySchedule>     WeeklySchedules     => Set<WeeklySchedule>();

        protected override void OnModelCreating(ModelBuilder m)
        {
            //  PRIMARY -, COMPOSITE KEYS 
            m.Entity<EmployeeSkill>()
             .HasKey(es => new { es.EmployeeId, es.SkillId });

            m.Entity<ShiftRequiredSkill>()
             .HasKey(rs => new { rs.ShiftId, rs.SkillId });

            m.Entity<ShiftAssignment>()
             .HasKey(sa => sa.AssignmentId);      //  PK

            m.Entity<WeeklySchedule>()
             .HasKey(ws => ws.ScheduleId);        //  PK

            //  EMPLOYEE - EMPLOYEESKILL - SKILL 
            m.Entity<Employee>()
             .HasMany(e => e.EmployeeSkills)
             .WithOne(es => es.Employee)
             .HasForeignKey(es => es.EmployeeId);

            m.Entity<Skill>()
             .HasMany(s => s.EmployeeSkills)
             .WithOne(es => es.Skill)
             .HasForeignKey(es => es.SkillId);

            //  SHIFT - SHIFTREQUIREDSKILL - SKILL
            m.Entity<Shift>()
             .HasMany(s => s.RequiredSkills)
             .WithOne(rs => rs.Shift)
             .HasForeignKey(rs => rs.ShiftId);

            m.Entity<Skill>()
             .HasMany(s => s.ShiftRequiredSkills)
             .WithOne(rs => rs.Skill)
             .HasForeignKey(rs => rs.SkillId);

            //  SHIFT  SHIFTASSIGNMENT 
            m.Entity<Shift>()
             .HasMany(s => s.Assignments)
             .WithOne(a => a.Shift)
             .HasForeignKey(a => a.ShiftId);

            //  EMPLOYEE (main / backup) - SHIFTASSIGNMENT 
            m.Entity<ShiftAssignment>()
             .HasOne(sa => sa.MainEmployee)
             .WithMany(e => e.MainAssignments)
             .HasForeignKey(sa => sa.MainEmployeeId)
             .OnDelete(DeleteBehavior.Restrict);   

            m.Entity<ShiftAssignment>()
             .HasOne(sa => sa.BackupEmployee)
             .WithMany(e => e.BackupAssignments)
             .HasForeignKey(sa => sa.BackupEmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            //  WEEKLYSCHEDULE - SHIFTASSIGNMENT (NEW) 
            m.Entity<ShiftAssignment>()
            .HasOne(sa => sa.WeeklySchedule)
            .WithMany(ws => ws.Assignments)
            .HasForeignKey(sa => sa.ScheduleId)   // nullable FK
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
