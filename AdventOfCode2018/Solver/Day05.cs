namespace AdventOfCode2018.Solver
{
    internal partial class Day05 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Alchemical Reduction";

        public override string GetSolution1(bool isChallenge)
        {
            return PerformFullReduction(_puzzleInput[0]).Length.ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            string reducedInput = PerformFullReduction(_puzzleInput[0]);
            int minSize = int.MaxValue;
            foreach (char c in Enumerable.Range('a', 26).Select(c => (char)c))
            {
                string reduced = reducedInput.Replace(c.ToString(), "").Replace(char.ToUpper(c).ToString(), "");
                minSize = Math.Min(minSize, PerformFullReduction(reduced).Length);
            }
            return minSize.ToString();
        }

        private static string PerformFullReduction(string stringToReduce)
        {
            List<string> letters = Enumerable.Range('a', 26).Select(c => ((char)c).ToString()).ToList();
            bool reduced;
            do
            {
                int iniSize = stringToReduce.Length;
                stringToReduce = letters.Aggregate(stringToReduce, (current, letter) =>
                    current.Replace(letter + letter.ToUpper(), "").Replace(letter.ToUpper() + letter, ""));
                reduced = stringToReduce.Length != iniSize;
            } while (reduced);
            return stringToReduce;
        }
    }
}