var plots = ParseInput();
var regions = AssignRegions(plots);
var price = regions.GetAllRegions().Select(ComputePrice).Sum();
Console.WriteLine(price.ToString());


Plots ParseInput() {
    var input = GetLines().ToArray();
    var width = (uint)input[0].Length;
    var height = (uint)input.Length;
    var output = new char[width, height];
    for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            output[x, y] = input[y][x];
    return new(output);
}

Regions AssignRegions(Plots plots) {
    var regions = new Regions(new Region[plots.width, plots.height]);
    foreach (var coords in plots.GetAllCoords())
        AssignRegions(coords, plots);
    return regions;

    void AssignRegions(Coords coords, Plots plots) {
        var ownID = plots.Get(coords);
        var sameAsLeft = SameAsLeft();
        var sameAsUp = SameAsUp();
        if (sameAsLeft && sameAsUp)
            regions.Merge(coords);
        else if (sameAsLeft)
            regions.AssignLeft(coords);
        else if (sameAsUp)
            regions.AssignUp(coords);
        else
            regions.Create(coords);
        return;

        bool SameAsLeft() {
            if (coords.X == 0)
                return false;
            return plots.Get(new(coords.X - 1, coords.Y)) == ownID;
        }

        bool SameAsUp() {
            if (coords.Y == 0)
                return false;
            return plots.Get(new(coords.X, coords.Y - 1)) == ownID;
        }
    }
}

long ComputePrice(Region region) {
    var perimeter = region.GetCoords().Select(plots.GetFencesCount).Sum();
    var area = region.GetCoords().Count;
    return perimeter * area;
}

IEnumerable<string> GetLines() {
    while (true) {
        var line = Console.In.ReadLine();
        if (line == null)
            break;
        yield return line;
    }
}

class Plots(char[,] plots) {
    public readonly uint width = (uint)plots.GetLength(0);
    public readonly uint height = (uint)plots.GetLength(1);

    readonly char[,] plots = plots;
    readonly List<Region> regions = [];

    public IEnumerable<Coords> GetAllCoords() {
        for (var y = 0u; y < height; y++)
            for (var x = 0u; x < width; x++)
                yield return new Coords(x, y);
    }

    public char Get(Coords coords)
        => plots[coords.X, coords.Y];

    public int GetFencesCount(Coords coords) {
        var ownID = Get(coords);
        return Up(coords) + Right(coords) + Down(coords) + Left(coords);

        int Up(Coords coords) {
            var ownValue = 1;
            var left = coords.Left();
            if (coords.X > 0 && Get(left) == ownID)
                ownValue = EdgeUp(left) ? 0 : 1;
            return EdgeUp(coords) ? ownValue : 0;
        }

        bool EdgeUp(Coords coords) {
            if (coords.Y == 0)
                return true;
            return plots[coords.X, coords.Y - 1] != ownID;
        }

        int Right(Coords coords) {
            var ownValue = 1;
            var up = coords.Up();
            if (coords.Y > 0 && Get(up) == ownID)
                ownValue = EdgeRight(up) ? 0 : 1;
            return EdgeRight(coords) ? ownValue : 0;
        }

        bool EdgeRight(Coords coords) {
            if (coords.X == width - 1)
                return true;
            return plots[coords.X + 1, coords.Y] != ownID;
        }

        int Down(Coords coords) {
            var ownValue = 1;
            var left = coords.Left();
            if (coords.X > 0 && Get(left) == ownID)
                ownValue = EdgeDown(left) ? 0 : 1;
            return EdgeDown(coords) ? ownValue : 0;
        }

        bool EdgeDown(Coords coords) {
            if (coords.Y == height - 1)
                return true;
            return plots[coords.X, coords.Y + 1] != ownID;
        }

        int Left(Coords coords) {
            var ownValue = 1;
            var up = coords.Up();
            if (coords.Y > 0 && Get(up) == ownID)
                ownValue = EdgeLeft(up) ? 0 : 1;
            return EdgeLeft(coords) ? ownValue : 0;
        }

        bool EdgeLeft(Coords coords) {
            if (coords.X == 0)
                return true;
            return plots[coords.X - 1, coords.Y] != ownID;
        }
    }
}

record struct Coords(uint X, uint Y) {
    public Coords Up() => new(X, Y - 1);
    public Coords Left() => new(X - 1, Y);
}

class Region {
    readonly List<Coords> coords = [];

    public IReadOnlyList<Coords> GetCoords()
        => coords;

    public void Add(Coords coords)
        => this.coords.Add(coords);

    public void Incorporate(Region another)
        => coords.AddRange(another.GetCoords());
}

class Regions(Region[,] regions) {
    readonly Region[,] regionsMap = regions;
    readonly List<Region> allRegions = [];

    public IReadOnlyList<Region> GetAllRegions()
        => allRegions;

    public void Create(Coords coords) {
        var region = new Region();
        allRegions.Add(region);
        Assign(region, coords);
    }

    public void AssignLeft(Coords coords)
        => Assign(regionsMap[coords.X - 1, coords.Y], coords);

    public void AssignUp(Coords coords)
        => Assign(regionsMap[coords.X, coords.Y - 1], coords);

    public void Merge(Coords coords) {
        var up = regionsMap[coords.X, coords.Y - 1];
        var left = regionsMap[coords.X - 1, coords.Y];
        if (up != left) {
            allRegions.Remove(left);
            up.Incorporate(left);
            foreach (var leftCoords in left.GetCoords())
                regionsMap[leftCoords.X, leftCoords.Y] = up;
        }
        AssignUp(coords);
    }

    void Assign(Region region, Coords coords) {
        regionsMap[coords.X, coords.Y] = region;
        region.Add(coords);
    }
}
