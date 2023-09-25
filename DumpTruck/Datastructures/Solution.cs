namespace DumpTruck.Datastructures
{
    public class Solution
    {
        // Score used by our program, can differ form the Official Score
        public double OptScore { get; set; }
        // The official Score as used by the score checker
        public double OfficialScore { get; set; }
        // A csv-formatted string containing the solution
        public string SolutionCsv { get; set; }
        
        public void Deconstruct(out double optScore, out double officialScore, out string weekCsv)
        {
            optScore = OptScore;
            officialScore = OfficialScore;
            weekCsv = SolutionCsv;
        }
    }
}