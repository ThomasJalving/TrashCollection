using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;
using DumpTruck.ScoreStatics;

namespace DumpTruck
{
    internal static class InitialSolutionCreator
    {
        //Creates an initial solution with two routes per truck for each day and set the starting score
        public static Week CreateInitialSolution()
        {
            var days = new Day[5];
            for (var i = 0; i < days.Length; i++)
            {
                var day = new Day(i);
                days[i] = day;
            }
            var week = new Week(days);
            for (var i = 0; i < days.Length; i++)
            {
                int[] pattern = new int[5];
                pattern[i] = 1;
                week.Days[i].Trucks[0].Add(RandomDayRoute(week.Days[i], pattern, 0));
                week.Days[i].Trucks[0].Add(RandomDayRoute(week.Days[i], pattern, 0));
                week.Days[i].Trucks[1].Add(RandomDayRoute(week.Days[i], pattern, 1));
                week.Days[i].Trucks[1].Add(RandomDayRoute(week.Days[i], pattern, 1));
            }
            week.Score = OptSolutionChecker.Score(week);
            return week;
        }

        //Create a randomroute with the set maximum length and orders that have a frequency of 1
        public static Route RandomDayRoute(Day day, int[] pattern, int truckId)
        {
            var order = RandomHelper.RandomNotDoneOrder(day.parentWeek);
            while(Week.Orders[order.OrderId].Pwk != 1)
                order = RandomHelper.RandomNotDoneOrder(day.parentWeek);
            var route = new Route(order, pattern, day, truckId);
            day.parentWeek.AddCompleteOrder(order, pattern);
            while (route.Duration < Constants.MAX_ROUTE_DURATION / 2.5f)
            {
                order = RandomHelper.RandomNotDoneOrder(day.parentWeek);
                while (Week.Orders[order.OrderId].Pwk != 1)
                    order = RandomHelper.RandomNotDoneOrder(day.parentWeek);
                route.AppendNode(order, pattern);
                day.parentWeek.AddCompleteOrder(order, pattern);
            }

            return route;
        }
    }
}