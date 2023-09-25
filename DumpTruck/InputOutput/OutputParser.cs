using System.Text;
using System.IO;
using DumpTruck.Datastructures;
using System;

namespace DumpTruck.InputOutput
{
    internal static class OutputParser
    {
        public static string ParseWeek(Week week)
        {
            var sb = new StringBuilder();
            for (var di = 1; di < 6; di++) //loop through days
            for (var ti = 1; ti < 3; ti++) //loop through trucks
            {
                var numberOfLocations = 1;
                foreach (var route in week.Days[di - 1].Trucks[ti - 1]) //loop through routes
                {
                    var current = route.FirstNode;
                    while (current.Next != null)
                    {
                        sb.AppendLine($"{ti};{di};{numberOfLocations++};{current.Order.Id}");
                        current = current.Next;
                    }

                    sb.AppendLine($"{ti};{di};{numberOfLocations++};{current.Order.Id}"); // Add last order)
                    sb.AppendLine($"{ti};{di};{numberOfLocations++};0"); //add dump location in the end
                }
            }

            return sb.ToString();
        }

        public static void WriteStringToFile(string output, double score)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.WriteAllText( $"../../../../{date}-score-{score:N0}.txt", output);
        }
    }
}