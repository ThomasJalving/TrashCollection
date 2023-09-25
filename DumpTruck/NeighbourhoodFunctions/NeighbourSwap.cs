using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;

namespace DumpTruck.NeighbourhoodFunctions
{
    internal class NeighbourSwap : INeighbourhoodFunction
    {
        public void ApplyFunction(Week week, NeighbourDelta neighbourDelta)
        {
            var node = (Node) neighbourDelta.Parameters[0];
            SwapNeighbour(node);
            node.Route.Duration += (double) neighbourDelta.Parameters[1];
        }

        public NeighbourDelta SuggestRandomNeighbour(Week week)
        {
            var randomDay = RandomHelper.RandomDay(week);
            var rndNode = RandomHelper.RandomDoneNode(randomDay);
            var node = rndNode.Node;
            var randomTruck = randomDay.Trucks[node.Route.TruckId];
            if (node.Next == null || node.Route.LastNode == node.Next) return new NeighbourDelta { Delta = 20000000 };

            var durationDelta = NeighbourDelta(node);

            var penaltyDelta = PenaltyHelper.DurationPenaltyDelta(node.Route.ParentDay, node.Route.TruckId, durationDelta);

            object[] parameters = { node, durationDelta };
            return new NeighbourDelta { Delta = durationDelta + penaltyDelta, Parameters = parameters };
        }

        public static double NeighbourDelta(Node node)
        {
            var durationDelta = -OrderHelper.NodeDriveDuration(node.Previous, node) -
                                OrderHelper.NodeDriveDuration(node, node.Next) -
                                OrderHelper.NodeDriveDuration(node.Next, node.Next.Next);
            durationDelta += OrderHelper.NodeDriveDuration(node.Previous, node.Next) +
                             OrderHelper.NodeDriveDuration(node.Next, node) +
                             OrderHelper.NodeDriveDuration(node, node.Next.Next);
            return durationDelta;
        }

        // 1 --> 2
        public static void SwapNeighbour(Node node1)
        {
            var node2 = node1.Next;
            var next = node2.Next;
            var previous = node1.Previous;
            node1.Next = next;
            node1.Previous = node2;
            node2.Next = node1;
            node2.Previous = previous;


            if (node2.Route.LastNode == node2)
            {
                node2.Route.LastNode = node1;
            }
            else
            {
                next.Previous = node1;
            }

            if (node1.Route.FirstNode == node1)
            {
                node1.Route.FirstNode = node2;
            }
            else
            {
                previous.Next = node2;
            }
        }
    }
}