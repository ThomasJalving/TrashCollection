using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;
using System;

namespace DumpTruck.NeighbourhoodFunctions
{
    public class Add : INeighbourhoodFunction
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
            var nodesFrom = new RandomNode[5];
            double delta = 0;
            for (var i = 0; i < 5; i++)
            {
                if (pattern[i] != 1) continue;
                var randomNewNodeTo = week.Days[i].NodeContainer[rndOrder.OrderId];
                var randomNodeFrom = RandomHelper.RandomDoneNode(week.Days[i]);
                if (randomNodeFrom.Failure)
                {
                    return new NeighbourDelta
                    {
                        Delta = double.PositiveInfinity
                    };
                }
                nodesFrom[i] = randomNodeFrom;
                var timeDelta = DistanceDelta(randomNodeFrom.Node, randomNewNodeTo);
                timeDelta += randomNewNodeTo.Order.TimeToEmpty;
                delta += timeDelta;
                delta += PenaltyHelper.DurationPenaltyDelta(randomNodeFrom.Node.Route.ParentDay, randomNodeFrom.Node.Route.TruckId, timeDelta);
                delta += PenaltyHelper.WeightPenaltyDelta(randomNodeFrom.Node.Route.CurrentVolume, randomNewNodeTo.Order.Volume);
            }
            var order = Week.Orders[rndOrder.OrderId];
            delta -= PenaltyHelper.SatisfactionPenalty(order.Pwk, order);

            return new NeighbourDelta
            {
                Delta = delta,
                Parameters = new object[] {pattern, rndOrder, nodesFrom }
            };
        }

        //Adds the order and the corresponding nodes given the pattern to the correct routes
        //changes the route duration and volume
        public void ApplyFunction(Week week, NeighbourDelta neighbourDelta)
        {
            var pattern = (int[]) neighbourDelta.Parameters[0];
            var rndOrder = (RandomOrder) neighbourDelta.Parameters[1];
            var randomFromNodes = (RandomNode[]) neighbourDelta.Parameters[2];
            for (var i = 0; i < 5; i++)
            {
                if (pattern[i] != 1) continue;
                var newNode = week.Days[i].NodeContainer[rndOrder.OrderId];
                var fromNode = randomFromNodes[i].Node;
                var route = fromNode.Route;

                route.CurrentVolume += newNode.Order.Volume;
                route.Duration += DistanceDelta(fromNode, newNode) + newNode.Order.TimeToEmpty;

                if (route == null)
                {
                    System.Console.WriteLine("A node route was null");
                }
                newNode.Previous = fromNode;
                if (fromNode == route.LastNode)
                {
                    fromNode.Next = newNode;
                    route.LastNode = newNode;
                }
                else
                {
                    newNode.Next = fromNode.Next;
                    fromNode.Next = newNode;
                    newNode.Next.Previous = newNode;
                }

                newNode.Route = route;

                
            }
            week.AddCompleteOrder(rndOrder, pattern);
        }

        //Delta in travel time if adding toNode after fromNode
        private static double DistanceDelta(Node fromNode, Node toNode) =>
            OrderHelper.NodeDriveDuration(fromNode, toNode) +
            OrderHelper.NodeDriveDuration(toNode, fromNode.Next) -
            OrderHelper.NodeDriveDuration(fromNode, fromNode.Next);
    }
}