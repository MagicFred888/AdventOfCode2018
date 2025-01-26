using System.Text.RegularExpressions;

namespace AdventOfCode2018.Solver
{
    internal partial class Day24 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Immune System Simulator 20XX";

        private readonly List<Group> _immuneSystem = [];
        private readonly List<Group> _infection = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            PerformTheFight();
            return Math.Max(_immuneSystem.Sum(g => g.Units), _infection.Sum(g => g.Units)).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            for (int boost = 0; boost < int.MaxValue; boost += 1)
            {
                ExtractData();
                foreach (Group group in _immuneSystem)
                {
                    group.AttackDamage += boost;
                }
                PerformTheFight();
                if (_immuneSystem.Sum(g => g.Units) > 0 && _infection.Sum(g => g.Units) == 0)
                {
                    return _immuneSystem.Sum(g => g.Units).ToString();
                }
            }
            throw new InvalidDataException();
        }

        private void PerformTheFight()
        {
            bool continueFight;
            do
            {
                // Continue conditions
                continueFight = false;

                // Keep only valid groups for the round
                List<Group> allGroups = [.. _immuneSystem.FindAll(g => g.Units > 0)];
                allGroups.AddRange(_infection.FindAll(g => g.Units > 0));
                allGroups = [.. allGroups.OrderByDescending(g => (g.EffectivePower, g.Initiative))];

                // Target selection
                Dictionary<Group, Group> attackerTargetDic = [];
                foreach (Group attackingGroup in allGroups)
                {
                    List<Group> potentialTargets = allGroups.FindAll(g => g.GroupType != attackingGroup.GroupType && !attackerTargetDic.ContainsValue(g));
                    if (potentialTargets.Count == 0)
                    {
                        continue;
                    }
                    int maxDamage = potentialTargets.Max(t => t.DamageIfAttackedBy(attackingGroup));
                    if (maxDamage == 0)
                    {
                        continue;
                    }
                    Group? targetGroup = potentialTargets.FirstOrDefault(t => t.DamageIfAttackedBy(attackingGroup) == maxDamage);
                    if (targetGroup != null)
                    {
                        attackerTargetDic.Add(attackingGroup, targetGroup);
                    }
                }

                // Attack phase
                attackerTargetDic = attackerTargetDic.OrderByDescending(kv => kv.Key.Initiative).ToDictionary(kv => kv.Key, kv => kv.Value);
                foreach (KeyValuePair<Group, Group> pair in attackerTargetDic)
                {
                    Group attacker = pair.Key;
                    Group target = pair.Value;
                    int attackDamage = target.DamageIfAttackedBy(attacker);
                    int potentialUnitsKilled = (int)Math.Floor((double)attackDamage / (double)target.HitPoints);
                    continueFight = continueFight || potentialUnitsKilled > 0;
                    int unitsKilled = Math.Min(potentialUnitsKilled, target.Units);
                    target.Units -= unitsKilled;
                }
            } while (continueFight);
        }

        private void ExtractData()
        {
            _immuneSystem.Clear();
            _infection.Clear();
            List<Group> targetGroup = _immuneSystem;
            GroupType currentGroupType = GroupType.ImmuneSystem;
            int groupNumber = 1;
            foreach (string line in _puzzleInput)
            {
                if (line == "Infection:")
                {
                    groupNumber = 1;
                    targetGroup = _infection;
                    currentGroupType = GroupType.Infection;
                }
                else if (line.Contains("units", StringComparison.InvariantCultureIgnoreCase))
                {
                    targetGroup.Add(new(currentGroupType, groupNumber, line));
                    groupNumber++;
                }
            }
        }

        private enum GroupType
        {
            ImmuneSystem,
            Infection
        }

        private sealed partial class Group
        {
            public GroupType GroupType { get; init; }
            public int GroupNumber { get; init; }
            public int Units { get; set; }
            public int HitPoints { get; init; }
            public int AttackDamage { get; set; }
            public int EffectivePower => Units * AttackDamage;
            public string AttackType { get; init; }
            public int Initiative { get; init; }
            public List<string> Weaknesses { get; init; } = [];
            public List<string> Immunities { get; init; } = [];

            public Group(GroupType groupType, int groupNumber, string line)
            {
                // Check if the line is valid
                Match match = DataExtractionRegex().Match(line);
                if (!match.Success) throw new InvalidDataException("Invalid input");

                // Values always available
                GroupType = groupType;
                GroupNumber = groupNumber;
                Units = int.Parse(match.Groups["nbrUnit"].Value);
                HitPoints = int.Parse(match.Groups["hitPower"].Value);
                AttackDamage = int.Parse(match.Groups["nbrDamage"].Value);
                AttackType = match.Groups["damageType"].Value;
                Initiative = int.Parse(match.Groups["initative"].Value);

                // Optional values
                if (match.Groups["immuneWeak"].Success)
                {
                    string[] immuneWeak = match.Groups["immuneWeak"].Value.Split(';');
                    foreach (string info in immuneWeak)
                    {
                        if (info.Contains("weak", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Weaknesses = [.. info.Replace("weak to", "").Replace(" ", "").Split(",")];
                        }
                        else if (info.Contains("immune", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Immunities = [.. info.Replace("immune to", "").Replace(" ", "").Split(",")];
                        }
                    }
                }
            }

            public int DamageIfAttackedBy(Group attackingGroup)
            {
                int factor = attackingGroup.AttackType switch
                {
                    _ when Immunities.Contains(attackingGroup.AttackType) => 0,
                    _ when Weaknesses.Contains(attackingGroup.AttackType) => 2,
                    _ => 1
                };
                return attackingGroup.EffectivePower * factor;
            }

            public override string ToString()
            {
                return $"{GroupType} {GroupNumber}, Units: {Units}, HitPoints: {HitPoints}, " +
                    $"Immune: {(Immunities.Count == 0 ? "none" : string.Join(", ", Immunities))}, " +
                    $"Weak: {(Weaknesses.Count == 0 ? "none" : string.Join(", ", Weaknesses))}, " +
                    $"Attack: {AttackDamage} {AttackType}, Initiative: {Initiative}, " +
                    $"Effective Power: {EffectivePower}";
            }

            [GeneratedRegex(@"^(?<nbrUnit>\d+) units each with (?<hitPower>\d+) hit points( \((?<immuneWeak>[^)]*)\))? with an attack that does (?<nbrDamage>\d+) (?<damageType>[a-zA-Z]+) damage at initiative (?<initative>\d+)$")]
            private static partial Regex DataExtractionRegex();
        }
    }
}