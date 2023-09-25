namespace DumpTruck.Datastructures
{
    //The result of using a randomhelper function to get an orderId from an orderset
    public class RandomOrder
    {
        //The randomised orderid
        public int OrderId { get; set; }
        //The index in the orderset
        public int Index { get; set; }
        //Set to true if no order was found
        public bool Failure { get; set; }
    }
}