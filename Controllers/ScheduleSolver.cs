using System;
using System.Collections.Generic;
using System.Linq;
using ShiftScheduler.Models;

namespace ShiftScheduler.Services
{
    public class ScheduleSolver
    {
        private readonly Random _rand = new();

        public List<ShiftAssignment> GenerateSchedule(
            List<Employee> employees,
            List<Shift> shifts,
            int popSize = 50,
            int generations = 100,
            double mutationRate = 0.1)
        {
            int nShifts = shifts.Count;
            int nEmps = employees.Count;

            // Initialize population
            var population = new List<int[]>();
            for (int i = 0; i < popSize; i++)
            {
                var chromosome = new int[nShifts];
                for (int j = 0; j < nShifts; j++)
                    chromosome[j] = _rand.Next(nEmps);
                population.Add(chromosome);
            }

            int[] bestChrom = population[0];
            double bestFitness = double.NegativeInfinity;

            for (int gen = 0; gen < generations; gen++)
            {
                // Evaluate fitness and find the best chromosome
                foreach (var chrom in population)
                {
                    double fitness = Fitness(chrom, employees, shifts);
                    if (fitness > bestFitness)
                    {
                        bestFitness = fitness;
                        bestChrom = chrom;
                    }
                }

                // Selection (Tournament)
                var selected = new List<int[]>();
                for (int i = 0; i < popSize; i++)
                {
                    selected.Add(TournamentSelect(population, employees, shifts));
                }

                // Crossover
                population.Clear();
                for (int i = 0; i < popSize / 2; i++)
                {
                    var parent1 = selected[_rand.Next(selected.Count)];
                    var parent2 = selected[_rand.Next(selected.Count)];

                    (int[] child1, int[] child2) = Crossover(parent1, parent2);

                    population.Add(child1);
                    population.Add(child2);
                }

                // Mutation
                foreach (var chrom in population)
                {
                    if (_rand.NextDouble() < mutationRate)
                        Mutate(chrom, nEmps);
                }
            }

            //  best chromosome into shift assignments
            return bestChrom.Select((empIdx, shiftIdx) => new ShiftAssignment
            {
                ShiftId = shifts[shiftIdx].ShiftId,
                MainEmployeeId = employees[empIdx].EmployeeId,
                BackupEmployeeId = employees[_rand.Next(nEmps)].EmployeeId
            }).ToList();
        }

        private double Fitness(int[] chrom, List<Employee> emps, List<Shift> shifts)
        {
            double score = 0;
            for (int i = 0; i < chrom.Length; i++)
            {
                var e = emps[chrom[i]];
                var s = shifts[i];

                score += 1;
                if ((s.DayOfWeek is "Saturday" or "Sunday") && e.AvoidsWeekends)
                    score -= 2;
                if (s.ShiftTime == "Morning" && !e.PrefersMorning)
                    score -= 1;
                if (s.ShiftTime == "Evening" && !e.PrefersEvening)
                    score -= 1;
            }
            return score;
        }

        private int[] TournamentSelect(List<int[]> population, List<Employee> employees, List<Shift> shifts, int tournamentSize = 3)
        {
            var competitors = new List<int[]>();
            for (int i = 0; i < tournamentSize; i++)
                competitors.Add(population[_rand.Next(population.Count)]);

            return competitors.OrderByDescending(c => Fitness(c, employees, shifts)).First();
        }

        private (int[], int[]) Crossover(int[] parent1, int[] parent2)
        {
            int length = parent1.Length;
            int crossPoint = _rand.Next(1, length - 1);

            int[] child1 = new int[length];
            int[] child2 = new int[length];

            for (int i = 0; i < length; i++)
            {
                child1[i] = i < crossPoint ? parent1[i] : parent2[i];
                child2[i] = i < crossPoint ? parent2[i] : parent1[i];
            }

            return (child1, child2);
        }

        private void Mutate(int[] chromosome, int nEmps)
        {
            int geneToMutate = _rand.Next(chromosome.Length);
            chromosome[geneToMutate] = _rand.Next(nEmps);
        }
    }
}