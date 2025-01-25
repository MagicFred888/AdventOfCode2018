namespace AdventOfCode2018.Solver
{
    internal partial class Day23 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Experimental Emergency Teleportation";

        private sealed class Nanobot(long x, long y, long z, long r)
        {
            public long X = x;
            public long Y = y;
            public long Z = z;
            public long Radius = r;

            public long ManhattanDistance()
            {
                return ManhattanDistance(new Nanobot(0, 0, 0, 0));
            }

            public long ManhattanDistance(Nanobot other)
            {
                return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
            }
        }

        private readonly List<Nanobot> _allNanobots = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            long maxRadius = _allNanobots.Max(nb => nb.Radius);
            Nanobot strongest = _allNanobots.Find(nb => nb.Radius == maxRadius) ?? throw new InvalidDataException();
            return _allNanobots.Count(nb => nb.ManhattanDistance(strongest) <= strongest.Radius).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();

            // Compute bounding box of all robots
            long minX = _allNanobots.Min(nb => nb.X);
            long minY = _allNanobots.Min(nb => nb.Y);
            long minZ = _allNanobots.Min(nb => nb.Z);
            long maxX = _allNanobots.Max(nb => nb.X);
            long maxY = _allNanobots.Max(nb => nb.Y);
            long maxZ = _allNanobots.Max(nb => nb.Z);
            long xRange = maxX - minX;
            long yRange = maxY - minY;
            long zRange = maxZ - minZ;

            // To speed up the search, we look for a step that is a power of 2 and divide the range in approximately 2 to do a kind of binary search.
            long scanStep = 1;
            while (xRange / scanStep > 2 || yRange / scanStep > 2 || zRange / scanStep > 2)
            {
                scanStep *= 2;
            }

            // Search the best location
            long maxNanobotInRange = 0;
            long minManhattanDistance;
            (long x, long y, long z) bestLocation = (0, 0, 0);
            do
            {
                maxNanobotInRange = 0;
                minManhattanDistance = long.MaxValue;
                for (long x = minX; x < maxX; x += scanStep)
                {
                    for (long y = minY; y < maxY; y += scanStep)
                    {
                        for (long z = minZ; z < maxZ; z += scanStep)
                        {
                            long nanoBotInRange = _allNanobots.Count(nb => nb.ManhattanDistance(new Nanobot(x, y, z, 0)) <= nb.Radius);
                            if (nanoBotInRange > maxNanobotInRange
                                || nanoBotInRange == maxNanobotInRange && new Nanobot(x, y, z, 0).ManhattanDistance() < minManhattanDistance)
                            {
                                maxNanobotInRange = nanoBotInRange;
                                bestLocation = (x, y, z);
                                minManhattanDistance = new Nanobot(x, y, z, 0).ManhattanDistance();
                            }
                        }
                    }
                }

                // Have we reached the scanStep limit ?
                if (scanStep == 1)
                {
                    break;
                }

                // Refine scanStep and the search area size and position (we keep it centered on the best location)
                scanStep /= 2;
                xRange = 1 + xRange / 2;
                yRange = 1 + yRange / 2;
                zRange = 1 + zRange / 2;
                minX = bestLocation.x - xRange / 2;
                minY = bestLocation.y - yRange / 2;
                minZ = bestLocation.z - zRange / 2;
                maxX = bestLocation.x + xRange / 2;
                maxY = bestLocation.y + yRange / 2;
                maxZ = bestLocation.z + zRange / 2;
            } while (true);

            return minManhattanDistance.ToString();
        }

        private void ExtractData()
        {
            _allNanobots.Clear();
            foreach (string line in _puzzleInput)
            {
                string[] parts = line.Replace("pos=<", "").Replace(">, r=", ",").Split(',');
                _allNanobots.Add(new(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]), long.Parse(parts[3])));
            }
        }
    }
}