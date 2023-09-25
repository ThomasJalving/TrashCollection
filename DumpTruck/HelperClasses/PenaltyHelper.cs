using System;
using DumpTruck.Datastructures;

namespace DumpTruck.HelperClasses
{
    public static class PenaltyHelper
    {
        public static double SatisfactionPenalty(int currentDistance, Order order)
        {
            return currentDistance * 3 * Constants.SATISFACTION_PENALTY_MULTIPLIER * order.TimeToEmpty;
        }
        

        public static double DurationPenaltyDelta(Day day, int truckId, double durationDelta)
        {
            var currentDuration = day.TruckDuration(truckId); //Should be made O(1)
            var newOvertime = Math.Max(0, currentDuration + durationDelta - Constants.MAX_DAY_TIME);
            return newOvertime > 0 ? double.PositiveInfinity : 0;
        }
        
        public static double OvertimePenalty(double overTime) =>
            Constants.DURATION_PENALTY_MULTIPLIER * overTime;

        public static double WeightPenaltyDelta(double routeVolume, double addedVolume)
        {
            var currentOverflow = Math.Max(0, routeVolume - Constants.MAX_TRUCK_VOLUME);
            var newOverflow = Math.Max(0, routeVolume + addedVolume - Constants.MAX_TRUCK_VOLUME);
            var currentPenalty = OverloadPenalty(currentOverflow);
            var newPenalty = OverloadPenalty(newOverflow);
            return newPenalty - currentPenalty;
        }

        public static double OverloadPenalty(in double overLoad) =>
            Constants.CAPACITY_PENALTY_MULTIPLIER * overLoad;
    }
}