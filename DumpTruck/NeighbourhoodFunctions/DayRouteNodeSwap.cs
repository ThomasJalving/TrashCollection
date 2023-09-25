using System;
using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;

namespace DumpTruck.NeighbourhoodFunctions
{
    internal class DayRouteNodeSwap : INeighbourhoodFunction
    {
        public void ApplyFunction(Week week, NeighbourDelta neighbourDelta)
        {
            throw new NotImplementedException();
        }

        public NeighbourDelta SuggestRandomNeighbour(Week week)
        {
            var ranDay = RandomHelper.RandomDay(week);
            var rndNode1 = RandomHelper.RandomDoneNode(ranDay);
            var rndNode2 = RandomHelper.RandomDoneNode(ranDay);
            var node1 = rndNode1.Node;
            var node2 = rndNode2.Node;
            while (node1.Order.Id == node2.Order.Id) rndNode2 = RandomHelper.RandomDoneNode(ranDay);
            node2 = rndNode2.Node;
            //Get node cost and expected new cost
            var node1new = node1.Route == null;
            var node2new = node2.Route == null;
            var duration1Delta = -NodeDuration(node1) + NodeDuration(node2, node1.Previous, node1.Next, node1.Route);
            var duration2Delta = -NodeDuration(node2) + NodeDuration(node1, node2.Previous, node2.Next, node1.Route);
            throw new NotImplementedException();
        }

        private static double NodeDuration(Node node)
        {
            return NodeDuration(node, node.Previous, node.Next, node.Route);
        }

        private static double NodeDuration(Node node, Node? previous, Node? next, Route? route)
        {
            double duration = 0;
            ;
            if (route == null)
            {
            }
            else
            {
                duration += node.Order.TimeToEmpty;
                if (next == null) //next node is dump
                    duration += Week.Distances[(node.Order.MatrixId, Constants.DUMP_ID)];
                else
                    duration += Week.Distances[(node.Order.MatrixId, next.Order.MatrixId)];
                if (previous == null) //previous node is dump
                    duration += Week.Distances[(Constants.DUMP_ID, node.Order.MatrixId)];
                else
                    duration += Week.Distances[(previous.Order.MatrixId, node.Order.MatrixId)];
            }

            return duration;
        }
    }
}