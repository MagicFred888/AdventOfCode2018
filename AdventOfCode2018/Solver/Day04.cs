namespace AdventOfCode2018.Solver
{
    internal partial class Day04 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Repose Record";

        private static readonly Dictionary<int, List<(int sleepFrom, int sleepUntil)>> _guardSleepingTime = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            int mostSleepingGuardId = _guardSleepingTime.OrderByDescending(kvp => kvp.Value.Sum(t => t.sleepUntil - t.sleepFrom)).First().Key;
            int mostSleepingMinute = GetMostSleepingMinute(_guardSleepingTime[mostSleepingGuardId]).sleepingMinute;
            return (mostSleepingGuardId * mostSleepingMinute).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            int maxSleepingMinute = 0;
            int maxSleepingNbr = 0;
            int guardId = 0;
            foreach (KeyValuePair<int, List<(int sleepFrom, int sleepUntil)>> kvp in _guardSleepingTime)
            {
                (int sleepingMinute, int sleepingNbr) = GetMostSleepingMinute(kvp.Value);
                if (sleepingNbr >= maxSleepingNbr)
                {
                    maxSleepingMinute = sleepingMinute;
                    maxSleepingNbr = sleepingNbr;
                    guardId = kvp.Key;
                }
            }
            return (guardId * maxSleepingMinute).ToString();
        }

        private static (int sleepingMinute, int sleepingNbr) GetMostSleepingMinute(List<(int sleepFrom, int sleepUntil)> list)
        {
            int[] allMinutes = new int[60];
            foreach ((int sleepFrom, int sleepUntil) in list)
            {
                for (int i = sleepFrom; i < sleepUntil; i++)
                {
                    allMinutes[i]++;
                }
            }
            int maxSleepingNbr = allMinutes.Max();
            int maxSleepingMinute = Array.IndexOf(allMinutes, maxSleepingNbr);
            return (maxSleepingMinute, maxSleepingNbr);
        }

        private void ExtractData()
        {
            _puzzleInput.Sort();
            _guardSleepingTime.Clear();
            int currentGuard = 0;
            int startSleeping = 0;
            foreach (string line in _puzzleInput)
            {
                if (line.Contains("Guard #"))
                {
                    _ = int.TryParse(line.Split(' ')[3].Trim('#'), out currentGuard);
                }
                else if (line.Contains("falls asleep"))
                {
                    _ = int.TryParse(line.Replace("]", ":").Split(':')[1], out startSleeping);
                }
                else
                {
                    int sleepUntil = int.Parse(line.Replace("]", ":").Split(':')[1]);
                    _guardSleepingTime.TryAdd(currentGuard, []);
                    _guardSleepingTime[currentGuard].Add((startSleeping, sleepUntil));
                }
            }
        }
    }
}