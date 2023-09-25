using System;
using System.Collections.Generic;
using System.Linq;
using DumpTruck.Datastructures;

namespace DumpTruck.HelperClasses
{
    public static class OrderHelper
    {
        public static bool OrderIsValid(Week week, Order order) =>
            SatisfiesPattern(week, order) ||
            OrderDoesNotOccur(week, order);

        public static bool OrderDeservesPenalty(Week week, Order order) =>
            OrderDoesNotOccur(week, order);

        public static int MinimalPatternDistance(Week week, Order order) =>
            MinimalPatternDistance(order.Pwk, CurrentPattern(week, order));

        // Aims to find the closest allowable pattern toa given pattern, given the weekly required frequency
        public static int MinimalPatternDistance(int pwk, IEnumerable<int> pattern) =>
            GetPossiblePatterns(pwk).Aggregate(int.MaxValue, (min, next) =>
            {
                var dist = Distance(next, pattern);
                return dist < min ? dist : min;
            });

        // Manhattan distance in (Z/2Z)^5
        private static int Distance(IEnumerable<int> pattern1, IEnumerable<int> pattern2) =>
            pattern1
                .Zip(pattern2)
                .Select(tuple => Math.Abs(tuple.First - tuple.Second))
                .Sum();


        // Hardcoded every allowable pattern given your weekly frequency pwk
        public static int[][] GetPossiblePatterns(int pwk) =>
            pwk switch
            {
                1 => new[]
                {
                    new[] {1, 0, 0, 0, 0}, new[] {0, 1, 0, 0, 0}, new[] {0, 0, 1, 0, 0}, new[] {0, 0, 0, 1, 0},
                    new[] {0, 0, 0, 0, 1}
                },
                2 => new[] {new[] {1, 0, 0, 1, 0}, new[] {0, 1, 0, 0, 1}},
                3 => new[] {new[] {1, 0, 1, 0, 1}},
                4 => new[]
                {
                    new[] {1, 1, 1, 1, 0}, new[] {1, 1, 1, 0, 1}, new[] {1, 1, 0, 1, 1}, new[] {1, 0, 1, 1, 1},
                    new[] {0, 1, 1, 1, 1}
                },
                5 => new[] {new[] {1, 1, 1, 1, 1}},
                _ => new[] {Array.Empty<int>()}
            };

        public static bool OrderDoesNotOccur(Week week, Order order) => 
            CurrentPattern(week, order).Sum() == 0;

        public static bool SatisfiesPattern(Week week, Order order)
        {
            return SatisfiesPattern(order.Pwk, CurrentPattern(week, order));
        }

        public static bool SatisfiesPattern(int pwk, IEnumerable<int> satisfactionPerDay)
        {
            return MinimalPatternDistance(pwk, satisfactionPerDay) == 0;
        }
        
        public static int[] CurrentPattern(Week week, Order order)
        {
            return week.Days
                .Select(d => d.OrdersDoneThisDay.Contains(order.Id) ? 1 : 0)
                .ToArray();
        }

        // Safe wrapper to get the distance to get from one node to another
        public static double NodeDriveDuration(Node fromNode, Node toNode)
        {
            return Week.Distances[(NodeMatrixId(fromNode), NodeMatrixId(toNode))];
        }

        // Returns the matrix ID, except when it's the dump, because it has no associated order, we use it implicitly
        public static int NodeMatrixId(Node node)
        {
            return node?.Order.MatrixId ?? Constants.DUMP_ID;
        }
    }
}