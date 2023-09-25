using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;
using System;

namespace DumpTruck.NeighbourhoodFunctions
{
    public class InRouteNodeSwap : INeighbourhoodFunction
    {
        //swaps the two nodes in the route and changes the duration of the route
        public void ApplyFunction(Week week, NeighbourDelta neighbourDelta)
        {
            var node1 = (Node)neighbourDelta.Parameters[0];
            var node2 = (Node)neighbourDelta.Parameters[1];

            var node1Next = node1.Next;
            var node1Prev = node1.Previous;
            var node2Next = node2.Next;
            var node2Prev = node2.Previous;

            if (node1.Next != node2 && node2.Next != node1)        // 1 & 2 aren't neighbours
            {
                node1.Previous = node2Prev;
                node1.Next = node2Next;
                node2.Previous = node1Prev;
                node2.Next = node1Next;
            }
            else if (node1.Next == node2) //-> 1 -> 2 ->
            {
                NeighbourSwap.SwapNeighbour(node1);
            }
            else //-> 2 -> 1 ->
            {
                NeighbourSwap.SwapNeighbour(node2);
            }

            FirstLastNodeCheck(node1);
            FirstLastNodeCheck(node2);

            CorrectNeighbours(node1);
            CorrectNeighbours(node2);

            node1.Route.Duration += (double)neighbourDelta.Parameters[2];
        }

        //Gets two random nodes in the same route and calculates the cost of swapping them
        public NeighbourDelta SuggestRandomNeighbour(Week week)
        {
            var ranDay = RandomHelper.RandomDay(week);
            var (rndNode1, rndNode2) = RandomHelper.RandomNodePairFromRoute(ranDay);
            var node1 = rndNode1?.Node;
            var node2 = rndNode2?.Node;
            while (node1 == null || node2 == null)
            {
                ranDay = RandomHelper.RandomDay(week);
                (rndNode1, rndNode2) = RandomHelper.RandomNodePairFromRoute(ranDay);
                node1 = rndNode1?.Node;
                node2 = rndNode2?.Node;
            }

            double durationDelta;
            if (node1.Next != node2 && node2.Next != node1)        // 1 & 2 aren't neighbours
            {
                durationDelta = -OrderHelper.NodeDriveDuration(node1.Previous, node1)
                                - OrderHelper.NodeDriveDuration(node1, node1.Next)
                                - OrderHelper.NodeDriveDuration(node2.Previous, node2)
                                - OrderHelper.NodeDriveDuration(node2, node2.Next);
                durationDelta += OrderHelper.NodeDriveDuration(node1.Previous, node2)
                                 + OrderHelper.NodeDriveDuration(node2, node1.Next)
                                 + OrderHelper.NodeDriveDuration(node2.Previous, node1)
                                 + OrderHelper.NodeDriveDuration(node1, node2.Next);
            }
            else if (node1.Next == node2) //-> 1 -> 2 ->
            {
                durationDelta = NeighbourSwap.NeighbourDelta(node1);
            }
            else //-> 2 -> 1 ->
            {
                durationDelta = NeighbourSwap.NeighbourDelta(node2);
            }

            var penaltyDelta = PenaltyHelper.DurationPenaltyDelta(node1.Route.ParentDay, node1.Route.TruckId, durationDelta);

            object[] parameters = { node1, node2, durationDelta };
            return new NeighbourDelta { Delta = durationDelta + penaltyDelta, Parameters = parameters };
        }

        private static void FirstLastNodeCheck(Node node)
        {
            if (node.Next == null) node.Route.LastNode = node;

            if (node.Previous == null) node.Route.FirstNode = node;
        }

        private static void CorrectNeighbours(Node node)
        {
            if (node.Previous != null)
                node.Previous.Next = node;
            if (node.Next != null)
                node.Next.Previous = node;
        }
    }
}