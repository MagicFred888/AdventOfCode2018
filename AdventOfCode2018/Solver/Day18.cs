using AdventOfCode2018.Tools;

namespace AdventOfCode2018.Solver
{
    internal partial class Day18 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Settlers of The North Pole";

        private QuickMatrix _field = new();

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            return GetResourceValue(10).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            return GetResourceValue(1000000000).ToString();
        }

        private int GetResourceValue(int nbrOfCycles)
        {
            int fullCalcNbr = 650;
            List<int> repeating = [];
            for (int i = 0; i < Math.Min(nbrOfCycles, fullCalcNbr); i++)
            {
                QuickMatrix newField = new(_field.ColCount, _field.RowCount, ".");
                foreach (CellInfo cell in _field.Cells)
                {
                    List<CellInfo> around = _field.GetNeighbours(cell.Position, TouchingMode.All);
                    newField.Cell(cell.Position).StringVal = cell.StringVal switch
                    {
                        "." => around.Count(c => c.StringVal == "|") >= 3 ? "|" : ".",
                        "|" => around.Count(c => c.StringVal == "#") >= 3 ? "#" : "|",
                        "#" => around.Any(c => c.StringVal == "#") && around.Any(c => c.StringVal == "|") ? "#" : ".",
                        _ => throw new NotImplementedException()
                    };
                }
                _field = newField;
                int energy = _field.Cells.Count(c => c.StringVal == "|") * _field.Cells.Count(c => c.StringVal == "#");
                repeating.Add(energy);
            }

            // Done
            if (repeating.Count == nbrOfCycles)
            {
                return _field.Cells.Count(c => c.StringVal == "|") * _field.Cells.Count(c => c.StringVal == "#");
            }

            // Compute value for the repeating part
            List<int> matches = repeating
                .Select((value, index) => new { value, index })
                .Where(x => x.value == repeating[^1])
                .Select(x => x.index)
                .ToList();
            int repetingStep = matches[^1] - matches[^2];
            int remaining = (nbrOfCycles - fullCalcNbr) % repetingStep;
            return repeating[matches[^2] + remaining];
        }

        private void ExtractData()
        {
            _field = new(_puzzleInput);
        }
    }
}