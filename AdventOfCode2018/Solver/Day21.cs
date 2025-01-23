namespace AdventOfCode2018.Solver
{
    internal partial class Day21 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Chronal Conversion";

        private enum OpCode
        { addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr }

        private int _instructionPtrId = 0;
        private readonly List<(OpCode opcode, int a, int b, int c)> _program = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            return RunProgramAndGetRegisterZero(true).ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            return RunProgramAndGetRegisterZero(false).ToString();
        }

        private int RunProgramAndGetRegisterZero(bool isPart1)
        {
            List<int> alreadySeen = [];
            int nbrOfLoops;
            nbrOfLoops = 0;
            int[] registers = [0, 0, 0, 0, 0, 0];
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

                // Find value
                if (i == 28)
                {
                    if (isPart1)
                    {
                        return registers[1];
                    }
                    else
                    {
                        if (!alreadySeen.Contains(registers[1]))
                        {
                            alreadySeen.Add(registers[1]);
                        }
                        else
                        {
                            return alreadySeen[^1];
                        }
                    }
                }

                registers[_instructionPtrId]++;
                nbrOfLoops++;
            }
            throw new InvalidDataException();
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