using DumpTruck.HelperClasses;

namespace DumpTruck.Datastructures
{
    public class Route
    {
        //The id of the truck that serves this route
        public readonly int TruckId;
        //The garbage volume that is accumulated by the end of the route
        public int CurrentVolume;
        
        public Day ParentDay { get; }
        public Node FirstNode { get; set; }
        public Node LastNode { get; set; }
        public double Duration { get; set; } = 1_800;

        //Constructs a route with its initial node and calculates the starting duration and volume
        //Also sets the order as completed for the week
        public Route(RandomOrder order, int[] pattern, Day day, int truck)
        {
            System.Console.WriteLine("new route");
            ParentDay = day;
            var initialNode = ParentDay.NodeContainer[order.OrderId];
            Duration += Week.Distances[(Constants.DUMP_ID, initialNode.Order.MatrixId)] +
                        Week.Distances[(initialNode.Order.MatrixId, Constants.DUMP_ID)] +
                        initialNode.Order.TimeToEmpty;
            CurrentVolume += initialNode.Order.Volume;
            FirstNode = initialNode;
            LastNode = initialNode;
            initialNode.Route = this;
            TruckId = truck;
        }
        //Appends a node for an order at the end of the route and updates volume and duration
        //Also sets the order as completed for the week
        public void AppendNode(RandomOrder order, int[] pattern)
        {
            System.Console.WriteLine("append");
            var node = ParentDay.NodeContainer[order.OrderId];
            if (node.Previous != null || node.Next != null) return;
            Duration += Week.Distances[(LastNode.Order.MatrixId, node.Order.MatrixId)] + node.Order.TimeToEmpty;
            Duration += Week.Distances[(node.Order.MatrixId, Constants.DUMP_ID)] -
                        Week.Distances[(LastNode.Order.MatrixId, Constants.DUMP_ID)];
            CurrentVolume += node.Order.Volume;
            node.Previous = LastNode;
            LastNode.Next = node;
            LastNode = node;
            node.Route = this;
        }
    }
}