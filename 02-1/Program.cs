var safeCount = GetLines().Where(IsSafe).Count();
Console.WriteLine(safeCount.ToString());

IEnumerable<string> GetLines() {
    string? line;
    while ((line = Console.In.ReadLine()) != null)
        yield return line;
}

bool IsSafe(string line) {
    if (string.IsNullOrWhiteSpace(line))
        return false;

    var elements = line.Split(separator: ' ', options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var previous = Get(0);
    var previousDirection = 0;
    for (var i = 1; i < elements.Length; i++) {
        var current = Get(i);
        var diff = current - previous;
        var currentDirection = Math.Sign(diff);
        if (IsDirectionChanged())
            return false;
        if (!IsDistanceValid())
            return false;
        previous = current;
        previousDirection = currentDirection;

        bool IsDirectionChanged() {
            return previousDirection + currentDirection == 0;
        }

        bool IsDistanceValid() {
            var distance = Math.Abs(diff);
            return distance >= 1 && distance <= 3;
        }
    }
    return true;

    int Get(int index)
        => int.Parse(elements[index]);
}