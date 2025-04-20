using System.Collections.Generic;
using System.Linq;

namespace ShiftScheduler.Models
{
    public class ScheduleCandidate
    {
        public List<ShiftAssignment> Assignments { get; set; } = new List<ShiftAssignment>();
        public int Fitness { get; set; }

        public void CalculateFitness()
        {
            int score = 0;

            foreach (var assign in Assignments)
            {
                var shift = assign.Shift;
                var employees = assign.AssignedEmployees;

                if (employees.Count == shift.RequiredEmployees)
                    score += 1;
                else if (employees.Count < shift.RequiredEmployees)
                    score -= (shift.RequiredEmployees - employees.Count);

                foreach (var emp in employees)
                {
                    score += emp.IsAvailableForShift(shift) ? 1 : -1;
                    score += emp.HasSkillsForShift(shift) ? 1 : -1;
                }

                foreach (var skill in shift.RequiredSkills)
                {
                    bool covered = employees.Any(e => e.Skills.Contains(skill));
                    score += covered ? 1 : -2;
                }
            }

            Fitness = score;
        }
    }
}
