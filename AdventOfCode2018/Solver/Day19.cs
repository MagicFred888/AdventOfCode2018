using AdventOfCode2018.Tools;

namespace AdventOfCode2018.Solver
{
    internal partial class Day19 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Go With The Flow";

        private enum OpCode
        { addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr }

        private int _instructionPtrId = 0;
        private readonly List<(OpCode opcode, int a, int b, int c)> _program = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            return RunProgramAndGetRegisterZero(0).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            return ComputeSumOfFactor(_program[21].b, _program[23].b).ToString();
        }

        private static long ComputeSumOfFactor(int a, int b)
        {
            long numberToFactorize = 10551236 + a * 22 + b; // Thanks to https://www.reddit.com/r/adventofcode/comments/a7j9zc/2018_day_19_solutions/
            return SmallTools.SumOfFactors(numberToFactorize);
        }

        private int RunProgramAndGetRegisterZero(int registerZeroIniValue)
        {
            int[] registers = [registerZeroIniValue, 0, 0, 0, 0, 0];
            registers[_instructionPtrId] = 0;
            while (registers[_instructionPtrId] < _program.Count)
            {
                int i = registers[_instructionPtrId];
                (OpCode opcode, int a, int b, int c) = _program[i];
                registers[c] = opcode switch
                {
                    OpCode.addr => registers[a] + registers[b],
                    OpCode.addi => registers[a] + b,
                    OpCode.setr => registers[a],
                    OpCode.seti => a,
                    OpCode.mulr => registers[a] * registers[b],
                    OpCode.muli => registers[a] * b,
                    OpCode.banr => registers[a] & registers[b],
                    OpCode.bani => registers[a] & b,
                    OpCode.borr => registers[a] | registers[b],
                    OpCode.bori => registers[a] | b,
                    OpCode.gtir => a > registers[b] ? 1 : 0,
                    OpCode.gtri => registers[a] > b ? 1 : 0,
                    OpCode.gtrr => registers[a] > registers[b] ? 1 : 0,
                    OpCode.eqir => a == registers[b] ? 1 : 0,
                    OpCode.eqri => registers[a] == b ? 1 : 0,
                    OpCode.eqrr => registers[a] == registers[b] ? 1 : 0,
                    _ => throw new InvalidDataException(),
                };
                registers[_instructionPtrId]++;
            }
            return registers[0];
        }

        private void ExtractData()
        {
            // Extract sample
            _program.Clear();
            _instructionPtrId = int.Parse(_puzzleInput[0].Split(" ")[1]);
            for (int i = 1; i < _puzzleInput.Count; i++)
            {
                var parts = _puzzleInput[i].Split(" ");
                _program.Add(((OpCode)Enum.Parse(typeof(OpCode), parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3])));
            }
        }
    }
}