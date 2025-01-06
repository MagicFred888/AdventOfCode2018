using System.Diagnostics;
using System.Text;

namespace AdventOfCode2018.Tools
{
    public sealed class ChainedNode
    {
        public int Value { get; init; }
        public ChainedNode Next { get; set; }
        public ChainedNode Previous { get; set; }

        public ChainedNode(int value)
        {
            Value = value;
            Next = this;
            Previous = this;
        }

        public void InsertAfter(ChainedNode newNode)
        {
            newNode.Next = Next;
            Next.Previous = newNode;
            Next = newNode;
            newNode.Previous = this;
        }

        public void RemovePreviousNode()
        {
            ChainedNode marble = Previous;
            Previous = marble.Previous;
            marble.Previous.Next = this;
        }

        public void DebugPrint(ChainedNode activeMarble)
        {
            // Debugging method to print the current state of the game
            ChainedNode tmp = this;
            StringBuilder stringBuilder = new();
            do
            {
                bool isActive = tmp == activeMarble;
                stringBuilder.Append(isActive ? "[" : "");
                stringBuilder.Append(tmp.Value);
                stringBuilder.Append(isActive ? "]" : "");
                stringBuilder.Append("  ");
                tmp = tmp.Next;
            } while (tmp != Previous);
            Debug.WriteLine(stringBuilder.ToString().Trim());
        }
    }
}