using System;
using System.Collections.Generic;
using System.Linq;

namespace ShiftScheduler.Models
{
    public static class GeneticScheduler
    {
        private static Random _rand = new Random();

        public static List<ScheduleCandidate> GenerateInitialPopulation(int size, List<Employee> employees, List<Shift> shifts)
        {
            var population = new List<ScheduleCandidate>();

            for (int i = 0; i < size; i++)
            {
                var candidate = new ScheduleCandidate();

                foreach (var shift in shifts)
                {
                    var assign = new ShiftAssignment { Shift = shift };
                    var pool = new List<Employee>(employees);

                    for (int j = 0; j < shift.RequiredEmployees && pool.Count > 0; j++)
                    {
                        var emp = pool[_rand.Next(pool.Count)];
                        assign.AssignedEmployees.Add(emp);
                        pool.Remove(emp);
                    }

                    candidate.Assignments.Add(assign);
                }

                candidate.CalculateFitness();
                population.Add(candidate);
            }

            return population;
        }

        public static List<ShiftAssignment> GenerateSchedule(List<Employee> employees, List<Shift> shifts)
        {
            int popSize = 10, generations = 30;
            var population = GenerateInitialPopulation(popSize, employees, shifts);

            for (int g = 0; g < generations; g++)
            {
                foreach (var p in population)
                    p.CalculateFitness();

                var top2 = population.OrderByDescending(p => p.Fitness).Take(2).ToList();
                var newPop = new List<ScheduleCandidate>(top2);

                while (newPop.Count < popSize)
                {
                    var (c1, c2) = Crossover(top2[0], top2[1]);
                    if (_rand.NextDouble() < 0.3) Mutate(c1, employees);
                    if (_rand.NextDouble() < 0.3) Mutate(c2, employees);
                    c1.CalculateFitness();
                    c2.CalculateFitness();
                    newPop.Add(c1);
                    if (newPop.Count < popSize)
                        newPop.Add(c2);
                }

                population = newPop;
            }

            return population.OrderByDescending(p => p.Fitness).First().Assignments;
        }

        private static (ScheduleCandidate, ScheduleCandidate) Crossover(ScheduleCandidate p1, ScheduleCandidate p2)
        {
            int point = _rand.Next(p1.Assignments.Count);
            var c1 = new ScheduleCandidate();
            var c2 = new ScheduleCandidate();

            for (int i = 0; i < p1.Assignments.Count; i++)
            {
                c1.Assignments.Add(Clone(i < point ? p1.Assignments[i] : p2.Assignments[i]));
                c2.Assignments.Add(Clone(i < point ? p2.Assignments[i] : p1.Assignments[i]));
            }

            return (c1, c2);
        }

        private static ShiftAssignment Clone(ShiftAssignment original)
        {
            return new ShiftAssignment
            {
                Shift = original.Shift,
                AssignedEmployees = new List<Employee>(original.AssignedEmployees)
            };
        }

        private static void Mutate(ScheduleCandidate c, List<Employee> pool)
        {
            if (c.Assignments.Count == 0) return;
            int shiftIndex = _rand.Next(c.Assignments.Count);
            var assign = c.Assignments[shiftIndex];
            if (assign.AssignedEmployees.Count == 0) return;

            int empIndex = _rand.Next(assign.AssignedEmployees.Count);
            var others = pool.Where(e => !assign.AssignedEmployees.Contains(e)).ToList();
            if (others.Count == 0) return;

            assign.AssignedEmployees[empIndex] = others[_rand.Next(others.Count)];
        }
    }
}
