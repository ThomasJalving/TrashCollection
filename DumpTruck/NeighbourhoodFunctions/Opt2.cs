using System;
using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;

namespace DumpTruck.NeighbourhoodFunctions
{
    public class Opt2 : INeighbourhoodFunction
    {
        public NeighbourDelta SuggestRandomNeighbour(Week week)
        {
            var rndDay = RandomHelper.RandomDay(week);
            var rndSlice = RandomHelper.RandomSlice(rndDay);
            if (rndSlice == (null, null)) return new NeighbourDelta {Delta = double.PositiveInfinity};
            var durationDelta = DurationDelta(rndSlice);
            //penalties
            durationDelta += PenaltyHelper.DurationPenaltyDelta(rndDay, rndSlice.beginNode.Route.TruckId, durationDelta);
            return new NeighbourDelta {Delta = durationDelta, Parameters = new object[] {rndSlice}};
        }

        public void ApplyFunction(Week week, NeighbourDelta neighbourDelta)
        {
            var (beginNode, endNode) = ((Node, Node)) neighbourDelta.Parameters[0];
            beginNode.Route.Duration += neighbourDelta.Delta;
            
            // Starting situation: https://imgur.com/hxYDzAP
            var beginNodePrev = beginNode.Previous;
            var endNodeNext = endNode.Next;
            var beginNodeNext = beginNode.Next;
            var endNodePrev = endNode.Previous;
            
            // Remove cross: https://imgur.com/kdYobqc
            SafeSetNextNodeTo(beginNodePrev, endNode);
            endNode.Previous = beginNodePrev;
            SafeSetPrevNodeTo(endNodeNext, beginNode);
            beginNode.Next = endNodeNext;
            
            // Replace weird lines and prepare for flipping route: https://imgur.com/fZa4qIY
            beginNode.Previous = beginNodeNext;
            endNode.Next = endNodePrev;
            
            // Flip route, after one iteration: https://imgur.com/wKxg4Q2 , after all iterations: https://imgur.com/9xz1zh7
            var iterateNode = beginNodeNext;
            while (iterateNode != endNode)
            {
                var next = iterateNode.Next;
                iterateNode.Next = iterateNode.Previous;
                iterateNode.Previous = next;
                iterateNode = next;
            }

        }

        private void SafeSetPrevNodeTo(Node node, Node toSet)
        {
            if (node == null)       // We have to set the last node of the route, since null means we are at the dump
            {
                toSet.Route.LastNode = toSet;
                return;
            }

            node.Previous = toSet;
        }

        private void SafeSetNextNodeTo(Node node, Node toSet)
        {
            if (node == null)       // We have to set the first node of the route, since null means we are at the dump
            {
                toSet.Route.FirstNode = toSet;
                return;
            }

            node.Next = toSet;
        }

        private double DurationDelta((Node, Node) rndSlice)
        {
            var (beginNode, endNode) = rndSlice;
            var beginNodePrev = beginNode.Previous;
            var endNodeNext = endNode.Next;
            var durationDelta = 0d;
            // First recalculate the costs for removing the cross, i.e. b-e, c-f in image https://bit.ly/35FJBth
            durationDelta += -OrderHelper.NodeDriveDuration(beginNodePrev, beginNode)
                             + OrderHelper.NodeDriveDuration(beginNodePrev, endNode)
                             - OrderHelper.NodeDriveDuration(endNode, endNodeNext)
                             + OrderHelper.NodeDriveDuration(beginNode, endNodeNext);
            // Now calculate the costs of flipping al the intermediate ones
            var iterateNode = beginNode.Next;
            while (iterateNode != endNode.Next)
            {
                durationDelta -= OrderHelper.NodeDriveDuration(iterateNode.Previous, iterateNode);
                durationDelta += OrderHelper.NodeDriveDuration(iterateNode, iterateNode.Previous); //This goes wrong at least for the beginNode
                iterateNode = iterateNode.Next;
            }
            return durationDelta;
        }
    }
}