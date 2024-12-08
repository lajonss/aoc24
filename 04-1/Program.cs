const string WORD = "XMAS";
var sum = 0;
foreach (var coords in new Board(ParseInput()).AllCoords())
    Search(coords);
Console.WriteLine(sum.ToString());

IReadOnlyList<string> ParseInput() {
    return GetLines().ToArray();

    IEnumerable<string> GetLines() {
        string? line;
        while ((line = Console.In.ReadLine()) != null)
            yield return line;
    }
}

void Search(Coords coords) {
    if (coords.GetValue() != WORD[0])
        return;

    foreach (var direction in Direction.All())
        SearchInDirection(direction, coords.Add(direction));
}

void SearchInDirection(Direction direction, Coords coords, int index = 1) {
    if (index == WORD.Length) {
        sum += 1;
        return;
    }

    if (!coords.IsValid())
        return;


    if (coords.GetValue() == WORD[index])
        SearchInDirection(direction, coords.Add(direction), index + 1);
}

public struct Board {
    readonly IReadOnlyList<string> data;

    public Board(IReadOnlyList<string> data) {
        this.data = data;
    }

    public IEnumerable<Coords> AllCoords() {
        for (var y = 0; y < data.Count; y++)
            for (var x = 0; x < data[0].Length; x++)
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

    public Coords Add(Direction direction)
        => new(data, x + direction.x, y + direction.y);

    public bool IsValid() {
        return x >= 0
            && x < data[0].Length
            && y >= 0
            && y < data.Count;
    }
}

public struct Direction {
    public readonly int x;
    public readonly int y;

    public Direction(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static IEnumerable<Direction> All() {
        yield return new(1, 0);
        yield return new(1, -1);
        yield return new(0, -1);
        yield return new(-1, -1);
        yield return new(-1, 0);
        yield return new(-1, 1);
        yield return new(0, 1);
        yield return new(1, 1);
    }
}

