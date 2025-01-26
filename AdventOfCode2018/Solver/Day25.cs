using AdventOfCode2018.Tools;

namespace AdventOfCode2018.Solver
{
    internal partial class Day25 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Four-Dimensional Adventure";

        private List<Point4D> _allPoints = [];

        private sealed class Point4D
        {
            public string Name { get; init; }
            public int A { get; init; }
            public int B { get; init; }
            public int C { get; init; }
            public int D { get; init; }

            public Point4D(string rawData)
            {
                Name = rawData;
                string[] parts = rawData.Split(',');
                A = int.Parse(parts[0]);
                B = int.Parse(parts[1]);
                C = int.Parse(parts[2]);
                D = int.Parse(parts[3]);
            }
        }

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();

            // Create all links
            List<(string from, string to, long distance)> allLinks = [];
            for (int i = 0; i < _allPoints.Count; i++)
            {
                Point4D p1 = _allPoints[i];
                for (int j = i + 1; j < _allPoints.Count; j++)
                {
                    Point4D p2 = _allPoints[j];
                    if (Math.Abs(p1.A - p2.A) + Math.Abs(p1.B - p2.B) + Math.Abs(p1.C - p2.C) + Math.Abs(p1.D - p2.D) <= 3)
                    {
                        allLinks.Add((p1.Name, p2.Name, 1));
                    }
                }
            }

            // Count networks
            int nbrOfNetworks = 0;
            QuickDijkstra quickDijkstra = new(allLinks);
            while (_allPoints.Count > 0)
            {
                nbrOfNetworks++;
                List<Point4D> tmpNetwork = quickDijkstra.GetNodesInNetwork(_allPoints[0].Name).ConvertAll(p => _allPoints.FirstOrDefault(i => i.Name == p))!;
                _allPoints = _allPoints.Except(tmpNetwork).ToList();
            }
            return nbrOfNetworks.ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            return "Merry Christmas !";
        }

        private void ExtractData()
        {
            _allPoints.Clear();
            foreach (string line in _puzzleInput)
            {
                _allPoints.Add(new Point4D(line));
            }
        }
    }
}