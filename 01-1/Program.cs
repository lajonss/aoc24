List<int> left = new();
List<int> right = new();

ParseInput();

var sum = left.Order().Zip(right.Order(), Distance).Sum();
Console.WriteLine(sum.ToString());


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

int Distance(int a, int b)
    => Math.Abs(a - b);

