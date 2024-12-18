var board = new Board(ReadLines());
foreach (var c in board.GetAllCoords()) {
    var antenna = board.GetAntenna(c);
    if (!IsAntenna(antenna))
        continue;
    foreach (var c2 in board.GetAllCoords()) {
        if (c.Equals(c2))
            continue;

        var antenna2 = board.GetAntenna(c2);
        if (antenna != antenna2)
            continue;

        var diff = c.To(c2);
        var antinode = c2;
        while (board.WithinTheBoard(antinode)) {
            board.Mark(antinode);
            antinode = antinode.Plus(diff);
        }
    }
}
Console.WriteLine(board.GetAntinodesCount());

IEnumerable<string> ReadLines() {
    while (true) {
        var line = Console.In.ReadLine();
        if (string.IsNullOrEmpty(line))
            yield break;

        yield return line;
    }
}

bool IsAntenna(char c)
    => char.IsAsciiLetterOrDigit(c);

struct Board {
    int width;
    int height;

    bool[,] antinodes;
    char[,] antennas;

    public Board(IEnumerable<string> lines) {
        var input = lines.ToArray();
        width = input[0].Length;
        height = input.Length;
        antinodes = new bool[width, height];
        antennas = new char[width, height];
        foreach (var c in GetAllCoords())
            antennas[c.x, c.y] = input[c.y][c.x];
    }

    public int GetAntinodesCount() {
        var sum = 0;
        foreach (var c in GetAllCoords())
            if (antinodes[c.x, c.y] == true)
                sum++;
        return sum;
    }

    public IEnumerable<Coords> GetAllCoords() {
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                yield return new(x, y);
    }

    public char GetAntenna(Coords c)
        => antennas[c.x, c.y];

    public void Mark(Coords c) {
        if (WithinTheBoard(c))
            antinodes[c.x, c.y] = true;
    }

    public bool WithinTheBoard(Coords c) {
        return c.x >= 0
            && c.x < width
            && c.y >= 0
            && c.y < height;
    }
}

struct Coords {
    public readonly int x;
    public readonly int y;

    public Coords(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public bool Equals(Coords other) {
        return other.x == x && other.y == y;
    }

    public CoordsDiff To(Coords other) {
        return new(
            x: other.x - x,
            y: other.y - y
        );
    }

    public Coords Plus(CoordsDiff cd) {
        return new(
            x: x + cd.x,
            y: y + cd.y
        );
    }
}

struct CoordsDiff {
    public readonly int x;
    public readonly int y;

    public CoordsDiff(int x, int y) {
        this.x = x;
        this.y = y;
    }
}