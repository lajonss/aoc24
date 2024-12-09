var rules = ParseRules();
var sum = GetReports().Where(IsValid).Select(GetMiddlePage).Sum();
Console.WriteLine(sum.ToString());

IEnumerable<string> GetParagraphLines() {
    while (true) {
        var line = Console.In.ReadLine();
        if (string.IsNullOrEmpty(line))
            yield break;

        yield return line;
    }
}

Rules ParseRules() {
    var rules = new Rules();
    foreach (var line in GetParagraphLines())
        rules.Add(line);
    return rules;
}

IEnumerable<Report> GetReports()
    => GetParagraphLines().Select(Report.Create);

bool IsValid(Report report) {
    for (var i = 1; i < report.Count; i++) {
        var ruleset = rules.For(report.Get(i));
        for (var j = 0; j < i; j++)
            if (ruleset.ShouldAppearBefore(report.Get(j)))
                return false;
    }
    return true;
}

int GetMiddlePage(Report report)
    => report.Get(report.Count / 2);


struct Rules {
    readonly Dictionary<int, List<int>> data = [];

    public Rules() { }

    public RulesContext For(int value) {
        if (data.TryGetValue(value, out var list))
            return new(list);
        return RulesContext.Empty;
    }

    public void Add(string line) {
        var split = line.Split('|');
        Add(int.Parse(split[0]), int.Parse(split[1]));
    }

    void Add(int before, int after) {
        if (!data.TryGetValue(before, out var list))
            data[before] = list = [];
        list.Add(after);
    }
}

struct Report {
    readonly IReadOnlyList<int> data;

    public int Count => data.Count;

    Report(IReadOnlyList<int> data) {
        this.data = data;
    }

    public static Report Create(string line)
        => new(line.Split(',').Select(int.Parse).ToArray());

    public int Get(int index)
        => data[index];
}

struct RulesContext {
    readonly IReadOnlyList<int> data;

    public static RulesContext Empty => new([]);

    public RulesContext(IReadOnlyList<int> data) {
        this.data = data;
    }

    public bool ShouldAppearBefore(int value)
        => data.Contains(value);
}
