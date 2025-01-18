using AdventOfCode2018.Extensions;
using AdventOfCode2018.Tools;
using System.Drawing;

namespace AdventOfCode2018.Solver
{
    internal partial class Day17 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Reservoir Research";

        private QuickGrid _cave = new();

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            SolveWaterDrop();
            return _cave.Cells.Count(c => c.StringVal is "~" or "|").ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            SolveWaterDrop();
            return _cave.Cells.Count(c => c.StringVal is "~").ToString();
        }

        private void SolveWaterDrop()
        {
            List<Point> sources = [new(500, _cave.MinY)];
            _cave.Cell(sources[0]).StringVal = "|";
            while (sources.Count != 0)
            {
                Point source = sources[0];
                sources.RemoveAt(0);
                sources.AddRange(FillFromSourceAndAddExtraSources(source));
            }
        }

        private List<Point> FillFromSourceAndAddExtraSources(Point dropPos)
        {
            // Initialization
            bool firstLoop = true;
            List<Point> newSources = [];
            Point downDirection = new(0, 1);
            string stopChar;
            do
            {
                // Will make us move out of map?
                if (!_cave.Cell(dropPos.Add(downDirection)).IsValid)
                {
                    return newSources;
                }

                // Touching something ?
                stopChar = _cave.Cell(dropPos.Add(downDirection)).StringVal;
                if (firstLoop && stopChar == "#")
                {
                    return newSources;
                }
                if (stopChar != ".")
                {
                    break;
                }

                // Set drop and move down
                firstLoop = false;
                dropPos = dropPos.Add(downDirection);
                _cave.Cell(dropPos).StringVal = "|";
            } while (true);

            // In water ?
            if (stopChar == "|")
            {
                return newSources;
            }

            // Check if in a "pot" or on a surface
            do
            {
                // Scan
                (Point leftEnd, bool leftIsSource) = HoriScan(dropPos, true);
                (Point rightEnd, bool rightIsSource) = HoriScan(dropPos, false);

                // Fill the row
                string waterType = leftIsSource || rightIsSource ? "|" : "~";
                for (int x = leftEnd.X; x <= rightEnd.X; x++)
                {
                    _cave.Cell(new(x, dropPos.Y)).StringVal = waterType;
                }

                // Done?
                if (waterType == "|")
                {
                    if (leftIsSource)
                    {
                        newSources.Add(leftEnd);
                    }
                    if (rightIsSource)
                    {
                        newSources.Add(rightEnd);
                    }
                    return newSources;
                }

                // Move up
                dropPos = dropPos.Subtract(downDirection);
            } while (true);
        }

        private (Point leftEnd, bool leftIsSource) HoriScan(Point startPos, bool scanLeft)
        {
            Point moveDir = scanLeft ? new(-1, 0) : new(1, 0);
            while (_cave.Cell(startPos.Add(moveDir)).StringVal != "#" && _cave.Cell(startPos.Add(moveDir)).IsValid)
            {
                startPos = startPos.Add(moveDir);
                if (_cave.Cell(startPos.Add(new(0, 1))).StringVal is "." or "|")
                {
                    return (startPos, true);
                }
            }
            if (!_cave.Cell(startPos.Add(moveDir)).IsValid)
            {
                return (startPos, true);
            }
            return (startPos, false);
        }

        private void ExtractData()
        {
            // Extract points
            List<Point> allPoints = [];
            foreach (string line in _puzzleInput)
            {
                string[] values = line.Replace("=", ",").Replace("..", ",").Split(",");
                for (int i = int.Parse(values[3]); i <= int.Parse(values[4]); i++)
                {
                    allPoints.Add(new Point(values[0] == "x" ? int.Parse(values[1]) : i, values[0] == "x" ? i : int.Parse(values[1])));
                }
            }

            // Create cave map from points
            _cave = new QuickGrid(allPoints.Min(p => p.X) - 1, allPoints.Max(p => p.X) + 1, allPoints.Min(p => p.Y), allPoints.Max(p => p.Y), ".");
            allPoints.ForEach(p => _cave.Cell(p).StringVal = "#");
        }
    }
}