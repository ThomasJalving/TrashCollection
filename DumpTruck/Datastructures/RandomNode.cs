namespace DumpTruck.Datastructures
{
    //The result of using a randomhelper function to get a node from an orderset
    public class RandomNode
    {
        //The randomised node
        public Node Node { get; set; }
        //The index in the orderset
        public int Index { get; set; }
        //Set to true if no node was found
        public bool Failure { get; set; }
    }
}