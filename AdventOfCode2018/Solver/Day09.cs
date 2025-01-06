using AdventOfCode2018.Tools;

namespace AdventOfCode2018.Solver
{
    internal partial class Day09 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Marble Mania";

        private (int nbrOfPlayer, int lastMarble) _gameInfo = (0, 0);

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            return RunGame(_gameInfo.nbrOfPlayer, _gameInfo.lastMarble).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            return RunGame(_gameInfo.nbrOfPlayer, 100 * _gameInfo.lastMarble).ToString();
        }

        private static long RunGame(int nbrOfPlayer, long lastMarble)
        {
            int marbleId = 0;
            long[] players = Enumerable.Repeat((long)0, nbrOfPlayer).ToArray();
            ChainedNode activeMarble = new(0);
            do
            {
                marbleId++;
                activeMarble = activeMarble.Next;
                if (marbleId % 23 == 0)
                {
                    // Skip Marble, add to player score and remove marble
                    players[marbleId % nbrOfPlayer] += marbleId;
                    for (int i = 0; i < 7; i++)
                    {
                        activeMarble = activeMarble.Previous;
                    }
                    players[marbleId % nbrOfPlayer] += activeMarble.Previous.Value;
                    activeMarble.RemovePreviousNode();
                }
                else
                {
                    // Place new Marble in the loop
                    ChainedNode newMarble = new(marbleId);
                    activeMarble.InsertAfter(newMarble);
                    activeMarble = newMarble;
                }
            } while (marbleId < lastMarble);
            return players.Max();
        }

        private void ExtractData()
        {
            string[] parts = _puzzleInput[0].Split(' ');
            _gameInfo = (int.Parse(parts[0]), int.Parse(parts[6]));
        }
    }
}