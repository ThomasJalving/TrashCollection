using System;
using System.Collections.Generic;
using System.Linq;
using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;

namespace DumpTruck.ScoreStatics
{
    public static class OptSolutionChecker
    {
        public static double Score(Week week)
        {
            var travelingTime = week.Days
                .SelectMany(day => day.Trucks)
                .SelectMany(truck => truck)
                .Sum(SolutionChecker.TimeForRoute);

            var loadPenalty = week.Days
                .SelectMany(day => day.Trucks)
                .SelectMany(truck => truck)
                .Sum(LoadPenalty);

            var timePenalty = week.Days
                .SelectMany(day => day.Trucks)
                .Sum(TimePenalty);

            var patternPenalty = Week.Orders.Sum(kvp =>
                PenaltyHelper.SatisfactionPenalty(OrderHelper.MinimalPatternDistance(week, kvp.Value), kvp.Value));

            return travelingTime + loadPenalty + timePenalty + patternPenalty;
        }

        private static double TimePenalty(List<Route> routes)
        {
            var timeBusy = 0d;
            foreach (var route in routes)
            {
                timeBusy += IterateRoute(route).Sum(n =>
                    n.Order.TimeToEmpty +
                    OrderHelper.NodeDriveDuration(n.Previous, n));
                timeBusy += OrderHelper.NodeDriveDuration(route.LastNode, route.LastNode.Next);
            }

            return PenaltyHelper.OvertimePenalty(Math.Max(0, timeBusy - Constants.MAX_ROUTE_DURATION));
        }

        private static double LoadPenalty(Route route)
        {
            var load = IterateRoute(route).Sum(n => n.Order.Volume);
            var overLoad = Math.Max(0, load - Constants.MAX_TRUCK_VOLUME);
            return PenaltyHelper.OverloadPenalty(overLoad);
        }

        private static IEnumerable<Node> IterateRoute(Route route)
        {
            var node = route.FirstNode;
            while (node.Next != null)
            {
                yield return node;
                node = node.Next;
            }

            yield return node;
        }
    }
}