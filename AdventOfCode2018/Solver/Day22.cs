using AdventOfCode2018.Tools;
using System.Drawing;

namespace AdventOfCode2018.Solver
{
    internal partial class Day22 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Mode Maze";

        private enum Equipment
        {
            Torch = 0,
            ClimbingGear = 1,
            Neither = 2,
        }

        private Point _target = new();
        private int _depth = 0;

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            QuickMatrix cave = ComputeCave(_target, _depth, _target);
            return cave.Cells.Sum(c => c.LongVal % 3).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            QuickMatrix cave = ComputeCave(_target, _depth, new(_target.X + 20, _target.Y + 20)); // 20 arbitrarily chosen, may need more on you puzzle input
            return FindShortestTime(cave, _target).ToString();
        }

        private static int FindShortestTime(QuickMatrix cave, Point target)
        {
            Queue<(Point position, Equipment equipment, int cumulatedTime)> toVisit = new();
            Dictionary<(Point position, Equipment equipment), int> visited = [];
            toVisit.Enqueue((new Point(0, 0), Equipment.Torch, 0));
            int bestTime = int.MaxValue;

            while (toVisit.Count > 0)
            {
                var (currentPosition, currentEquipment, currentTime) = toVisit.Dequeue();

                // Already visited?
                if (visited.TryGetValue((currentPosition, currentEquipment), out int visitedTime) && visitedTime <= currentTime)
                {
                    continue;
                }
                visited[(currentPosition, currentEquipment)] = currentTime;

                // Arrived at target with Torch?
                if (currentPosition == target)
                {
                    bestTime = Math.Min(bestTime, currentTime + (currentEquipment == Equipment.Torch ? 0 : 7));
                    continue;
                }

                // Scan neighbors
                foreach (Point nextPosition in cave.GetNeighbours(currentPosition, TouchingMode.HorizontalAndVertical).ConvertAll(c => c.Position))
                {
                    Equipment nextEquipment = currentEquipment;

                    // Determine if we need to change equipment
                    bool needChangeEquipment = cave.Cell(nextPosition).StringVal switch
                    {
                        "." when currentEquipment == Equipment.Neither => true, // Rocky
                        "=" when currentEquipment == Equipment.Torch => true, // Wet
                        "|" when currentEquipment == Equipment.ClimbingGear => true, // Narrow
                        _ => false
                    };

                    if (needChangeEquipment)
                    {
                        foreach (Equipment newEquipment in Enum.GetValues(typeof(Equipment)))
                        {
                            if (newEquipment != currentEquipment)
                            {
                                toVisit.Enqueue((nextPosition, newEquipment, currentTime + 8));
                            }
                        }
                    }
                    else
                    {
                        toVisit.Enqueue((nextPosition, nextEquipment, currentTime + 1));
                    }
                }
            }

            return bestTime;
        }

        private static QuickMatrix ComputeCave(Point target, int depth, Point caveSize)
        {
            QuickMatrix caveErosionLevel = new(caveSize.X + 1, caveSize.Y + 1, 0);
            for (int y = 0; y < caveErosionLevel.RowCount; y++)
            {
                for (int x = 0; x < caveErosionLevel.ColCount; x++)
                {
                    long geologicIndex = (x, y) switch
                    {
                        (0, 0) => 0,
                        (var xPos, var yPos) when xPos == target.X && yPos == target.Y => 0,
                        (_, 0) => x * 16807,
                        (0, _) => y * 48271,
                        _ => caveErosionLevel.Cell(x - 1, y).LongVal * caveErosionLevel.Cell(x, y - 1).LongVal
                    };
                    long erosionLevel = (geologicIndex + depth) % 20183;
                    caveErosionLevel.Cell(x, y).LongVal = erosionLevel;
                    caveErosionLevel.Cell(x, y).StringVal = (erosionLevel % 3) switch
                    {
                        0 => ".",
                        1 => "=",
                        2 => "|",
                        _ => throw new InvalidDataException("Invalid erosion level"),
                    };
                }
            }
            caveErosionLevel.DebugPrint();
            return caveErosionLevel;
        }

        private void ExtractData()
        {
            _depth = int.Parse(_puzzleInput[0].Split(' ')[1]);
            _target = new Point(int.Parse(_puzzleInput[1].Split(' ')[1].Split(',')[0]), int.Parse(_puzzleInput[1].Split(' ')[1].Split(',')[1]));
        }
    }
}