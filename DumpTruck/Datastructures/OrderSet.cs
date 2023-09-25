using System.Collections.Generic;
using DumpTruck.HelperClasses;

namespace DumpTruck.Datastructures
{
    //A set of orderIds that can be added to and removed from in O(1) and that can also do O(1) for getting a random element
    public class OrderSet
    {
        private int[] _orders = new int[Constants.MAX_ORDER_COUNT];
        private HashSet<int> _orderSet = new HashSet<int>();
        //The current amount of elements in the OrderSet, 
        //and also the index of the last element in the array + 1
        public int Count { get; private set; }

        //Adds an element to the hashset and add the end of the array and returns it's current index in the array
        public int AddByOrderId(int orderId)
        {
            _orders[Count++] = orderId;
            _orderSet.Add(orderId);
            return Count - 1;
        }
        
        //Removes the orderId that is at the given index from the hashset and then overwrites it with the last array element
        public void RemoveAtIndex(int index)
        {
            var orderId = _orders[index];
            _orders[index] = _orders[Count - 1];
            _orders[Count - 1] = 0;
            _orderSet.Remove(orderId);
            Count--;
        }

        public bool Contains(int orderId) => _orderSet.Contains(orderId);
        
        public int this[int index] => _orders[index];
    }
}