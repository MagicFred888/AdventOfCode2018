using AdventOfCode2018.Tools;
using System.Drawing;

namespace AdventOfCode2018.Solver
{
    internal partial class Day11 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Chronal Charge";

        private QuickMatrix _grid = new();

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            return SearchMax(3, 3);
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            return SearchMax(3, 15);
        }

        private string SearchMax(int startSize, int endSize)
        {
            // Search max power
            long maxPower = long.MinValue;
            Point maxAreaPosition = new(0, 0);
            int maxAreaSize = 0;
            for (int size = startSize; size <= endSize; size++)
            {
                for (int i = 1; i <= _grid.ColCount - size; i++)
                {
                    for (int j = 1; j <= _grid.RowCount - size; j++)
                    {
                        long power = _grid.GetCellsInRange(new(i, j), new(i + size - 1, j + size - 1)).Sum(c => c.LongVal);
                        if (power > maxPower)
                        {
                            maxPower = power;
                            maxAreaPosition = new(i, j);
                            maxAreaSize = size;
                        }
                    }
                }
            }
            return startSize == endSize ? $"{maxAreaPosition.X + 1},{maxAreaPosition.Y + 1}" : $"{maxAreaPosition.X + 1},{maxAreaPosition.Y + 1},{maxAreaSize}";
        }

        private void ExtractData()
        {
            // Compute grid
            int gridSerialNumber = int.Parse(_puzzleInput[0]);
            _grid = new QuickMatrix(300, 300);
            foreach (CellInfo cell in _grid.Cells)
            {
                // Compute each cell power level
                int rackId = (cell.Position.X + 1) + 10;
                int powerLevel = rackId * (cell.Position.Y + 1);
                powerLevel += gridSerialNumber;
                powerLevel *= rackId;
                powerLevel = (powerLevel / 100) % 10;
                powerLevel -= 5;
                cell.LongVal = powerLevel;
            }
        }
    }
}