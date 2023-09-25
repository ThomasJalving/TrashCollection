using DumpTruck.Datastructures;

namespace DumpTruck.NeighbourhoodFunctions
{
    public interface INeighbourhoodFunction
    {
        //The function that will suggest a random neighbour and give it's delta and neccesary elements to apply it
        public NeighbourDelta SuggestRandomNeighbour(Week week);
        //The function that will move the current solution to the suggested neighbour based on the neigbhourdelta
        public void ApplyFunction(Week week, NeighbourDelta neighbourDelta);
    }
}