var map = ParseMap();

var ends = new Ends();
var queue = new Queue<Trail>();
foreach (var coords in map.GetAllCoords())
    queue.Enqueue(new Trail(Value.Start, coords, start: coords));
while (queue.TryDequeue(out var trail)) {
    var value = trail.value;
    if (!map.GetValue(trail.coords).Equals(value))
        continue;

    if (value.Equals(Value.End)) {
        ends.Add(trail.start, trail.coords);
        continue;
    }

    var nextValue = value.Next();
    foreach (var neighbour in trail.coords.GetNeighbours())
        queue.Enqueue(new(nextValue, neighbour, start: trail.start));
}
Console.WriteLine(ends.GetScore());

Map ParseMap() {
    var lines = GetLines().ToArray();
    var width = lines[0].Length;
    var height = lines.Length;
    var values = new int[width, height];
    for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            values[x, y] = lines[y][x] - '0';
    return new(width, height, values);

    IEnumerable<string> GetLines() {
        while (true) {
            var line = Console.In.ReadLine();
            if (line == null)
                yield break;
            yield return line;
        }
    }
}

class Map(int width, int height, int[,] values) {
    public readonly int width = width;
    public readonly int height = height;

    readonly int[,] values = values;

    public Value GetValue(Coords coords) {
        if (coords.x >= width || coords.y >= height)
            return Value.Invalid;

        return new(values[coords.x, coords.y]);
    }

    public IEnumerable<Coords> GetAllCoords() {
        for (var y = 0u; y < height; y++)
            for (var x = 0u; x < width; x++)
                yield return new(x, y);
    }
}

struct Coords(uint x, uint y) : IEquatable<Coords> {
    public readonly uint x = x;
    public readonly uint y = y;

    public bool Equals(Coords other) {
        return other.x == x && other.y == y;
    }

    public IEnumerable<Coords> GetNeighbours() {
        yield return new(x - 1, y);
        yield return new(x + 1, y);
        yield return new(x, y - 1);
        yield return new(x, y + 1);
    }
}

struct Value(int value) {
    readonly int value = value;

    public static Value Start => new(0);
    public static Value End => new(9);
    public static Value Invalid => new(-2);

    public bool Equals(Value other) {
        return other.value == value;
    }

    public Value Next()
        => new(value + 1);

    public override string ToString()
        => value.ToString();
}

struct Trail(Value value, Coords coords, Coords start) {
    public readonly Coords start = start;
    public readonly Value value = value;
    public readonly Coords coords = coords;
}

class Ends {
    List<EndRecord> records = new();

    public void Add(Coords start, Coords end) {
        GetRecord(start).Add(end);
        return;

        EndRecord GetRecord(Coords start) {
            foreach (var record in records)
                if (record.start.Equals(start))
                    return record;
            var newRecord = new EndRecord(start);
            records.Add(newRecord);
            return newRecord;
        }
    }

    public int GetScore()
        => records.Select(r => r.GetScore()).Sum();

    class EndRecord(Coords start) {
        public readonly Coords start = start;
        readonly List<Coords> ends = new();

        public void Add(Coords end) {
            if (!ends.Contains(end))
                ends.Add(end);
        }

        public int GetScore()
            => ends.Count;
    }
}
