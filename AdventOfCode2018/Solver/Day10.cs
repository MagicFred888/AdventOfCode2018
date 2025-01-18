using AdventOfCode2018.Extensions;
using AdventOfCode2018.Tools;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode2018.Solver
{
    internal partial class Day10 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "The Stars Align";

        private List<(Point position, Point speed)> _points = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            Rectangle smallestBoundingBox = SolveChallenge().smallestBoundingBox;

            // Generate answer in console
            QuickGrid grid = new(smallestBoundingBox.Left, smallestBoundingBox.Right, smallestBoundingBox.Top, smallestBoundingBox.Bottom, " ");
            foreach ((Point position, _) in _points)
            {
                grid.Cell(position.X, position.Y).StringVal = "#";
            }
            return string.Join(Environment.NewLine + "   ", grid.GetDebugPrintLines(CellInfoContentType.String));
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            return SolveChallenge().nbrOfStep.ToString();
        }

        private (int nbrOfStep, Rectangle smallestBoundingBox) SolveChallenge()
        {
            Rectangle smallestBoundingBox = GetBoundingBox(_points);
            for (int i = 0; i < int.MaxValue; i++)
            {
                _points = _points.ConvertAll(p => (p.position.Add(p.speed), p.speed));
                Rectangle boundingBox = GetBoundingBox(_points);
                if (boundingBox.Width < smallestBoundingBox.Width && boundingBox.Height < smallestBoundingBox.Height)
                {
                    smallestBoundingBox = boundingBox;
                }
                else
                {
                    _points = _points.ConvertAll(p => (p.position.Subtract(p.speed), p.speed));
                    return (i, smallestBoundingBox);
                }
            }
            throw new InvalidDataException();
        }

        private static Rectangle GetBoundingBox(List<(Point position, Point speed)> points)
        {
            int minX = points.Min(p => p.position.X);
            int maxX = points.Max(p => p.position.X);
            int minY = points.Min(p => p.position.Y);
            int maxY = points.Max(p => p.position.Y);
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        private void ExtractData()
        {
            _points.Clear();
            foreach (string line in _puzzleInput)
            {
                Match match = startInfoExtractionRegex().Match(line);
                if (match.Success)
                {
                    _points.Add((new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)),
                                 new Point(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value))));
                }
            }
        }

        [GeneratedRegex(@"^position=<\s?(-?\d+),\s+(-?\d+)> velocity=<\s?(-?\d+),\s+(-?\d+)>$")]
        private static partial Regex startInfoExtractionRegex();
    }
}