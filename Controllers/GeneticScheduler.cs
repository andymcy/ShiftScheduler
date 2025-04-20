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

            // 1) initialize population (each chromosome is an array of employee‐indices)
            var population = new List<int[]>();
            for (int i = 0; i < populationSize; i++)
            {
                var chromosome = new int[nShifts];
                for (int j = 0; j < nShifts; j++)
                    chromosome[j] = _random.Next(nEmps);
                population.Add(chromosome);
            }

            int[] best = population[0];
            double bestFit = double.NegativeInfinity;

            // 2) evolve
            for (int gen = 0; gen < generations; gen++)
            {
                // evaluate fitness
                var fits = population.Select(ch => Evaluate(ch, employees, shifts)).ToArray();
                // pick the best
                for (int i = 0; i < populationSize; i++)
                    if (fits[i] > bestFit)
                    {
                        bestFit = fits[i];
                        best = (int[])population[i].Clone();
                    }

                // sort by fitness descending
                var ordered = population
                    .Zip(fits, (chrom, fit) => (chrom, fit))
                    .OrderByDescending(cf => cf.fit)
                    .Select(cf => cf.chrom)
                    .ToList();

                // carry over top 10%
                var next = ordered.Take(populationSize / 10).Select(c => (int[])c.Clone()).ToList();

                // fill the rest by crossover + mutation
                while (next.Count < populationSize)
                {
                    // pick two parents
                    var p1 = ordered[_random.Next(0, populationSize / 2)];
                    var p2 = ordered[_random.Next(0, populationSize / 2)];
                    // one‐point crossover
                    int cross = _random.Next(nShifts);
                    var child = new int[nShifts];
                    for (int j = 0; j < nShifts; j++)
                        child[j] = j < cross ? p1[j] : p2[j];
                    // small mutation
                    if (_random.NextDouble() < 0.1)
                        child[_random.Next(nShifts)] = _random.Next(nEmps);
                    next.Add(child);
                }

                population = next;
            }

            // 3) apply best to your shifts
            for (int i = 0; i < nShifts; i++)
            {
                int empIdx = best[i];
                shifts[i].AssignedEmployee = (empIdx >= 0 && empIdx < employees.Count)
                    ? employees[empIdx]
                    : null;
            }

            // optionally assign backup if needed (omitted for brevity)
        }

        private static double Evaluate(int[] chromosome, List<Employee> emps, List<Shift> shifts)
        {
            double score = 0;
            int n = shifts.Count;
            for (int i = 0; i < n; i++)
            {
                var emp = emps[chromosome[i]];
                var shift = shifts[i];

                // 1) availability
                bool ok = emp.Availability.Any(a => a.Day == shift.Day && a.Time == shift.ShiftTime);
                score += ok ? +1 : -10;

                // 2) skills
                if (shift.RequiredSkills.Any())
                {
                    bool hasAll = shift.RequiredSkills.All(req => emp.Skills.Contains(req));
                    score += hasAll ? +2 : -5;
                }

                // 3) type preference
                if (emp.EmploymentType == EmploymentType.Temporary)
                {
                    // penalize temporaries if a permanent could fill
                    bool permAvail = emps.Any(o =>
                        o.EmploymentType == EmploymentType.Permanent &&
                        o.Availability.Any(a => a.Day == shift.Day && a.Time == shift.ShiftTime) &&
                        shift.RequiredSkills.All(req => o.Skills.Contains(req))
                    );
                    if (permAvail) score -= 3;
                }

                // 4) soft prefs
                if (emp.PrefersMorning && shift.ShiftTime == ShiftTime.Evening) score -= 1;
                if (emp.PrefersEvening && shift.ShiftTime == ShiftTime.Morning) score -= 1;
                bool isWeekend = shift.Day == DayOfWeek.Saturday || shift.Day == DayOfWeek.Sunday;
                if (emp.AvoidsWeekends && isWeekend) score -= 2;
            }
            return score;
        }
    }
}
