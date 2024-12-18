char[] SEPARATORS = [':', ' '];

var sum = 0L;
foreach (var line in ReadLines())
    Check(line);
Console.WriteLine(sum);

IEnumerable<string> ReadLines() {
    while (true) {
        var line = Console.In.ReadLine();
        if (string.IsNullOrEmpty(line))
            yield break;

        yield return line;
    }
}

void Check(string line) {
    var elements = line.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
    var target = elements[0];
    var parts = elements[1..];
    var maskSize = 1 << (parts.Length - 1);
    for (Int128 mask = 0; mask < maskSize; mask++) {
        var localSum = parts[0];
        for (var i = 1; i < parts.Length; i++)
            if ((mask & (1 << (i - 1))) != 0)
                localSum *= parts[i];
            else
                localSum += parts[i];
        if (localSum == target) {
            sum += target;
            return;
        }
    }
}
