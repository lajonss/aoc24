var input = ParseInput();
var board = input.Board;
var guard = input.Guard;

var loops = 0;
for (var x = 0; x < board.width; x++)
    for (var y = 0; y < board.height; y++)
        if (CanLoop(x, y))
            loops += 1;
Console.WriteLine(loops.ToString());

Input ParseInput() {
    Guard guard = default;
    var data = new List<List<bool>>();
    var y = 0;
    foreach (var line in GetLines()) {
        ParseLine(line);
        y++;
    }
    return new Input(guard, new(data));

    void ParseLine(string line) {
        var lineData = new List<bool>();
        var x = 0;
        foreach (var character in line) {
            if (character == '.')
                lineData.Add(false);
            else if (character == '#')
                lineData.Add(true);
            else {
                guard = new Guard(new Location(x, y), ParseDirection(character));
                lineData.Add(false);
            }
            x++;
        }
        data.Add(lineData);
    }

    IEnumerable<string> GetLines() {
        string? line;
        while ((line = Console.In.ReadLine()) != null)
            yield return line;
    }
}

bool CanLoop(int x, int y) {
    if (board.IsSolid(x, y))
        return false;

    var boardCopy = board.CopyWithSolidAt(x, y);
    var guardCopy = new GuardCopy(guard, boardCopy);
    GuardMovementResult result;
    do {
        result = guardCopy.Advance();
    } while (result == GuardMovementResult.Continue);
    return result == GuardMovementResult.Looped;
}


Direction ParseDirection(char character) {
    return character switch {
        '^' => Direction.Up,
        '>' => Direction.Right,
        'v' => Direction.Down,
        '<' => Direction.Left,
        _ => throw new Exception("Failed to parse direction")
    };
}

enum Direction {
    Up,
    Right,
    Down,
    Left
}

readonly record struct Location(int X, int Y) {
    public Location Add(Direction direction) {
        return direction switch {
            Direction.Up => new(X, Y - 1),
            Direction.Right => new(X + 1, Y),
            Direction.Down => new(X, Y + 1),
            Direction.Left => new(X - 1, Y),
            _ => throw new Exception()
        };
    }
}
readonly record struct Guard(Location Location, Direction Direction);
readonly record struct Input(Guard Guard, Board Board);

class GuardCopy(Guard guard, BoardCopy board) {
    Location location = guard.Location;
    Direction direction = guard.Direction;
    BoardCopy board = board;

    public GuardMovementResult Advance() {
        var nextLocation = location.Add(direction);
        if (!board.Contains(nextLocation))
            return GuardMovementResult.WentOut;
        var nextField = board[nextLocation];
        if (nextField.solid) {
            direction = NextDirection(direction);
            var currentField = board[location];
            if (currentField.Turned(direction))
                return GuardMovementResult.Looped;
            board[location] = currentField.MarkTurned(direction);
            return GuardMovementResult.Continue;
        }
        if (nextField.Visited(direction))
            return GuardMovementResult.Looped;

        board[location] = board[location].MarkWent(direction);
        location = nextLocation;
        return GuardMovementResult.Continue;

        Direction NextDirection(Direction direction) {
            return direction switch {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => throw new Exception()
            };
        }
    }
}

enum GuardMovementResult {
    Continue,
    WentOut,
    Looped
}

class Board {
    readonly bool[,] solids;
    readonly Field[,] cachedFields;

    public readonly int width;
    public readonly int height;

    public Board(List<List<bool>> data) {
        width = data[0].Count;
        height = data.Count;
        solids = new bool[width, height];
        cachedFields = new Field[width, height];
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                solids[x, y] = data[y][x];
    }

    public bool IsSolid(int x, int y)
        => solids[x, y];

    public BoardCopy CopyWithSolidAt(int x, int y) {
        for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                cachedFields[i, j] = new Field(solid: solids[i, j]);
        cachedFields[x, y] = new Field(solid: true);
        return new(cachedFields);
    }
}

class BoardCopy(Field[,] fields) {
    readonly Field[,] fields = fields;

    public Field this[Location location] {
        get => fields[location.X, location.Y];
        set => fields[location.X, location.Y] = value;
    }

    public bool Contains(Location location) {
        var x = location.X;
        var y = location.Y;
        return x >= 0 && x < fields.GetLength(0) && y >= 0 && y < fields.GetLength(1);
    }
}

struct Field(bool solid) {
    public readonly bool solid = solid;

    public bool wentLeft;
    public bool wentUp;
    public bool wentRight;
    public bool wentDown;

    public bool turnedLeft;
    public bool turnedUp;
    public bool turnedRight;
    public bool turnedDown;

    public bool Turned(Direction direction) {
        return direction switch {
            Direction.Up => turnedUp,
            Direction.Right => turnedRight,
            Direction.Down => turnedDown,
            Direction.Left => turnedLeft,
            _ => throw new Exception()
        };
    }

    public bool Visited(Direction direction) {
        return direction switch {
            Direction.Up => wentUp,
            Direction.Right => wentRight,
            Direction.Down => wentDown,
            Direction.Left => wentLeft,
            _ => throw new Exception()
        };
    }

    public Field MarkTurned(Direction direction) {
        switch (direction) {
            case Direction.Up:
                turnedUp = true;
                return this;
            case Direction.Right:
                turnedRight = true;
                return this;
            case Direction.Down:
                turnedDown = true;
                return this;
            case Direction.Left:
                turnedLeft = true;
                return this;
        }
        throw new Exception();
    }

    public Field MarkWent(Direction direction) {
        switch (direction) {
            case Direction.Up:
                wentUp = true;
                return this;
            case Direction.Right:
                wentRight = true;
                return this;
            case Direction.Down:
                wentDown = true;
                return this;
            case Direction.Left:
                wentLeft = true;
                return this;
        }
        throw new Exception();
    }
}
