namespace AdventOfCode2018.Solver
{
    internal partial class Day08 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Memory Maneuver";

        private sealed class Node(List<int> header)
        {
            public List<int> Header = header;
            public List<Node> Children = [];
            public List<int> MetaData = [];
        }

        private List<int> _data = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            Node root = ExtractNode();
            return ComputeMetaDataSum(root).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            Node root = ExtractNode();
            return ComputeMetaDataSum2(root).ToString();
        }

        private static long ComputeMetaDataSum2(Node root)
        {
            if (root.Children.Count == 0)
            {
                return root.MetaData.Sum();
            }
            long result = 0;
            foreach (int id in root.MetaData)
            {
                if (id > root.Children.Count)
                {
                    continue;
                }
                result += ComputeMetaDataSum2(root.Children[id - 1]);
            }
            return result;
        }

        private static long ComputeMetaDataSum(Node root)
        {
            long result = root.MetaData.Sum();
            foreach (Node child in root.Children)
            {
                result += ComputeMetaDataSum(child);
            }
            return result;
        }

        private Node ExtractNode()
        {
            Node newNode = new(_data.GetRange(0, 2));
            _data = _data.GetRange(2, _data.Count - 2);
            for (int i = 0; i < newNode.Header[0]; i++)
            {
                newNode.Children.Add(ExtractNode());
            }
            newNode.MetaData = _data.GetRange(0, newNode.Header[1]);
            _data = _data.GetRange(newNode.Header[1], _data.Count - newNode.Header[1]);
            return newNode;
        }

        private void ExtractData()
        {
            _data = _puzzleInput[0].Split(" ").Select(int.Parse).ToList();
        }
    }
}