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

    var elements = line.Split(
        separator: ' ',
        options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
    ).Select(int.Parse).ToArray();

    var differencesCount = elements.Length - 1;
    var differences = new List<int>(differencesCount);
    for (var i = 0; i < differencesCount; i++)
        differences.Add(elements[i + 1] - elements[i]);

    return Valid(Rising) || Valid(Falling);

    bool Valid(Predicate<int> predicate) {
        var invalid = Not(predicate);
        var invalidIndex = differences.FindIndex(invalid);
        if (invalidIndex == -1)
            return true;
        if (invalidIndex == 0)
            if (differences.FindIndex(startIndex: 1, match: invalid) == -1)
                return true;
        if (invalidIndex > 0 && predicate(differences[invalidIndex] + differences[invalidIndex - 1]))
            if (differences.
            FindIndex(startIndex: invalidIndex + 1, match: invalid) == -1)
                return true;
        if (invalidIndex == differences.Count - 1)
            return true;
        if (invalid(differences[invalidIndex] + differences[invalidIndex + 1]))
            return false;
        return differences.FindIndex(startIndex: invalidIndex + 2, match: invalid) == -1;
    }
}

bool Rising(int difference)
    => difference >= 1 && difference <= 3;

bool Falling(int difference)
    => difference <= -1 && difference >= -3;

Predicate<T> Not<T>(Predicate<T> predicate)
    => t => !predicate(t);
