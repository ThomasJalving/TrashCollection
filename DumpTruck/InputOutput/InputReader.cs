using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DumpTruck.Datastructures;

namespace DumpTruck.InputOutput
{
    public static class InputReader
    {
        private const string DISTANCE_PATH = "InputOutput/AfstandenMatrix.csv";
        private const string ORDER_PATH = "InputOutput/Orderbestand.csv";

        public static Dictionary<(int From, int To), int> LoadDistanceMatrix()
        {
            var distanceMatrix = new Dictionary<(int From, int To), int>();
            using var sr = File.OpenText(DISTANCE_PATH);
            var line = sr.ReadLine(); // Read header line
            while ((line = sr.ReadLine()) != null)
            {
                var columns = line.Split(';');
                var from = int.Parse(columns[0]);
                var to = int.Parse(columns[1]);
                var drivingTime = int.Parse(columns[3]);
                distanceMatrix.Add((from, to), drivingTime);
            }

            return distanceMatrix;
        }

        public static Tuple<int[], Dictionary<int, Order>> LoadOrderFile()
        {
            var nfi = new NumberFormatInfo {NumberDecimalSeparator = "."};
            using var sr = File.OpenText(ORDER_PATH);
            var line = sr.ReadLine(); // Read header line
            var routeOptions = new Dictionary<int, Order>();
            var orderIDs = new List<int>();
            while ((line = sr.ReadLine()) != null)
            {
                var columns = line.Split(';');
                var order = new Order
                {
                    Id = int.Parse(columns[0]),
                    Pwk = int.Parse(columns[2]),
                    Volume = int.Parse(columns[3]) * int.Parse(columns[4]),
                    TimeToEmpty = double.Parse(columns[5], nfi) * 60d,
                    MatrixId = int.Parse(columns[6]),
                    NodeIndex = new int[5]
                };
                routeOptions.Add(order.Id, order);
                orderIDs.Add(order.Id);
            }

            return new Tuple<int[], Dictionary<int, Order>>(orderIDs.ToArray(), routeOptions);
        }
    }
}