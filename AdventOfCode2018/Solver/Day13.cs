using AdventOfCode2018.Extensions;
using AdventOfCode2018.Tools;
using System.Drawing;

namespace AdventOfCode2018.Solver
{
    internal partial class Day13 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Mine Cart Madness";

        private QuickMatrix _map = new();
        private List<(Point position, Point direction, int turnId)> _allCart = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            Point firstCrashLocation = ComputeMove(true);
            return $"{firstCrashLocation.X},{firstCrashLocation.Y}";
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            Point firstCrashLocation = ComputeMove(false);
            return $"{firstCrashLocation.X},{firstCrashLocation.Y}";
        }

        private Point ComputeMove(bool stopOnFirstCrash)
        {
            do
            {
                // Sort cat to have them from top to bottom and if same line, from left to right
                _allCart = [.. _allCart.OrderBy(c => c.position.Y).ThenBy(c => c.position.X)];

                // Move all carts
                for (int i = 0; i < _allCart.Count; i++)
                {
                    (Point position, Point direction, int turnId) = _allCart[i];

                    // Move cart
                    position = position.Add(direction);
                    string trackChar = _map.Cell(position).StringVal;
                    if (trackChar == "+")
                    {
                        direction = turnId switch
                        {
                            // Clockwise and Counterclockwise are inverted because Y axis is pointing down
                            0 => direction.RotateClockwise(),
                            1 => direction,
                            2 => direction.RotateCounterclockwise(),
                            _ => throw new NotImplementedException(),
                        };
                        turnId = (turnId + 1) % 3;
                    }
                    else if (trackChar is "/" or "\\")
                    {
                        direction = (direction, trackChar) switch
                        {
                            // Clockwise and Counterclockwise are inverted because Y axis is pointing down
                            (var d, "\\") when d.X != 0 => d.RotateCounterclockwise(),
                            (var d, "/") when d.X != 0 => d.RotateClockwise(),
                            (var d, "\\") when d.Y != 0 => d.RotateClockwise(),
                            (var d, "/") when d.Y != 0 => d.RotateCounterclockwise(),
                            _ => direction
                        };
                    }

                    // Check for collision
                    if (_allCart.Any(c => c.position == position))
                    {
                        if (stopOnFirstCrash)
                        {
                            return position;
                        }

                        // Flag cart as no more valid
                        int index = _allCart.FindIndex(c => c.position == position);
                        _allCart[index] = (new(-1, -1), new(), turnId);
                        _allCart[i] = (new(-1, -1), new(), turnId);
                    }
                    else
                    {
                        // Save changed cart
                        _allCart[i] = (position, direction, turnId);
                    }
                }

                // Check if we have just 1 cart remaining
                if (_allCart.Count(c => !(c.direction.X == 0 && c.direction.Y == 0)) == 1)
                {
                    return _allCart.Find(c => !(c.direction.X == 0 && c.direction.Y == 0)).position;
                }
            } while (true);
        }

        private void ExtractData()
        {
            _allCart.Clear();
            _map = new QuickMatrix(_puzzleInput);
            foreach (CellInfo cell in _map.Cells.FindAll(c => c.StringVal is "<" or ">" or "^" or "v"))
            {
                Point direction = cell.StringVal switch
                {
                    "<" => new Point(-1, 0),
                    ">" => new Point(1, 0),
                    "^" => new Point(0, -1),
                    "v" => new Point(0, 1),
                    _ => throw new InvalidDataException()
                };
                cell.StringVal = direction.X == 0 ? "|" : "-";
                _allCart.Add((cell.Position, direction, 0));
            }
        }
    }
}