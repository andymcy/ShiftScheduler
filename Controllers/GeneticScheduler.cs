using System;
using System.Collections.Generic;
using System.Linq;
using ShiftScheduler.Models;

namespace ShiftScheduler.Controllers
{
    public static class GeneticScheduler
    {
        private static readonly Random _random = new Random();

        public static void GenerateSchedule(List<Employee> employees, List<Shift> shifts)
        {
            // nothing to do if we have no employees or no shifts
            if (employees == null || employees.Count == 0 || shifts == null || shifts.Count == 0)
                return;

            int populationSize = 30;
            int generations    = 50;
            int nShifts        = shifts.Count;
            int nEmps          = employees.Count;

            // 1) Initialize population
            var population = new List<int[]>(populationSize);
            for (int i = 0; i < populationSize; i++)
            {
                var chromosome = new int[nShifts];          // ← exactly one slot per shift
                for (int j = 0; j < nShifts; j++)
                    chromosome[j] = _random.Next(nEmps);    // safe: 0 ≤ index < nEmps
                population.Add(chromosome);
            }

            int[] best         = population[0];
            double bestFitness = double.NegativeInfinity;

            // 2) Evolve
            for (int gen = 0; gen < generations; gen++)
            {
                // a) Evaluate all
                var fits = population
                    .Select(ch => Evaluate(ch, employees, shifts))
                    .ToArray();

                // b) Keep track of overall best
                for (int i = 0; i < populationSize; i++)
                {
                    if (fits[i] > bestFitness)
                    {
                        bestFitness = fits[i];
                        best        = (int[])population[i].Clone();
                    }
                }

                // c) Sort by fitness descending
                var ordered = population
                    .Zip(fits, (chrom, fit) => (chrom, fit))
                    .OrderByDescending(cf => cf.fit)
                    .Select(cf => cf.chrom)
                    .ToList();

                // d) Elitism: carry top 10% forward
                int eliteCount = Math.Max(1, populationSize / 10);
                var next = ordered
                    .Take(eliteCount)
                    .Select(c => (int[])c.Clone())
                    .ToList();

                // e) Fill the rest with crossover + mutation
                while (next.Count < populationSize)
                {
                    // pick two parents from the top half
                    var p1 = ordered[_random.Next(populationSize / 2)];
                    var p2 = ordered[_random.Next(populationSize / 2)];
                    int cross = _random.Next(nShifts);      // split point

                    var child = new int[nShifts];
                    for (int j = 0; j < nShifts; j++)
                        child[j] = (j < cross ? p1[j] : p2[j]);

                    // mutation: reassign one random shift
                    if (_random.NextDouble() < 0.1)
                    {
                        int mIdx = _random.Next(nShifts);   // safe: 0 ≤ mIdx < nShifts
                        child[mIdx] = _random.Next(nEmps);  // safe: 0 ≤ new employee index < nEmps
                    }

                    next.Add(child);
                }

                population = next;
            }

            // 3) Apply best chromosome back onto your shifts
            for (int i = 0; i < nShifts; i++)
            {
                int empIdx = best[i];
                shifts[i].AssignedEmployee =
                    (empIdx >= 0 && empIdx < employees.Count)
                        ? employees[empIdx]
                        : null;
            }
        }

        private static double Evaluate(int[] chromosome, List<Employee> emps, List<Shift> shifts)
        {
            double score = 0;
            var busyDays = new[] { DayOfWeek.Sunday, DayOfWeek.Friday };

            for (int i = 0; i < shifts.Count; i++)
            {
                // guard just in case
                int gene = chromosome[i];
                if (gene < 0 || gene >= emps.Count)
                    continue;

                var emp   = emps[gene];
                var shift = shifts[i];

                // availability
                bool available = emp.Availability
                    .Any(a => a.Day == shift.Day && a.Time == shift.ShiftTime);
                score += available ? +1 : -10;

                // skills
                if (shift.RequiredSkills.Any())
                {
                    bool hasAll = shift.RequiredSkills
                        .All(req => emp.Skills.Contains(req));
                    score += hasAll ? +2 : -5;
                }

                // encourage temps on busy days
                bool isBusyDay = busyDays.Contains(shift.Day);
                if (emp.EmploymentType == EmploymentType.Temporary)
                {
                    if (isBusyDay)
                        score += 2;
                    else
                    {
                        bool permAvail = emps.Any(o =>
                            o.EmploymentType == EmploymentType.Permanent
                            && o.Availability.Any(a => a.Day == shift.Day && a.Time == shift.ShiftTime)
                            && shift.RequiredSkills.All(req => o.Skills.Contains(req))
                        );
                        if (permAvail) score -= 3;
                    }
                }

                // time preferences
                if (emp.PrefersMorning && shift.ShiftTime == ShiftTime.Evening) score -= 1;
                if (emp.PrefersEvening && shift.ShiftTime == ShiftTime.Morning) score -= 1;

                // weekend avoidance
                bool weekend = shift.Day == DayOfWeek.Saturday || shift.Day == DayOfWeek.Sunday;
                if (emp.AvoidsWeekends && weekend) score -= 2;
            }

            return score;
        }
    }
}
