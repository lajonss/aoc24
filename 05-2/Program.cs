var rules = ParseRules();
var sum = 0;
foreach (var report in GetReports()) {
    if (IsValid(report))
        continue;

    report.Sort(rules);
    sum += GetMiddlePage(report);
}
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


struct Rules : IComparer<int> {
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

    public int Compare(int x, int y) {
        if (data.TryGetValue(x, out var xafters)) {
            if (xafters.Contains(y))
                return -1;
        }

        if (data.TryGetValue(y, out var yafters)) {
            if (yafters.Contains(x))
                return 1;
        }

        return 0;
    }
}

struct Report {
    readonly List<int> data;

    public int Count => data.Count;

    Report(List<int> data) {
        this.data = data;
    }

    public static Report Create(string line)
        => new(line.Split(',').Select(int.Parse).ToList());

    public int Get(int index)
        => data[index];

    public void Sort(Rules rules) {
        data.Sort(rules);
    }
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
