using System;
using System.Collections.Generic;
using System.Linq;

namespace StaffPicker
{
    internal class Program
    {
        public struct ShiftTime
        {
            public double Start { get; set; }
            public double End { get; set; }
        }
        
        private static readonly List<ShiftTime> ShiftTimes = new List<ShiftTime>
        {
            new ShiftTime {Start = 6.5, End = 13},
            new ShiftTime {Start = 7.5, End = 15},
            new ShiftTime {Start = 8.5, End = 16},
            new ShiftTime {Start = 13, End = 20},
            new ShiftTime {Start = 16, End = 22}
        };

        private static readonly int[] ShiftAssignments = new[]
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7
        };
        
        public static readonly Random RandomGenerator = new Random();
        
        public static void Main(string[] args)
        {
            var nextMonday = GetNextMonday();
            var nextWeek = GetNextWeek(nextMonday);
            var workWeek = GetShifts(nextWeek);

            foreach (var workDay in workWeek.Days)
            {
                Console.WriteLine($"Processing {workDay.Name}");
                
                foreach (var shift in workDay.Shifts)
                {
                    var nextRandomNumber = GetAvailableAssignment(workDay);

                    shift.Assignee = nextRandomNumber;
                    Console.WriteLine($"Person {shift.Assignee} is on Shift {workDay.Shifts.IndexOf(shift) + 1}");
                }

                var unusedAssignments = GetLeftoverAssignment(workDay);
                
                Console.WriteLine($"Person {unusedAssignments} is Off Today");
                Console.WriteLine("------------------------------------------");
            }

            Console.ReadLine();
        }

        public static int GetLeftoverAssignment(WorkDay day)
        {
            var usedAssignments = day.Shifts.Select(shift => shift.Assignee);
            return ShiftAssignments.First(assignment => !usedAssignments.Contains(assignment));
        }

        public static int GetAvailableAssignment(WorkDay day)
        {
            var randomNumberGenerated = RandomGenerator.Next(1, 7);

            while (day.Shifts.Any(x => x.Assignee == randomNumberGenerated))
            {
                randomNumberGenerated = RandomGenerator.Next(1, 7);
            }

            return randomNumberGenerated;
        }

        public static WorkWeek GetShifts(List<DateTime> availableDates) 
        {
            var workWeek = new WorkWeek();
            
            for (var i = 0; i < availableDates.Count; i++)
            {
                var availableDate = availableDates[i];
                var workDay = new WorkDay {Name = availableDate.DayOfWeek};

                foreach (var shiftTime in ShiftTimes)
                {
                    var start = availableDate.AddHours(shiftTime.Start);
                    var end = availableDate.AddHours(shiftTime.End);

                    workDay.Shifts.Add(new Shift {Time = end - start});
                }

                workWeek.Days.Add(workDay);
            }

            return workWeek;
        }
        
        public static List<DateTime> GetNextWeek(DateTime nextMonday)
        {
            var nextWeek = new List<DateTime>();
            for (var i = 0; i < 7; i++) nextWeek.Add(nextMonday.AddDays(i));
            return nextWeek;
        }

        public static DateTime GetNextMonday()
        {
            var daysToAdd = ((int) DayOfWeek.Monday - (int) DateTime.Now.DayOfWeek + 7) % 7;
            var nexttMonday = DateTime.Now.AddDays(daysToAdd);
            return new DateTime(nexttMonday.Year, nexttMonday.Month, nexttMonday.Day, 0, 0, 0);
        }
        
        public class Shift
        {
            public TimeSpan Time { get; set; }
            public int Assignee { get; set; }
        }

       
        public class WorkDay
        {
            public DayOfWeek Name { get; set; }
            public List<Shift> Shifts = new List<Shift>();
        }

        public class WorkWeek
        {
            public List<WorkDay> Days = new List<WorkDay>();
        }
        


    }
}