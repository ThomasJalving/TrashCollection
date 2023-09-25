namespace DumpTruck.Datastructures
{
    //A node in the route linked list structure
    public class Node
    {
        //The previous node in the route
        public Node Previous { get; set; }
        //The next node in the route
        public Node Next { get; set; }
        //The order that this node is a stop for
        public Order Order { get; set; }
        //The parent route of this node
        public Route Route { get; set; }
    }
}