using AdventOfCode2018.Tools;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode2018.Solver
{
    internal partial class Day03 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "No Matter How You Slice It";

        private readonly List<(int id, Point start, Point end)> _allClaims = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            QuickMatrix fabric = new(1000, 1000);
            foreach ((_, Point start, Point end) in _allClaims)
            {
                fabric.GetCellsInRange(start, end).ForEach(cell => cell.LongVal++);
            }
            return fabric.Cells.Count(cell => cell.LongVal > 1).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            QuickMatrix fabric = new(1000, 1000);
            foreach ((_, Point start, Point end) in _allClaims)
            {
                fabric.GetCellsInRange(start, end).ForEach(cell => cell.LongVal++);
            }
            return _allClaims.Find(i => fabric.GetCellsInRange(i.start, i.end).Max(c => c.LongVal) == 1).id.ToString();
        }

        private void ExtractData()
        {
            _allClaims.Clear();
            foreach (string line in _puzzleInput)
            {
                Match match = ExtractClaimInfo().Match(line);
                if (match.Success)
                {
                    int id = int.Parse(match.Groups["id"].Value);
                    int left = int.Parse(match.Groups["left"].Value);
                    int top = int.Parse(match.Groups["top"].Value);
                    int width = int.Parse(match.Groups["width"].Value);
                    int height = int.Parse(match.Groups["height"].Value);
                    _allClaims.Add((id, new Point(left, top), new Point(left + width - 1, top + height - 1)));
                }
            }
        }

        [GeneratedRegex(@"^#(?<id>\d+) @ (?<left>\d+),(?<top>\d+): (?<width>\d+)x(?<height>\d+)$")]
        private static partial Regex ExtractClaimInfo();
    }
}