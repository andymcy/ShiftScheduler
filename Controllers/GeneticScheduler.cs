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
            int populationSize = 30;
            int generations    = 50;
            int nShifts        = shifts.Count;
            int nEmps          = employees.Count;

            // Initialize population
            var population = new List<int[]>();
            for (int i = 0; i < populationSize; i++)
            {
                var chromosome = new int[nShifts];
                for (int j = 0; j < nShifts; j++)
                    chromosome[j] = _random.Next(nEmps);
                population.Add(chromosome);
            }

            int[] best        = population[0];
            double bestFitness = double.NegativeInfinity;

            // Evolve generations
            for (int gen = 0; gen < generations; gen++)
            {
                var fits = population
                    .Select(ch => Evaluate(ch, employees, shifts))
                    .ToArray();

                // Track best
                for (int i = 0; i < populationSize; i++)
                {
                    if (fits[i] > bestFitness)
                    {
                        bestFitness = fits[i];
                        best        = (int[])population[i].Clone();
                    }
                }

                // Sort by fitness descending
                var ordered = population
                    .Zip(fits, (chrom, fit) => (chrom, fit))
                    .OrderByDescending(cf => cf.fit)
                    .Select(cf => cf.chrom)
                    .ToList();

                // Keep top 10%
                var next = ordered.Take(populationSize / 10)
                                  .Select(c => (int[])c.Clone())
                                  .ToList();

                // Fill rest by crossover + mutation
                while (next.Count < populationSize)
                {
                    var p1    = ordered[_random.Next(populationSize / 2)];
                    var p2    = ordered[_random.Next(populationSize / 2)];
                    int cross = _random.Next(nShifts);

                    var child = new int[nShifts];
                    for (int j = 0; j < nShifts; j++)
                        child[j] = j < cross ? p1[j] : p2[j];

                    if (_random.NextDouble() < 0.1)
                        child[_random.Next(nShifts)] = _random.Next(nEmps);

                    next.Add(child);
                }

                population = next;
            }

            // Apply best solution
            for (int i = 0; i < nShifts; i++)
            {
                int empIdx = best[i];
                shifts[i].AssignedEmployee = employees
                    .ElementAtOrDefault(empIdx);
            }
        }

        private static double Evaluate(int[] chromosome, List<Employee> emps, List<Shift> shifts)
        {
            double score = 0;
            var busyDays = new[] { DayOfWeek.Sunday, DayOfWeek.Friday };

            for (int i = 0; i < shifts.Count; i++)
            {
                var emp   = emps[chromosome[i]];
                var shift = shifts[i];

                // 1) availability
                bool available = emp.Availability
                    .Any(a => a.Day == shift.Day && a.Time == shift.ShiftTime);
                score += available ? +1 : -10;

                // 2) required skills
                if (shift.RequiredSkills.Any())
                {
                    bool hasAll = shift.RequiredSkills
                        .All(req => emp.Skills.Contains(req));
                    score += hasAll ? +2 : -5;
                }

                // 3) temporary bonus/penalty
                bool isBusyDay = busyDays.Contains(shift.Day);
                if (emp.EmploymentType == EmploymentType.Temporary)
                {
                    if (isBusyDay)
                    {
                        // Encourage temporaries on busy days
                        score += 2;
                    }
                    else
                    {
                        // Penalize if a permanent could fill
                        bool permAvail = emps.Any(o =>
                            o.EmploymentType == EmploymentType.Permanent
                            && o.Availability.Any(a => a.Day == shift.Day && a.Time == shift.ShiftTime)
                            && shift.RequiredSkills.All(req => o.Skills.Contains(req))
                        );
                        if (permAvail) score -= 3;
                    }
                }

                // 4) soft preferences
                if (emp.PrefersMorning && shift.ShiftTime == ShiftTime.Evening) score -= 1;
                if (emp.PrefersEvening && shift.ShiftTime == ShiftTime.Morning) score -= 1;
                bool weekend = shift.Day == DayOfWeek.Saturday || shift.Day == DayOfWeek.Sunday;
                if (emp.AvoidsWeekends && weekend) score -= 2;
            }

            return score;
        }
    }
}
