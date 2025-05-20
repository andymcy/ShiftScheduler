using System;
using System.Collections.Generic;
using System.Linq;
using ShiftScheduler.Models;

namespace ShiftScheduler.Services
{
    // 100 % compatible with your previous static GeneticScheduler,
    // but now in a DI-friendly class.
    public class ScheduleSolver
    {
        private readonly Random _rand = new();

        // A *very* small GA example; replace with your real OR-Tools code.
        public List<ShiftAssignment> GenerateSchedule(
            List<Employee> employees,
            List<Shift>    shifts)
        {
            // --- prepare data for GA ---
            int popSize    = 30;
            int generations= 50;
            int nShifts    = shifts.Count;
            int nEmps      = employees.Count;

            // population = list of chromosomes; each chromosome = int[nShifts]
            var population = new List<int[]>();
            for (int i = 0; i < popSize; i++)
            {
                var chrom = new int[nShifts];
                for (int j = 0; j < nShifts; j++)
                    chrom[j] = _rand.Next(nEmps);
                population.Add(chrom);
            }

            int[]   bestChrom   = population[0];
            double  bestFitness = double.NegativeInfinity;

            for (int gen = 0; gen < generations; gen++)
            {
                foreach (var chrom in population)
                {
                    double fit = Fitness(chrom, employees, shifts);
                    if (fit > bestFitness)
                    {
                        bestFitness = fit;
                        bestChrom   = chrom;
                    }
                }

                // very naive mutation
                population = population.Select(c =>
                {
                    var copy = c.ToArray();
                    if (_rand.NextDouble() < 0.3)
                        copy[_rand.Next(nShifts)] = _rand.Next(nEmps);
                    return copy;
                }).ToList();
            }

            // translate best chromosome into assignments
            var result = new List<ShiftAssignment>();
            for (int i = 0; i < nShifts; i++)
            {
                result.Add(new ShiftAssignment
                {
                    ShiftId        = shifts[i].ShiftId,
                    Date           = DateTime.Today,  // set real date outside
                    MainEmployeeId = employees[bestChrom[i]].EmployeeId,
                    BackupEmployeeId = employees[_rand.Next(nEmps)].EmployeeId
                });
            }
            return result;
        }

        // simple fitness: +1 per filled shift, −2 per weekend if AvoidsWeekends
        private double Fitness(int[] chrom, List<Employee> emps, List<Shift> shifts)
        {
            double score = 0;
            for (int i = 0; i < chrom.Length; i++)
            {
                var e = emps[chrom[i]];
                var s = shifts[i];

                score += 1; // filled

                if (s.DayOfWeek is "Saturday" or "Sunday" && e.AvoidsWeekends)
                    score -= 2;
                if (s.ShiftTime == "Morning" && !e.PrefersMorning)
                    score -= 1;
                if (s.ShiftTime == "Evening" && !e.PrefersEvening)
                    score -= 1;
            }
            return score;
        }
    }
}
