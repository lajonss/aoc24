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
    var maskSize = Pow(3, parts.Length - 1);
    for (Int128 mask = 0; mask < maskSize; mask++) {
        var localSum = parts[0];
        var maskCopy = mask;
        for (var i = 1; i < parts.Length; i++) {
            switch ((int)(maskCopy % 3)) {
                case 0:
                    localSum += parts[i];
                    break;
                case 1:
                    localSum *= parts[i];
                    break;
                case 2:
                    localSum = Concat(localSum, parts[i]);
                    break;
                default:
                    throw new NotImplementedException();
            }
            maskCopy /= 3;
        }

        if (localSum == target) {
            sum += target;
            return;
        }
    }
}

long Concat(long a, long b) {
    var bCopy = b;
    while (bCopy > 0) {
        bCopy /= 10;
        a *= 10;
    }
    return a + b;
}

int Pow(int number, int power) {
    var acc = 1;
    for (var i = 0; i < power; i++)
        acc *= number;
    return acc;
}

enum Operator {
    Add,
    Multiply,
    Concat
}