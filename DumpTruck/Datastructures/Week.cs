using System;
using System.Collections.Generic;

namespace DumpTruck.Datastructures
{
    public class Week
    {
        //The distance matrix
        public static Dictionary<(int From, int To), int> Distances;

        //All orders availabe from ther Id
        public static Dictionary<int, Order> Orders;

        //All order Ids
        public static int[] OrderIDs;

        //All orders that are done in the current solution
        public OrderSet OrdersDoneThisWeek = new OrderSet();

        //All orders that are not done in the current solution
        public OrderSet OrdersNotDoneThisWeek = new OrderSet();

        //All days for this solution
        public Day[] Days;
        
        //The current unofficial score of this solution based on the starting score and all deltas
        public double Score { get; set; }

        //The week is initialized with a given array of days and makes itself their parents
        //It also adds all orders to the set of orders that are not done yet
        public Week(Day[] days)
        {
            Days = days;
            foreach (var id in OrderIDs)
                OrdersNotDoneThisWeek.AddByOrderId(id);
            foreach (Day day in Days)
            {
                day.parentWeek = this;
            }
        }

        public int Routes()
        {
            int c = 0;
            foreach(Day d in Days)
            {
                c += d.Trucks[0].Count;
                c += d.Trucks[1].Count;
            }
            return c;
        }

        //Removes the order from the set of not done orders and adds it to the set of done orders
        //Also adds it to the set of done nodes for the days that are part of the given pattern
        public void AddCompleteOrder(RandomOrder order, int[] pattern)
        {
            OrdersDoneThisWeek.AddByOrderId(order.OrderId);
            OrdersNotDoneThisWeek.RemoveAtIndex(order.Index);
            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] == 1)
                {
                    Orders[order.OrderId].NodeIndex[i] = Days[i].OrdersDoneThisDay.AddByOrderId(order.OrderId);
                }
            }

            Orders[order.OrderId].Pattern = pattern;
        }

        //Adds the order to the set of not done orders and removes it from the set of done orders
        //Also removes it from the set of done nodes for the days that are part of the pattern found in the order
        public void RemoveCompleteOrder(RandomOrder order)
        {
            OrdersDoneThisWeek.RemoveAtIndex(order.Index);
            OrdersNotDoneThisWeek.AddByOrderId(order.OrderId);
            var pattern = Orders[order.OrderId].Pattern;
            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] == 1)
                {
                    //Removes an id from the orderset but also updates the changed indices in the orderset for other orders
                    Days[i].OrdersDoneThisDay.RemoveAtIndex(Orders[order.OrderId].NodeIndex[i]);
                    var movedOrderId = Days[i].OrdersDoneThisDay[Orders[order.OrderId].NodeIndex[i]];
                    if (movedOrderId != 0)
                        Orders[movedOrderId].NodeIndex[i] = Orders[order.OrderId].NodeIndex[i];
                }
            }
        }
    }
}