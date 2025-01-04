using AdventOfCode2018.Tools;

namespace AdventOfCode2018.Solver
{
    internal partial class Day01 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Chronal Calibration";

        public override string GetSolution1(bool isChallenge)
        {
            return QuickList.ListOfInt(_puzzleInput).Sum().ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            List<int> data = QuickList.ListOfInt(_puzzleInput);
            HashSet<int> seen = [];
            int position = 0;
            int sum = 0;
            while (true)
            {
                sum += data[position % data.Count];
                if (!seen.Add(sum))
                {
                    return sum.ToString();
                }
                position++;
            }
        }
    }
}