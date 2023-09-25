using System;

namespace DumpTruck.NeighbourhoodFunctions
{
    public class NeighbourDelta : IComparable<NeighbourDelta>
    {
        //The parameters needed to move to the new neighbour
        public object[] Parameters { get; set; }
        //The delta in cost between the current solution and the neigbhour this is related to
        public double Delta { get; set; }

        public int CompareTo(NeighbourDelta? obj)
        {
            return Delta.CompareTo(obj?.Delta);
        }
    }
}