using System.Drawing;

namespace AdventOfCode2018.Tools;

public class QuickGrid
{
    public int MinX { get; init; }
    public int MaxX { get; init; }
    public int MinY { get; init; }
    public int MaxY { get; init; }
    public int NbrRow { get; init; }
    public int NbrCol { get; init; }

    public enum TouchingMode
    {
        Horizontal,
        Vertical,
        Diagonal,
        All
    }

    public class CellInfo(Point position, long value)
    {
        public Point Position { get; init; } = position;
        public long Value { get; set; } = value;

        public override string ToString() => $"{Position} : {Value}";
    }

    private readonly Dictionary<TouchingMode, List<Point>> _touchingMode = new()
    {
        { TouchingMode.Horizontal, new() { new Point(-1, 0), new Point(1, 0) } },
        { TouchingMode.Vertical, new() { new Point(0, -1), new Point(0, 1) } },
        { TouchingMode.Diagonal, new() { new Point(-1, -1), new Point(1, 1), new Point(-1, 1), new Point(1, -1) } },
        { TouchingMode.All, new() { new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(-1, 0), new Point(1, 0), new Point(-1, 1), new Point(0, 1), new Point(1, 1) } }
    };

    private readonly Dictionary<Point, CellInfo> _allCells = [];

    public QuickGrid(int xMin, int xMax, int yMin, int yMax, long defaultValue)
    {
        MinX = xMin;
        MaxX = xMax;
        MinY = yMin;
        MaxY = yMax;
        NbrRow = yMax - yMin + 1;
        NbrCol = xMax - xMin + 1;

        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                _allCells.Add(new(x, y), new(new(x, y), defaultValue));
            }
        }
    }

    public CellInfo? Cell(Point position) => Cell(position.X, position.Y);

    public CellInfo? Cell(int x, int y)
    {
        if (x < MinX || x > MaxX || y < MinY || y > MaxY)
        {
            return null;
        }
        return _allCells[new(x, y)];
    }

    public List<CellInfo> TouchingCells(Point position, TouchingMode touchingMode) => TouchingCells(position.X, position.Y, touchingMode);

    public List<CellInfo> TouchingCells(int x, int y, TouchingMode touchingMode)
    {
        List<CellInfo> result = [];
        foreach (Point move in _touchingMode[touchingMode])
        {
            CellInfo? cell = Cell(x + move.X, y + move.Y);
            if (cell != null)
            {
                result.Add(cell);
            }
        }
        return result;
    }

    public List<CellInfo> Cells => _allCells.ToList().ConvertAll(c => c.Value);
}