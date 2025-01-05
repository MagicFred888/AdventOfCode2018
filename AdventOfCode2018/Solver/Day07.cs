using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2018.Solver
{
    internal partial class Day07 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "The Sum of Its Parts";

        private sealed class Step(char name)
        {
            public char Name = name;
            public List<char> MustDoneBefore = [];
        }

        private readonly Dictionary<char, Step> _steps = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            return ExecuteAssembly(_steps, 1, 0).result.ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            return ExecuteAssembly(_steps, isChallenge ? 5 : 2, isChallenge ? 60 : 0).time.ToString();
        }

        private static (string result, int time) ExecuteAssembly(Dictionary<char, Step> steps, int nbrOfWorkers, int baseTime)
        {
            // Initialize
            int finalLength = steps.Count;
            StringBuilder result = new();
            List<char> upcomingSteps = [];
            int time = -1;
            Dictionary<char, int> workers = [];

            // Solving loop
            do
            {
                // Increase time
                time++;

                // Reduce all workers time by 1
                foreach (char letter in workers.Keys)
                {
                    workers[letter]--;
                    if (workers[letter] == 0)
                    {
                        // Item is completed, we add it in result and remove it from workers
                        result.Append(letter);
                        steps.ToList().ForEach(x => x.Value.MustDoneBefore.Remove(letter));
                        workers.Remove(letter);
                    }
                }

                // Add new steps to upcomingSteps
                upcomingSteps.AddRange(steps.Where(x => x.Value.MustDoneBefore.Count == 0).Select(x => x.Value.Name));
                upcomingSteps = upcomingSteps.Distinct().ToList();
                upcomingSteps.Sort();

                // Select next job for availlaible workers
                while (workers.Count < nbrOfWorkers && upcomingSteps.Count > 0)
                {
                    workers[upcomingSteps[0]] = upcomingSteps[0] - 'A' + 1 + baseTime;
                    steps.Remove(upcomingSteps[0]);
                    upcomingSteps.RemoveAt(0);
                }
            } while (result.Length != finalLength);
            return (result.ToString(), time);
        }

        private void ExtractData()
        {
            _steps.Clear();
            foreach (string line in _puzzleInput)
            {
                Match match = BueprintExtractorRegex().Match(line);
                if (match.Success)
                {
                    char before = match.Groups["before"].Value[0];
                    char node = match.Groups["node"].Value[0];
                    _steps.TryAdd(before, new Step(before));
                    _steps.TryAdd(node, new Step(node));
                    _steps[node].MustDoneBefore.Add(before);
                }
            }
        }

        [GeneratedRegex(@"^Step (?<before>[A-Z]) must be finished before step (?<node>[A-Z]) can begin.$")]
        private static partial Regex BueprintExtractorRegex();
    }
}