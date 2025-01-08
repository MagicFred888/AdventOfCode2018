using AdventOfCode2018.Tools;
using System.Text;

namespace AdventOfCode2018.Solver
{
    internal partial class Day14 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Chocolate Charts";

        public override string GetSolution1(bool isChallenge)
        {
            return GetReceipt(long.Parse(_puzzleInput[0]), false);
        }

        public override string GetSolution2(bool isChallenge)
        {
            return GetReceipt(long.Parse(_puzzleInput[0]), true);
        }

        private static string GetReceipt(long targetNumberOfRecipe, bool findTargetNumber)
        {
            // Initialization
            long nbrOfRecipeMade = 2;
            StringBuilder lastTen = new("37");
            ChainedNode baseNode = new(3);
            baseNode.InsertAfter(new(7));
            ChainedNode elve1 = baseNode;
            ChainedNode elve2 = baseNode.Next;

            // Loop
            while (nbrOfRecipeMade < long.MaxValue)
            {
                // Create new receipt
                long recipe = elve1.Value + elve2.Value;
                if (recipe >= 10)
                {
                    // Recipe split in 2
                    baseNode.InsertBefore(new ChainedNode(1));
                    lastTen.Append('1');
                    baseNode.InsertBefore(new ChainedNode((int)recipe % 10));
                    lastTen.Append(recipe % 10);
                    nbrOfRecipeMade += 2;
                }
                else
                {
                    // Recipe don't split
                    baseNode.InsertBefore(new ChainedNode((int)recipe));
                    lastTen.Append(recipe);
                    nbrOfRecipeMade++;
                }

                if (findTargetNumber && lastTen.ToString().Contains(targetNumberOfRecipe.ToString()))
                {
                    return (nbrOfRecipeMade - lastTen.ToString()[lastTen.ToString().IndexOf(targetNumberOfRecipe.ToString())..].Length).ToString();
                }
                if (lastTen.Length > 10)
                {
                    lastTen = lastTen.Remove(0, lastTen.Length - 10);
                }

                // Move elves
                elve1 = elve1.MoveForward(elve1.Value + 1);
                elve2 = elve2.MoveForward(elve2.Value + 1);

                // Done?
                if (!findTargetNumber && nbrOfRecipeMade >= targetNumberOfRecipe + 10)
                {
                    break;
                }
            }

            // Extract result
            StringBuilder result = new();
            baseNode = baseNode.MoveBackward(10);
            for (long i = 0; i < 10; i++)
            {
                result.Append(baseNode.Value);
                baseNode = baseNode.Next;
            }
            return result.ToString();
        }
    }
}