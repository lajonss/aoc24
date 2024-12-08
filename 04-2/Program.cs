var sum = new Board(ParseInput()).AllValidCoords().Where(IsDiagonalCenter).Count();
Console.WriteLine(sum.ToString());

IReadOnlyList<string> ParseInput() {
    return GetLines().ToArray();

    IEnumerable<string> GetLines() {
        string? line;
        while ((line = Console.In.ReadLine()) != null)
            yield return line;
    }
}

bool IsDiagonalCenter(Coords coords) {
    if (coords.GetValue() != 'A')
        return false;

    return FoundDiagonal(coords.GetDiagonalIdentity())
        && FoundDiagonal(coords.GetDiagonalOpposite());
}

bool FoundDiagonal(DiagonalValues values) {
    if (values.left == 'M' && values.right == 'S')
        return true;

    if (values.left == 'S' && values.right == 'M')
        return true;

    return false;
}

public struct Board {
    readonly IReadOnlyList<string> data;

    public Board(IReadOnlyList<string> data) {
        this.data = data;
    }

    public IEnumerable<Coords> AllValidCoords() {
        for (var y = 1; y < data.Count - 1; y++)
            for (var x = 1; x < data[0].Length - 1; x++)
                yield return new Coords(data, x, y);
    }
}

public struct Coords {
    readonly IReadOnlyList<string> data;
    readonly int x;
    readonly int y;

    public Coords(IReadOnlyList<string> data, int x, int y) {
        this.data = data;
        this.x = x;
        this.y = y;
    }

    public char GetValue()
        => data[y][x];

    public DiagonalValues GetDiagonalIdentity()
        => GetDiagonal(-1, -1, 1, 1);

    public DiagonalValues GetDiagonalOpposite()
        => GetDiagonal(-1, 1, 1, -1);

    DiagonalValues GetDiagonal(int x1, int y1, int x2, int y2) {
        var coords1 = new Coords(data, x + x1, y + y1);
        var coords2 = new Coords(data, x + x2, y + y2);
        return new DiagonalValues(coords1.GetValue(), coords2.GetValue());
    }
}

public struct DiagonalValues {
    public readonly char left;
    public readonly char right;

    public DiagonalValues(char left, char right) {
        this.left = left;
        this.right = right;
    }
}
