List<int> left = new();
List<int> right = new();

ParseInput();

var similarity = left.Select(SimilarityScore).Sum();
Console.WriteLine(similarity.ToString());

void ParseInput() {
    foreach (var line in GetLines())
        ParseLine(line);
    return;

    IEnumerable<string> GetLines() {
        string? line;
        while ((line = Console.In.ReadLine()) != null)
            yield return line;
    }

    void ParseLine(string line) {
        if (string.IsNullOrWhiteSpace(line))
            return;

        var elements = line.Split(separator: ' ', options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        left.Add(int.Parse(elements[0]));
        right.Add(int.Parse(elements[1]));
    }
}

int SimilarityScore(int value) {
    return value * right.Count(r => r == value);
}