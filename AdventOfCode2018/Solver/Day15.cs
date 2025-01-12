using AdventOfCode2018.Tools;
using System.Drawing;
using static AdventOfCode2018.Tools.QuickMaze;

namespace AdventOfCode2018.Solver
{
    internal partial class Day15 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Beverage Bandits";

        private enum UnitType
        {
            Elf,
            Goblin
        }

        private sealed class Unit(UnitType unitType, Point position)
        {
            public UnitType Type { get; init; } = unitType;
            public Point Position { get; set; } = position;
            public int HitPoints { get; set; } = 200;
            public int AttackPower { get; set; } = 3;
            public bool IsAlive => HitPoints > 0;

            public Unit Clone()
            {
                return new(Type, new Point(Position.X, Position.Y))
                {
                    HitPoints = HitPoints,
                    AttackPower = AttackPower
                };
            }

            internal void Attack(Unit unitToAttack)
            {
                unitToAttack.HitPoints -= AttackPower;
                if (unitToAttack.HitPoints <= 0)
                {
                    unitToAttack.HitPoints = 0;
                    unitToAttack.Position = new Point(-1, -1);
                }
            }
        }

        private QuickMatrix _map = new();
        private List<Unit> _units = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            return ComputeBattleResult().ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            QuickMatrix clonnedMap = _map.Clone();
            List<Unit> clonedUnits = new(_units.ConvertAll(u => u.Clone()));

            // Increase attack power until no elf dies
            int ap = 3;
            long answer = 0;
            do
            {
                ap++;
                _map = clonnedMap.Clone();
                _units = new(clonedUnits.ConvertAll(u => u.Clone()));
                _units.ForEach(u => u.AttackPower = u.Type == UnitType.Goblin ? 3 : ap);
                answer = ComputeBattleResult();
            } while (_units.Any(u => !u.IsAlive && u.Type == UnitType.Elf));

            // Done
            return answer.ToString();
        }

        private long ComputeBattleResult()
        {
            int nbrFullRounds = 0;
            do
            {
                // Sort all based on Y and then X if Y is same
                _units = [.. _units.OrderBy(u => u.Position.Y).ThenBy(u => u.Position.X)];

                // Start round
                foreach (Unit refUnit in _units.FindAll(u => u.IsAlive))
                {
                    if (!refUnit.IsAlive)
                    {
                        continue;
                    }

                    // Get targets and check if there are any. If not game is over
                    List<Unit> targets = _units.FindAll(u => u.IsAlive && u.Type != refUnit.Type);
                    if (targets.Count == 0)
                    {
                        return nbrFullRounds * _units.Sum(u => u.HitPoints);
                    }

                    // Create up-to-date map
                    QuickMatrix blockedMap = _map.Clone();
                    _units.ForEach(u => blockedMap.Cell(u.Position).StringVal = "#");

                    // Check if we can attack
                    Unit? unitToAttack = GetAttackInfo(refUnit, targets, blockedMap);
                    if (unitToAttack != null)
                    {
                        // Perform the attack
                        refUnit.Attack(unitToAttack);
                    }
                    else
                    {
                        // Move
                        Point? nextMove = GetNextMove(refUnit, targets, blockedMap);
                        if (nextMove != null)
                        {
                            refUnit.Position = nextMove.Value;
                        }

                        // Check if we can attack
                        unitToAttack = GetAttackInfo(refUnit, targets, blockedMap);
                        if (unitToAttack != null)
                        {
                            // Perform the attack
                            refUnit.Attack(unitToAttack);
                        }
                    }
                }

                // Next round
                nbrFullRounds++;
            } while (true);
        }

        private static Point? GetNextMove(Unit refUnit, List<Unit> targets, QuickMatrix blockedMap)
        {
            // Serach possible target
            List<CellInfo> possibleCells = targets.Aggregate(new List<CellInfo>(), (acc, val) =>
            {
                acc.AddRange(blockedMap.GetNeighbours(val.Position, TouchingMode.HorizontalAndVertical));
                return acc;
            });
            possibleCells = possibleCells.FindAll(c => c.StringVal == ".");
            List<Point> possiblePoints = possibleCells.ConvertAll(c => c.Position).Distinct().ToList();

            // Using maze resolve
            long[,] distance = QuickMaze.CalculateDistancesToPosition(blockedMap, refUnit.Position, "#");
            Dictionary<Point, int> validDistance = possiblePoints.FindAll(c => distance[c.X, c.Y] > -1)
                .ToDictionary(c => c, c => (int)distance[c.X, c.Y]);

            // sort to pick right one and return result
            if (validDistance.Count == 0)
            {
                return null;
            }
            int minDistance = validDistance.Min(c => c.Value);
            validDistance = validDistance.Where(c => c.Value == minDistance).ToDictionary(c => c.Key, c => c.Value);
            Point nextMove = validDistance.Keys.OrderBy(p => p.Y).ThenBy(p => p.X).First();

            // Now that we have target we compute path
            QuickMaze.SolveMaze(blockedMap, nextMove, refUnit.Position, "#");
            List<CellInfo> possibleMove = blockedMap.GetNeighbours(refUnit.Position, TouchingMode.HorizontalAndVertical);
            possibleMove = possibleMove.FindAll(c => c.StringVal != "#" && ((MazeCellInfos)c.ObjectVal!).PartOfBest);
            int best = possibleMove.Min(c => (int)((MazeCellInfos)c.ObjectVal!).DistanceToStart);
            possibleMove = possibleMove.FindAll(c => (int)((MazeCellInfos)c.ObjectVal!).DistanceToStart == best);
            possibleMove = [.. possibleMove.OrderBy(c => c.Position.Y).ThenBy(c => c.Position.X)];
            return possibleMove.FirstOrDefault()?.Position;
        }

        private static Unit? GetAttackInfo(Unit refUnit, List<Unit> targets, QuickMatrix blockedMap)
        {
            // Search possible target in range
            List<Unit> possibleTargets = [];
            foreach (Point possibleTarget in blockedMap.GetNeighbours(refUnit.Position, TouchingMode.HorizontalAndVertical).Select(c => c.Position))
            {
                Unit? target = targets.Find(u => u.Position == possibleTarget && u.Type != refUnit.Type);
                if (target != null)
                {
                    possibleTargets.Add(target);
                }
            }

            // Search the one with min Hit Points
            if (possibleTargets.Count == 0)
            {
                return null;
            }
            int minHitPoints = possibleTargets.Min(u => u.HitPoints);
            possibleTargets = possibleTargets.FindAll(u => u.HitPoints == minHitPoints);

            // sort to pick right one and return result
            possibleTargets = [.. possibleTargets.OrderBy(u => u.Position.Y).ThenBy(u => u.Position.X)];
            return possibleTargets.Count != 0 ? possibleTargets[0] : null;
        }

        private void ExtractData()
        {
            _map = new QuickMatrix(_puzzleInput);
            _units.Clear();
            foreach (CellInfo cell in _map.Cells.FindAll(c => c.StringVal is "G" or "E"))
            {
                Unit newUnit = new(cell.StringVal == "E" ? UnitType.Elf : UnitType.Goblin, cell.Position);
                _units.Add(newUnit);
                _map.Cell(newUnit.Position).StringVal = ".";
            }
        }
    }
}