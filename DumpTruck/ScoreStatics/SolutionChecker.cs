using System;
using System.Linq;
using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;

namespace DumpTruck.ScoreStatics
{
    public static class SolutionChecker
    {
        public static bool ValidateSolution(Week week)
        {
            var allOrdersValid = Week.Orders
                .Select(a => a.Value)
                .All(o => OrderHelper.OrderIsValid(week, o));

            var withinTime = week.Days
                .SelectMany(day => day.Trucks)
                .SelectMany(truck => truck)
                .Select(TimeForRoute)
                .All(b => b <= Constants.MAX_DAY_TIME);

            var truckNotTooFull = week.Days
                .SelectMany(day => day.Trucks)
                .SelectMany(truck => truck)
                .Select(r => r.CurrentVolume)
                .All(v => v <= Constants.MAX_TRUCK_VOLUME);

            return allOrdersValid && withinTime && truckNotTooFull;
        }

        public static double OfficialScore(Week week, bool validate = false)
        {
            // Travel times
            var timeScore = week.Days
                .SelectMany(day => day.Trucks)
                .SelectMany(truck => truck)
                .Sum(TimeForRoute);
            Console.WriteLine($"Traveling time: {timeScore:F1}");

            // Penalties for not obeying orders
            var penaltyScore = Week.Orders
                .Select(t => t.Value)
                .Sum(order => PenaltyScore(week, order));
            Console.WriteLine($"Decline score: {penaltyScore:F1}");

            return !validate || ValidateSolution(week) ? timeScore + penaltyScore : double.NegativeInfinity;
        }

        private static double PenaltyScore(Week week, Order order)
        {
            return OrderHelper.OrderDeservesPenalty(week, order) ? 3 * order.TimeToEmpty * order.Pwk : 0d;
        }

        public static double TimeForRoute(Route route)
        {
            var timeBusy = 0d;
            var currentNode = route.FirstNode;
            var fromId = Constants.DUMP_ID;
            var toId = currentNode.Order.MatrixId;
            while (currentNode.Next != null)
            {
                timeBusy += Week.Distances[(fromId, toId)]; // Drive to the order
                timeBusy += currentNode.Order.TimeToEmpty; // Empty their containers
                currentNode = currentNode.Next;
                fromId = toId; // You wil now depart from here
                toId = currentNode.Order.MatrixId; // This is your next destination
            }
            timeBusy += Week.Distances[(fromId, toId)]; // Drive to the last order
            timeBusy += currentNode.Order.TimeToEmpty; // Empty container and
            timeBusy += Week.Distances[(currentNode.Order.MatrixId, Constants.DUMP_ID)]; // Come back to dump
            timeBusy += 1800d;
            return timeBusy;
        }
    }
}