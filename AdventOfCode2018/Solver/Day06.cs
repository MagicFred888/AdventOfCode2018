using AdventOfCode2018.Extensions;
using AdventOfCode2018.Tools;
using System.Drawing;

namespace AdventOfCode2018.Solver
{
    internal partial class Day06 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Chronal Coordinates";

        private Dictionary<int, Point> _coordinates = [];
        private QuickGrid grid = new();

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();

            // Report closest manhattan distance in each cells
            foreach (CellInfo cell in grid.Cells)
            {
                long minDistance = _coordinates.Values.Min(p => cell.Position.ManhattanDistance(p));
                List<KeyValuePair<int, Point>> closest = _coordinates.Where(kvp => cell.Position.ManhattanDistance(kvp.Value) == minDistance).ToList();
                if (closest.Count != 1)
                {
                    continue;
                }
                cell.LongVal = closest[0].Key;
            }

            // Make a list of all number on edge to remove them from final answer
            List<long> toIgnore = grid.Cells
                .FindAll(c => c.Position.X == grid.MinX || c.Position.X == grid.MaxX || c.Position.Y == grid.MinY || c.Position.Y == grid.MaxY)
                .ConvertAll(c => c.LongVal)
                .Distinct()
                .ToList();

            return grid.Cells.Where(c => !toIgnore.Contains(c.LongVal)).GroupBy(c => c.LongVal).Max(g => g.Count()).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();

            // Compute for each cells the sum of manhattan distance to all coordinates
            foreach (CellInfo cell in grid.Cells)
            {
                cell.LongVal = _coordinates.Values.Sum(p => cell.Position.ManhattanDistance(p));
            }

            return grid.Cells.Count(c => c.LongVal < 10000).ToString();
        }

        private void ExtractData()
        {
            // Extract data
            _coordinates = _puzzleInput.Aggregate(new Dictionary<int, Point>(), (acc, line) =>
            {
                var parts = line.Split(", ");
                acc.Add(acc.Count + 1, new Point(int.Parse(parts[0]), int.Parse(parts[1])));
                return acc;
            });

            // Create an empty grid
            int minX = _coordinates.Values.Min(p => p.X);
            int maxX = _coordinates.Values.Max(p => p.X);
            int minY = _coordinates.Values.Min(p => p.Y);
            int maxY = _coordinates.Values.Max(p => p.Y);
            grid = new QuickGrid(minX, maxX, minY, maxY, 0);
        }
    }
}