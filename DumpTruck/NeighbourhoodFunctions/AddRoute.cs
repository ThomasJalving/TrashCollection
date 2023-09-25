using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;
using System.Collections.Generic;

namespace DumpTruck.NeighbourhoodFunctions
{
    public class AddRoute : INeighbourhoodFunction
    {
        //Gets a random order and a random pattern that is valid for this orders frequency
        //Calculates the cost for adding it at random places on random routes for this pattern
        public NeighbourDelta SuggestRandomNeighbour(Week week)
        {
            var rndOrder = RandomHelper.RandomNotDoneOrder(week);
            if (rndOrder.Failure)
            {
                return new NeighbourDelta
                {
                    Delta = double.PositiveInfinity
                };
            }
            var patternPossibilities = OrderHelper.GetPossiblePatterns(Week.Orders[rndOrder.OrderId].Pwk);
            var pattern = patternPossibilities[RandomHelper.Random.Next(patternPossibilities.Length)];
            var order = Week.Orders[rndOrder.OrderId];
            int[] trucks = new int[5];
            double delta = 0;
            for (var i = 0; i < 5; i++)
            {
                if (pattern[i] != 1) continue;                
                trucks[i] = RandomHelper.Random.Next(1);
                double timeDelta = Week.Distances[(OrderHelper.NodeMatrixId(null), order.MatrixId)] + Week.Distances[(order.MatrixId, OrderHelper.NodeMatrixId(null))];
                timeDelta += order.TimeToEmpty;
                timeDelta += 1_800;
                if(week.Days[i].Trucks[trucks[i]].Count >= 2)
                {
                    delta += double.PositiveInfinity;
                }
                delta += timeDelta;
                delta += PenaltyHelper.DurationPenaltyDelta(week.Days[i], trucks[i], timeDelta);
                delta += PenaltyHelper.WeightPenaltyDelta(0, order.Volume);
            }
            delta -= PenaltyHelper.SatisfactionPenalty(order.Pwk, order);

            return new NeighbourDelta
            {
                Delta = delta,
                Parameters = new object[] { pattern, rndOrder, trucks }
            };
        }

        //Adds the order and the corresponding nodes given the pattern to the correct routes
        //changes the route duration and volume
        public void ApplyFunction(Week week, NeighbourDelta neighbourDelta)
        {
            var pattern = (int[])neighbourDelta.Parameters[0];
            var rndOrder = (RandomOrder)neighbourDelta.Parameters[1];
            var trucks = (int[])neighbourDelta.Parameters[2];
            for (var i = 0; i < 5; i++)
            {
                if (pattern[i] != 1) continue;
                var route = new Route(rndOrder, pattern, week.Days[i], trucks[i]);
                week.Days[i].Trucks[trucks[i]].Add(route);
            }
            week.AddCompleteOrder(rndOrder, pattern);
        }
    }
}