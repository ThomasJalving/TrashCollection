namespace DumpTruck.Datastructures
{
    //An order as defined by all necceary data
    public class Order
    {
        public int Id { get; set; }
        public int Pwk { get; set; }
        public int Volume { get; set; }
        public double TimeToEmpty { get; set; }
        public int MatrixId { get; set; }
        public int[] Pattern { get; set; }
        public int[] NodeIndex { get; set; }
    }
}