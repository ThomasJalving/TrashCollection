using System;
using System.Collections.Generic;
using DumpTruck.Datastructures;

namespace DumpTruck.HelperClasses
{
    public static class RandomHelper
    {
#if DEBUG
        public static readonly Random Random = new Random(1);
#else
        public static readonly Random Random = new();
#endif

        public static Day RandomDay(Week week)
        {
            return week.Days[Random.Next(5)];
        }

        public static RandomNode RandomDoneNode(Day day)
        {
            var count = day.OrdersDoneThisDay.Count;
            if (count == 0)
                return new RandomNode {Failure = true};
            var randomIndex = Random.Next(count);
            var orderId = day.OrdersDoneThisDay[randomIndex];
            return new RandomNode {Node = day.NodeContainer[orderId], Index = randomIndex};
        }

        public static (RandomNode, RandomNode) RandomPairDoneNodes(Day day)
        {
            var count = day.OrdersDoneThisDay.Count;
            if (count <= 2)
                return (new RandomNode {Failure = true}, new RandomNode {Failure = true});
            var random1Index = Random.Next(count);
            var random2Index = Random.Next(count);
            while (random1Index == random2Index)
                random2Index = Random.Next(count);

            var order1Id = day.OrdersDoneThisDay[random1Index];
            var order2Id = day.OrdersDoneThisDay[random2Index];
            return (new RandomNode {Node = day.NodeContainer[order1Id], Index = random1Index},
                new RandomNode {Node = day.NodeContainer[order2Id], Index = random2Index});
        }


        public static RandomOrder RandomDoneOrder(Week week)
        {
            var count = week.OrdersDoneThisWeek.Count;
            if (count == 0)
                return new RandomOrder {Failure = true};
            var randomIndex = Random.Next(count);
            var orderId = week.OrdersDoneThisWeek[randomIndex];
            return new RandomOrder {OrderId = orderId, Index = randomIndex};
        }

        public static RandomOrder RandomNotDoneOrder(Week week)
        {
            var count = week.OrdersNotDoneThisWeek.Count;
            if (count == 0)
                return new RandomOrder {Failure = true};
            var randomIndex = Random.Next(count);
            var orderId = week.OrdersNotDoneThisWeek[randomIndex];
            return new RandomOrder {OrderId = orderId, Index = randomIndex};
        }

        public static (RandomNode, RandomNode) RandomNodePairFromRoute(Day day)
        {
            var rndNode1 = RandomDoneNode(day);
            if (rndNode1.Failure || rndNode1.Node == rndNode1.Node.Route.FirstNode &&
                rndNode1.Node == rndNode1.Node.Route.LastNode)
                return (null, null);
            var rndNode2 = RandomDoneNode(day);
            while (rndNode1.Node.Route != rndNode2.Node.Route || rndNode1.Node == rndNode2.Node)
                rndNode2 = RandomDoneNode(day);
            return (rndNode1, rndNode2);
        }
        
        // Returns a slice of a route, which begins at beginNode and ends at endNode
        public static (Node beginNode, Node endNode) RandomSlice(Day day)
        {
            var rndBeginNode = RandomDoneNode(day);
            var beginNode = rndBeginNode.Node;
            var iterateNode = beginNode;
            var numOfNextNodes = 0;
            while (iterateNode.Next != null)
            {
                numOfNextNodes++;
                iterateNode = iterateNode.Next;
            }

            if (numOfNextNodes < 2) return (null, null); // Too short to perform 2OPT on it

            iterateNode = beginNode;
            var rndSliceLength = Random.Next(1, numOfNextNodes);
            for (var i = 0; i < rndSliceLength; i++) iterateNode = iterateNode.Next;

            return (beginNode, iterateNode);
        }
    }
}