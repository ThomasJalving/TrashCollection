using System;
using DumpTruck.Datastructures;
using DumpTruck.HelperClasses;
using DumpTruck.InputOutput;
using DumpTruck.NeighbourhoodFunctions;
using DumpTruck.ScoreStatics;

namespace DumpTruck
{
    internal class Program
    {
        public static WeightedRandomBag<INeighbourhoodFunction> NeighbourhoodFunctions;

        private static void Main(string[] args)
        {
            Week.Distances = InputReader.LoadDistanceMatrix();
            (Week.OrderIDs, Week.Orders) = InputReader.LoadOrderFile();
            var solution = InitialSolutionCreator.CreateInitialSolution();
            Console.WriteLine(SolutionChecker.OfficialScore(solution).ToString("F1"));
            GenerateFunctions();
            var (bestScore, officialBestScore, bestWeek) = SimulatedAnnealing(solution);
            for (int i = 0; i < Constants.SIM_ANNEAL_RUNS-1; i++)
            {
                Console.WriteLine("Restarting SIMAL");
                (bestScore, officialBestScore, bestWeek) = SimulatedAnnealing(solution, bestScore, officialBestScore, bestWeek);
            }
            Console.WriteLine(bestScore);
            Console.WriteLine($"Done! Best score found {officialBestScore}");
            OutputParser.WriteStringToFile(bestWeek, officialBestScore);
            Console.ReadLine();                             // Prevent exit when in Release mode
        }

        private static Solution SimulatedAnnealing(Week solution, double bestScore = double.PositiveInfinity, double officialBestScore = 0.0, string solutionString = "")
        {
            var reciprocalTemperature = 1.0 / Constants.INITIAL_TEMPERATURE;
            var bestWeek = solutionString;
            INeighbourhoodFunction neighbourhoodFunction;
            NeighbourDelta neighbourDelta;
            var applyCount = 0;
            var deltaSum = 0d;
            var deltaCount = 0;
            for (var progress = 1; progress <= 100; progress++)
            {
                for (var i = 0; i < Constants.ITER_Q; i++)
                {
                    neighbourhoodFunction = NeighbourhoodFunctions.GetRandom();
                    neighbourDelta = neighbourhoodFunction.SuggestRandomNeighbour(solution);
                    if (neighbourDelta.Delta < 0)//If the delta minimises cost accept
                    {
                        neighbourhoodFunction.ApplyFunction(solution, neighbourDelta);
                        solution.Score += neighbourDelta.Delta;
                        continue;
                    }
                    if(neighbourDelta.Delta == 0)
                    {
                        //Console.WriteLine($"Unlikely 0 delta caused by {neighbourhoodFunction.GetType()}");
                        continue;
                    }
                    if(neighbourDelta.Delta != double.PositiveInfinity)
                    {
                        deltaSum += neighbourDelta.Delta;
                        deltaCount++;
                    }
                    var p = Math.Exp(-neighbourDelta.Delta * reciprocalTemperature);
                    if (p > RandomHelper.Random.NextDouble())//If the delta didn't minimises cost accept with chance p
                    {
                        if (solution.Score < bestScore)//If current score was the best one so far, save it
                        {
                            bestWeek = OutputParser.ParseWeek(solution);
                            bestScore = solution.Score;
                            officialBestScore = SolutionChecker.OfficialScore(solution);
                        }
                        applyCount++;
                        neighbourhoodFunction.ApplyFunction(solution, neighbourDelta);
                        solution.Score += neighbourDelta.Delta;
                        if(applyCount >= Constants.APPLIES_TEMP_DOWN)
                        {
                            applyCount = 0;
                            reciprocalTemperature *= 1 / Constants.ALPHA;
                            Console.WriteLine(
                                $"Temperature is decreasing to {1 / reciprocalTemperature} with reciprocal of {reciprocalTemperature}");
                        }
                    }
                }
                Console.WriteLine($"We are now at {progress}%..");
            }

            if (solution.Score < bestScore)
            {
                bestWeek = OutputParser.ParseWeek(solution);
                bestScore = solution.Score;
                officialBestScore = SolutionChecker.OfficialScore(solution);
            }
            Console.WriteLine($"The average delta was {deltaSum / deltaCount}");
            Console.WriteLine($"Starting temperature should be approximately {-(deltaSum / deltaCount) / Math.Log(0.5)}");
            Console.WriteLine($"Done! Best score found {officialBestScore}");
            OutputParser.WriteStringToFile(bestWeek, officialBestScore);
            return new Solution {OptScore = bestScore, OfficialScore = officialBestScore, SolutionCsv = bestWeek};
        }

        private static void GenerateFunctions()
        {
            NeighbourhoodFunctions = new WeightedRandomBag<INeighbourhoodFunction>();
            NeighbourhoodFunctions.AddEntry(new Add(), 550);
            NeighbourhoodFunctions.AddEntry(new Opt2(), 110);
            NeighbourhoodFunctions.AddEntry(new InRouteNodeSwap(), 450);
            NeighbourhoodFunctions.AddEntry(new Remove(), 150);
            NeighbourhoodFunctions.AddEntry(new Shift(), 200);
            NeighbourhoodFunctions.AddEntry(new AddRoute(), 65);
        }
    }
}