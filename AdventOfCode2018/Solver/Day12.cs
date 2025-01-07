namespace AdventOfCode2018.Solver
{
    internal partial class Day12 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Subterranean Sustainability";

        private readonly List<(char[] iniState, char result)> _stateConversion = [];
        private char[] _initialState = [];
        private readonly int _extraBufferSize = 250;

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            return ComputeGeneration(20).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            // It stabilize after around 150 generations so we go to 200 and then we just need multiply the remaining by the linear increase...
            long val1 = ComputeGeneration(199);
            long val2 = ComputeGeneration(1);
            long difference = val2 - val1;
            return (val2 + (50_000_000_000L - 200) * difference).ToString();
        }

        private long ComputeGeneration(int nbrOfGeneration)
        {
            // Make number of cycles
            for (int i = 0; i < nbrOfGeneration; i++)
            {
                char[] nextGen = Enumerable.Repeat('.', _initialState.Length).ToArray();
                foreach (var (iniState, newState) in _stateConversion)
                {
                    for (int j = 0; j < _initialState.Length - 4; j++)
                    {
                        if (Enumerable.SequenceEqual(iniState, _initialState[j..(j + 5)]))
                        {
                            nextGen[j + 2] = newState;
                        }
                    }
                }
                _initialState = nextGen;
            }

            // Count pots
            long result = 0;
            for (int i = 0; i < _initialState.Length; i++)
            {
                result += _initialState[i] == '#' ? (long)(i - _extraBufferSize) : 0L;
            }
            return result;
        }

        private void ExtractData()
        {
            _initialState = (new string(Enumerable.Repeat('.', _extraBufferSize).ToArray()) + _puzzleInput[0].Split(" ")[2] + new string(Enumerable.Repeat('.', _extraBufferSize).ToArray())).ToCharArray();
            for (int i = 2; i < _puzzleInput.Count; i++)
            {
                var parts = _puzzleInput[i].Split(" => ");
                _stateConversion.Add((parts[0].ToCharArray(), parts[1][0]));
            }
        }
    }
}