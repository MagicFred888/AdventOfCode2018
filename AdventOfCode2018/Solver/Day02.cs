namespace AdventOfCode2018.Solver
{
    internal partial class Day02 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Inventory Management System";

        public override string GetSolution1(bool isChallenge)
        {
            long nbrTwice = 0;
            long nbrThrice = 0;
            foreach (string line in _puzzleInput)
            {
                (long twice, long thrice) = CountLetters(line);
                nbrTwice += twice;
                nbrThrice += thrice;
            }
            return (nbrTwice * nbrThrice).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            foreach (string line in _puzzleInput)
            {
                string? match = _puzzleInput.FirstOrDefault(otherLine => otherLine != line && CompareLines(line, otherLine));
                if (match != null)
                {
                    return new string(line.Where((c, i) => c == match[i]).ToArray());
                }
            }
            throw new InvalidDataException();
        }

        private static bool CompareLines(string line, string l)
        {
            int diff = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] != l[i])
                {
                    diff++;
                }
                if (diff > 1)
                {
                    return false;
                }
            }
            return diff == 1;
        }

        private static (long twice, long thrice) CountLetters(string line)
        {
            bool isTwice = false;
            bool isThrice = false;
            foreach (long count in line.GroupBy(static c => c).Select(g => g.Count()))
            {
                isTwice = isTwice || count == 2;
                isThrice = isThrice || count == 3;
            }
            return (isTwice ? 1 : 0, isThrice ? 1 : 0);
        }
    }
}