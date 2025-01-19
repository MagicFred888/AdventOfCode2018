using AdventOfCode2018.Extensions;
using AdventOfCode2018.Tools;
using System.Drawing;
using static AdventOfCode2018.Tools.QuickMaze;

namespace AdventOfCode2018.Solver
{
    internal partial class Day20 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "A Regular Map";

        private readonly HashSet<Point> _allCells = [];

        public override string GetSolution1(bool isChallenge)
        {
            QuickMatrix maze = GetMazeFromRegex(_puzzleInput[0]);

            // Most far room
            long max = maze.Cells.Max(c => ((MazeCellInfos)c.ObjectVal!).DistanceToStart);
            return (max / 2).ToString(); // Because we have 2 moves per cell due to doors
        }

        public override string GetSolution2(bool isChallenge)
        {
            QuickMatrix maze = GetMazeFromRegex(_puzzleInput[0]);

            // Most far room (% 2 to not count doors)
            long nbrRoom = maze.Cells.Count(c => ((MazeCellInfos)c.ObjectVal!).DistanceToStart >= 2000 && ((MazeCellInfos)c.ObjectVal).DistanceToStart % 2 == 0);
            return nbrRoom.ToString();
        }

        private QuickMatrix GetMazeFromRegex(string input)
        {
            _allCells.Clear();
            Point currentPosition = new();
            List<string> groups = SplitByGroup(input);
            foreach (string group in groups)
            {
                if (!group.Contains('('))
                {
                    currentPosition = PerformMove(currentPosition, group);
                }
                else
                {
                    currentPosition = ComputeRoomsPosition(currentPosition, group)[0];
                }
            }

            // Create the maze
            int xMin = _allCells.Min(v => v.X);
            int xMax = _allCells.Max(v => v.X);
            int yMin = _allCells.Min(v => v.Y);
            int yMax = _allCells.Max(v => v.Y);
            QuickMatrix maze = new(xMax - xMin + 3, yMax - yMin + 3, "#");
            foreach (Point cell in _allCells)
            {
                maze.Cell(cell.Subtract(new(xMin - 1, yMin - 1))).StringVal = ".";
            }

            // Solve it
            QuickMaze.SolveMaze(maze, new Point().Subtract(new(xMin - 1, yMin - 1)), new Point(), "#");
            return maze;
        }

        private List<Point> ComputeRoomsPosition(Point startPosition, string masterGroup)
        {
            bool isGroup = false;
            List<string> groups = SplitByGroup(masterGroup);
            if (groups.Count == 1 && groups[0].StartsWith('('))
            {
                isGroup = true;
                groups = SplitGroupByOr(groups[0]);
            }

            List<Point> currentStart = [startPosition];
            foreach (string group in groups)
            {
                List<Point> nextStart = [];
                foreach (Point currentPosition in currentStart)
                {
                    if (string.IsNullOrEmpty(group))
                    {
                        nextStart.Add(currentPosition);
                        continue;
                    }
                    if (!group.Contains('('))
                    {
                        nextStart.Add(PerformMove(currentPosition, group));
                    }
                    else
                    {
                        nextStart.AddRange(ComputeRoomsPosition(currentPosition, group));
                    }
                }
                if (!isGroup)
                {
                    currentStart = nextStart;
                }
            }
            return currentStart;
        }

        private Point PerformMove(Point position, string group)
        {
            for (int i = 0; i < group.Length; i++)
            {
                Point moveDir = group[i] switch
                {
                    'N' => new(0, -1),
                    'E' => new(1, 0),
                    'S' => new(0, 1),
                    'W' => new(-1, 0),
                    _ => throw new InvalidDataException("Invalid move")
                };
                for (int j = 0; j < 2; j++)
                {
                    position = position.Add(moveDir);
                    if (!_allCells.Contains(new(position.X, position.Y)))
                    {
                        _allCells.Add(new(position.X, position.Y));
                    }
                }
            }
            return position;
        }

        private static List<string> SplitGroupByOr(string regex)
        {
            regex = regex[1..^1];
            List<string> groups = [];
            int lastGroupStart = 0;
            int depth = 0;
            for (int i = 0; i < regex.Length; i++)
            {
                if (regex[i] == '(')
                {
                    depth++;
                }
                else if (regex[i] == ')')
                {
                    depth--;
                }
                else if (regex[i] == '|' && depth == 0)
                {
                    groups.Add(regex[lastGroupStart..i]);
                    lastGroupStart = i + 1;
                }
            }
            groups.Add(regex[lastGroupStart..]);
            return groups;
        }

        private static List<string> SplitByGroup(string regex)
        {
            regex = regex.Trim('^', '$');
            List<string> groups = [];
            int lastGroupStart = 0;
            int i = 0;
            while (i < regex.Length)
            {
                if (regex[i] == '(')
                {
                    if (i - lastGroupStart > 0)
                    {
                        groups.Add(regex[lastGroupStart..i]);
                    }
                    int depth = 1;
                    int j = i + 1;
                    while (depth > 0)
                    {
                        if (regex[j] == '(')
                        {
                            depth++;
                        }
                        else if (regex[j] == ')')
                        {
                            depth--;
                        }
                        j++;
                    }
                    string group = regex[i..j];
                    groups.Add(group);
                    lastGroupStart = j;
                    i = j - 1;
                }
                i++;
            }
            if (lastGroupStart < regex.Length)
            {
                groups.Add(regex[lastGroupStart..]);
            }
            return groups;
        }
    }
}