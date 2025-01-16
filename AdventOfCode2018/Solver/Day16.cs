namespace AdventOfCode2018.Solver
{
    internal partial class Day16 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Chronal Classification";

        private enum OpCode
        { addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr }

        private readonly List<(List<int> before, List<int> instruction, List<int> after)> _allData = [];
        private readonly List<List<int>> _testProgram = [];

        public override string GetSolution1(bool isChallenge)
        {
            ExtractData();
            int possibleOpcodes = 0;
            foreach (var (before, instruction, after) in _allData)
            {
                int count = 0;
                foreach (OpCode opcode in Enum.GetValues(typeof(OpCode)))
                {
                    List<int> result = ApplyOpCode(opcode, before, instruction);
                    if (after.SequenceEqual(result))
                    {
                        count++;
                    }
                }
                possibleOpcodes += count >= 3 ? 1 : 0;
            }

            return possibleOpcodes.ToString();
        }

        public override string GetSolution2(bool isChallenge)
        {
            ExtractData();
            Dictionary<int, OpCode> opCodeMapping = [];
            do
            {
                foreach (var (before, instruction, after) in _allData)
                {
                    int count = 0;
                    OpCode matchingOpCode = OpCode.eqrr;
                    foreach (OpCode opcode in Enum.GetValues(typeof(OpCode)))
                    {
                        if (opCodeMapping.ContainsValue(opcode))
                        {
                            continue;
                        }
                        List<int> result = ApplyOpCode(opcode, before, instruction);
                        if (after.SequenceEqual(result))
                        {
                            count++;
                            matchingOpCode = opcode;
                        }
                    }
                    if (count == 1)
                    {
                        opCodeMapping.Add(instruction[0], matchingOpCode);
                    }
                }
            } while (opCodeMapping.Count < 16);

            // Apply the program
            List<int> registers = [0, 0, 0, 0];
            foreach (List<int> instruction in _testProgram)
            {
                OpCode opcode = opCodeMapping[instruction[0]];
                registers = ApplyOpCode(opcode, registers, instruction);
            }

            // Return register 0
            return registers[0].ToString();
        }

        private static List<int> ApplyOpCode(OpCode opcode, List<int> before, List<int> instruction)
        {
            int a = instruction[1];
            int b = instruction[2];
            int c = instruction[3];
            List<int> result = new(before)
            {
                [c] = opcode switch
                {
                    OpCode.addr => before[a] + before[b],
                    OpCode.addi => before[a] + b,
                    OpCode.mulr => before[a] * before[b],
                    OpCode.muli => before[a] * b,
                    OpCode.banr => before[a] & before[b],
                    OpCode.bani => before[a] & b,
                    OpCode.borr => before[a] | before[b],
                    OpCode.bori => before[a] | b,
                    OpCode.setr => before[a],
                    OpCode.seti => a,
                    OpCode.gtir => a > before[b] ? 1 : 0,
                    OpCode.gtri => before[a] > b ? 1 : 0,
                    OpCode.gtrr => before[a] > before[b] ? 1 : 0,
                    OpCode.eqir => a == before[b] ? 1 : 0,
                    OpCode.eqri => before[a] == b ? 1 : 0,
                    OpCode.eqrr => before[a] == before[b] ? 1 : 0,
                    _ => throw new InvalidDataException(),
                }
            };
            return result;
        }

        private void ExtractData()
        {
            int lineId = 0;

            // Extract sample
            _allData.Clear();
            while (lineId < _puzzleInput.Count && _puzzleInput[lineId].StartsWith("Before"))
            {
                List<int> before = _puzzleInput[lineId].Split('[')[1].Split(']')[0].Split(", ").ToList().ConvertAll(int.Parse);
                List<int> opcodeData = _puzzleInput[lineId + 1].Split(" ").ToList().ConvertAll(int.Parse);
                List<int> after = _puzzleInput[lineId + 2].Split('[')[1].Split(']')[0].Split(", ").ToList().ConvertAll(int.Parse);
                _allData.Add((before, opcodeData, after));
                lineId += 4;
            }

            // Extract program
            _testProgram.Clear();
            while (lineId < _puzzleInput.Count)
            {
                if (string.IsNullOrWhiteSpace(_puzzleInput[lineId]))
                {
                    lineId++;
                    continue;
                }
                _testProgram.Add(_puzzleInput[lineId].Split(' ').ToList().ConvertAll(int.Parse));
                lineId++;
            }
        }
    }
}