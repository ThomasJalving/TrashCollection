using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumpTruck.NeighbourhoodFunctions
{
    class Shift : INeighbourhoodFunction
    {
        public void ApplyFunction(Week week, NeighbourDelta neighbourDelta)
        {
            Node toShift = (Node) neighbourDelta.Parameters[0];
            Node shiftTarget = (Node) neighbourDelta.Parameters[1];
            bool previous = (bool)neighbourDelta.Parameters[2];
            double shiftTruckDurationDelta = (double)neighbourDelta.Parameters[3];
            double targetTruckDurationDelta = (double)neighbourDelta.Parameters[4];

            Node shiftPrev = toShift.Previous;
            Node shiftNext = toShift.Next;
            Node targetPrev = shiftTarget.Previous;
            Node targetNext = shiftTarget.Next;
            
            if (previous) //in front
            {
                if(targetNext == toShift) //neighbours
                {
                    if(targetPrev != null)
                        targetPrev.Next = toShift;
                    toShift.Previous = targetPrev;

                    shiftTarget.Next = shiftNext;
                    if(shiftNext != null)
                        shiftNext.Previous = shiftTarget;
                } else //not neighbours
                {
                    if(shiftPrev != null)
                        shiftPrev.Next = shiftNext;
                    if(shiftNext != null)
                        shiftNext.Previous = shiftPrev;

                    if(targetPrev != null)
                        targetPrev.Next = toShift;
                    toShift.Previous = targetPrev;
                }

                toShift.Next = shiftTarget;
                shiftTarget.Previous = toShift;

                
            
            } else //behind
            {
                if (targetPrev == toShift)
                {
                    if(shiftPrev != null)
                        shiftPrev.Next = shiftTarget;
                    shiftTarget.Previous = shiftPrev;

                    toShift.Next = targetNext;
                    if(targetNext != null)
                        targetNext.Previous = toShift;
                } else
                {
                    if (shiftPrev != null)
                        shiftPrev.Next = shiftNext;
                    if (shiftNext != null)
                        shiftNext.Previous = shiftPrev;

                    if (targetNext != null)
                        targetNext.Previous = toShift;
                    toShift.Next = targetNext;
                }

                shiftTarget.Next = toShift;
                toShift.Previous = shiftTarget;

                
            }
            //check if toShift was firstnode or lastnode of it's own route, update when needed
            if (toShift.Route.FirstNode == toShift)
                toShift.Route.FirstNode = shiftNext;
            if (toShift.Route.LastNode == toShift)
                toShift.Route.LastNode = shiftPrev;

            if (previous)
            {
                if (shiftTarget.Route.FirstNode == shiftTarget)
                    shiftTarget.Route.FirstNode = toShift;
            } else
            {
                if (shiftTarget.Route.LastNode == shiftTarget)
                    shiftTarget.Route.LastNode = toShift;
            }

            //update route weights and durations
            toShift.Route.CurrentVolume -= toShift.Order.Volume;
            toShift.Route.Duration += shiftTruckDurationDelta;
            shiftTarget.Route.CurrentVolume += toShift.Order.Volume;
            shiftTarget.Route.Duration += targetTruckDurationDelta;

            if(toShift.Route.FirstNode == null || toShift.Route.LastNode == null) //remove toShift.Route if it is now empty
            {
                Console.WriteLine("Remove route");
                toShift.Route.ParentDay.Trucks[toShift.Route.TruckId].Remove(toShift.Route);
            }
            
            toShift.Route = shiftTarget.Route;
        }

        public NeighbourDelta SuggestRandomNeighbour(Week week)
        {
            Day day = RandomHelper.RandomDay(week);
            (RandomNode rndNode1, RandomNode rndNode2) = RandomHelper.RandomPairDoneNodes(day);
            if (rndNode1.Failure)
            {
                return new NeighbourDelta
                {
                    Delta = double.PositiveInfinity
                };
            }
            Node toShift = rndNode1.Node;
            Node shiftTarget = rndNode2.Node;
            if(toShift.Route.LastNode == toShift && toShift.Route.FirstNode == toShift)
            {
                return new NeighbourDelta
                {
                    Delta = double.PositiveInfinity
                };
            }
            bool previous = RandomHelper.Random.NextDouble() < .5; //place the node in front or behind the target node
            double delta = 0;
            double durationDeltaTruckShift = 0;
            double durationDeltaTruckTarget = 0;
            if (previous) //in front
            {
                if(toShift.Next == shiftTarget) //do nothing
                {
                    return new NeighbourDelta
                    {
                        Delta = double.PositiveInfinity
                    };
                }

                if(shiftTarget.Next == toShift) //if they are neighbours the logic is different
                {
                    durationDeltaTruckShift -= OrderHelper.NodeDriveDuration(shiftTarget.Previous, shiftTarget);
                    durationDeltaTruckShift -= OrderHelper.NodeDriveDuration(shiftTarget, toShift);
                    durationDeltaTruckShift -= OrderHelper.NodeDriveDuration(toShift, toShift.Next);

                    durationDeltaTruckShift += OrderHelper.NodeDriveDuration(shiftTarget.Previous, toShift);
                    durationDeltaTruckShift += OrderHelper.NodeDriveDuration(toShift, shiftTarget);
                    durationDeltaTruckShift += OrderHelper.NodeDriveDuration(shiftTarget, toShift.Next);
                } else
                {
                    durationDeltaTruckShift -= OrderHelper.NodeDriveDuration(toShift.Previous, toShift);
                    durationDeltaTruckShift -= OrderHelper.NodeDriveDuration(toShift, toShift.Next);
                    durationDeltaTruckTarget -= OrderHelper.NodeDriveDuration(shiftTarget.Previous, shiftTarget);

                    durationDeltaTruckShift += OrderHelper.NodeDriveDuration(toShift.Previous, toShift.Next);
                    durationDeltaTruckTarget += OrderHelper.NodeDriveDuration(shiftTarget.Previous, toShift);
                    durationDeltaTruckTarget += OrderHelper.NodeDriveDuration(toShift, shiftTarget);
                }
            } else //behind
            {
                if (shiftTarget.Next == toShift) //do nothing
                {
                    return new NeighbourDelta
                    {
                        Delta = double.PositiveInfinity
                    };
                }

                if (toShift.Next == shiftTarget) //if they are neighbours the logic is different
                {
                    durationDeltaTruckShift -= OrderHelper.NodeDriveDuration(toShift.Previous, toShift);
                    durationDeltaTruckShift -= OrderHelper.NodeDriveDuration(toShift, shiftTarget);
                    durationDeltaTruckShift -= OrderHelper.NodeDriveDuration(shiftTarget, shiftTarget.Next);

                    durationDeltaTruckShift += OrderHelper.NodeDriveDuration(toShift.Previous, shiftTarget);
                    durationDeltaTruckShift += OrderHelper.NodeDriveDuration(shiftTarget, toShift);
                    durationDeltaTruckShift += OrderHelper.NodeDriveDuration(toShift, shiftTarget.Next);
                }
                else
                {
                    durationDeltaTruckShift -= OrderHelper.NodeDriveDuration(toShift.Previous, toShift);
                    durationDeltaTruckShift -= OrderHelper.NodeDriveDuration(toShift, toShift.Next);
                    durationDeltaTruckTarget -= OrderHelper.NodeDriveDuration(shiftTarget, shiftTarget.Next);

                    durationDeltaTruckShift += OrderHelper.NodeDriveDuration(toShift.Previous, toShift.Next);
                    durationDeltaTruckTarget += OrderHelper.NodeDriveDuration(shiftTarget, toShift);
                    durationDeltaTruckTarget += OrderHelper.NodeDriveDuration(toShift, shiftTarget.Next);
                }
            }
            if (toShift.Route != shiftTarget.Route)
            {
                delta += PenaltyHelper.WeightPenaltyDelta(toShift.Route.CurrentVolume, -toShift.Order.Volume); //remove volume from current route
                delta += PenaltyHelper.WeightPenaltyDelta(shiftTarget.Route.CurrentVolume, toShift.Order.Volume); //add volume to new route
                durationDeltaTruckShift -= toShift.Order.TimeToEmpty;
                durationDeltaTruckTarget += toShift.Order.TimeToEmpty;
            }
            
            delta += durationDeltaTruckShift;
            delta += durationDeltaTruckTarget;
            if(toShift.Route.TruckId == shiftTarget.Route.TruckId)
            {
                delta += PenaltyHelper.DurationPenaltyDelta(toShift.Route.ParentDay, toShift.Route.TruckId, durationDeltaTruckShift + durationDeltaTruckTarget);
            } else
            {
                delta += PenaltyHelper.DurationPenaltyDelta(toShift.Route.ParentDay, toShift.Route.TruckId, durationDeltaTruckShift);
                delta += PenaltyHelper.DurationPenaltyDelta(shiftTarget.Route.ParentDay, shiftTarget.Route.TruckId, durationDeltaTruckTarget);
            }
            
            return new NeighbourDelta
            {
                Delta = delta,
                Parameters = new object[] { toShift, shiftTarget, previous, durationDeltaTruckShift, durationDeltaTruckTarget }
            };
        }
    }
}
