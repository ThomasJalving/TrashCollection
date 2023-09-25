using System.Collections.Generic;
using System.Linq;

namespace DumpTruck.Datastructures
{
    public class Day
    {
        public Week parentWeek;
        //Which day of the week this is
        public readonly int DayId; 
        //Contains all nodes that are in or can be added to route on this day
        public Dictionary<int, Node> NodeContainer = new Dictionary<int, Node>();
        //Contains OrderIds for all orders that have a node in a route on this day
        public OrderSet OrdersDoneThisDay = new OrderSet();
        //Contains two lists of routes one for each truck
        public List<Route>[]
            Trucks = {new List<Route>(8), new List<Route>(8)}; // Default Capacity of 8 for smaller memory usage

        //Day is initialized with it's dayId and the NodeContainer is initialized with an empty node for all orders
        public Day(int dayId)
        {
            DayId = dayId;
            foreach (var kvp in Week.Orders) NodeContainer.Add(kvp.Key, new Node {Order = kvp.Value});
        }

        //Calculates the total duration over all the routes on the given truck
        public double TruckDuration(int truckId)
        {
            return Trucks[truckId].Sum(route => route.Duration);
        }
    }
}