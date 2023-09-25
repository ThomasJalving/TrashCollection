using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;

namespace DumpTruck.NeighbourhoodFunctions
{
    public class Remove : INeighbourhoodFunction
    {
        //Suggests a random done order to be removed and calculates the cost delta
        public NeighbourDelta SuggestRandomNeighbour(Week week)
        {
            var rndOrder = RandomHelper.RandomDoneOrder(week);
            if (rndOrder.Failure)
            {
                return new NeighbourDelta
                {
                    Delta = double.PositiveInfinity
                };
            }
            int[] pattern = Week.Orders[rndOrder.OrderId].Pattern;
            double delta = 0;
            double[] durationDeltas = new double[5];
            for (int i = 0; i < 5; i++)
            {
                if (pattern[i] == 1)
                {
                    var removeNode = week.Days[i].NodeContainer[rndOrder.OrderId];
                    var route = removeNode.Route;
                    var durationDelta = DurationDelta(removeNode);
                    durationDeltas[i] = durationDelta;
                    delta += durationDelta; //Takes into account if this is the last node
                    delta += PenaltyHelper.DurationPenaltyDelta(route.ParentDay, route.TruckId, durationDelta); // add penalty delta for duration !!!should use truck duration
                    delta += PenaltyHelper.WeightPenaltyDelta(removeNode.Route.CurrentVolume,
                        -removeNode.Order.Volume); // add penalty delta for weight
                }
            }
            var order = Week.Orders[rndOrder.OrderId];
            delta += PenaltyHelper.SatisfactionPenalty(order.Pwk, order);

            return new NeighbourDelta
            {
                Delta = delta,
                Parameters = new object[] {rndOrder, durationDeltas}
            };
        }

        //Removes the given order and if a node for this order is the last node of a route the route, too
        //Otherwise changes duration and volume of the routes a node is removed from
        public void ApplyFunction(Week week, NeighbourDelta neighbourDelta)
        {
            var rndOrder = (RandomOrder) neighbourDelta.Parameters[0];
            var durDeltas = (double[]) neighbourDelta.Parameters[1];
            var pattern = Week.Orders[rndOrder.OrderId].Pattern;
            for (int i = 0; i < 5; i++)
            {
                if (pattern[i] == 1)
                {
                    var removeNode = week.Days[i].NodeContainer[rndOrder.OrderId]; ;
                    var route = removeNode.Route;
                    if (removeNode == route.FirstNode && removeNode == route.LastNode
                    ) //if this is the lastnode, remove the route
                    {
                        route.ParentDay.Trucks[route.TruckId].Remove(route);
                    }
                    else
                    {
                        if (removeNode == route.FirstNode)
                        {
                            route.FirstNode = removeNode.Next;
                            removeNode.Next.Previous = null;
                        }
                        else
                        {
                            removeNode.Previous.Next = removeNode.Next;
                        }

                        if (removeNode == route.LastNode)
                        {
                            route.LastNode = removeNode.Previous;
                            removeNode.Previous.Next = null;
                        }
                        else
                        {
                            removeNode.Next.Previous = removeNode.Previous;
                        }


                        route.CurrentVolume -= removeNode.Order.Volume;
                        route.Duration += durDeltas[i];
                    }
                    removeNode.Next = null;
                    removeNode.Previous = null;
                    removeNode.Route = null;
                }
            }
            week.RemoveCompleteOrder(rndOrder);
        }

        //Delta in time if removing removeNode
        public static double DurationDelta(Node removeNode)
        {
            var delta = 0d;
            var route = removeNode.Route;
            if (removeNode == route.FirstNode && removeNode == route.LastNode) //if this is the last node in the route, remove the route
            {
                delta -= route.Duration;
                if(removeNode.Route.ParentDay.parentWeek.Routes() <= 14) //14, don't remove any routes if the total amount of routes is less/equal than this
                {
                    delta = double.PositiveInfinity;
                }
                //delta = double.PositiveInfinity;
            }
            else
            {
                delta = - OrderHelper.NodeDriveDuration(removeNode.Previous,
                            removeNode)
                        - OrderHelper.NodeDriveDuration(removeNode, removeNode.Next)
                        + OrderHelper.NodeDriveDuration(removeNode.Previous, removeNode.Next)
                        - removeNode.Order.TimeToEmpty;
            }

            return delta;
        }
    }
}