const int BLINK_COUNT = 75;

var blinker = new Blinker(ParseInput());
for (var i = 0; i < BLINK_COUNT; i++)
    blinker.Blink();
Console.WriteLine(blinker.GetCount());

Stones ParseInput() {
    var output = new Stones();
    var input = Console.In.ReadToEnd().Split(' ', StringSplitOptions.RemoveEmptyEntries);
    foreach (var token in input)
        output.Add(ulong.Parse(token));
    return output;
}

class Stones {
    readonly Dictionary<ulong, ulong> data = new();

    public void Add(ulong key, ulong count = 1) {
        if (!data.TryGetValue(key, out var value))
            value = 0;
        value += count;
        data[key] = value;
    }

    public ulong GetCount() {
        var sum = 0ul;
        foreach (var value in data.Values)
            sum += value;
        return sum;
    }

    public void Clear()
        => data.Clear();

    public IEnumerable<KeyValuePair<ulong, ulong>> GetData()
        => data;
}

class Blinker(Stones stones) {
    Stones current = stones;
    Stones copy = new();

    public void Blink() {
        copy.Clear();
        foreach (var kvp in current.GetData())
            Blink(value: kvp.Key, count: kvp.Value);
        (copy, current) = (current, copy);
        return;

        void Blink(ulong value, ulong count) {
            if (value == 0) {
                copy.Add(1, count);
                return;
            }

            var digitsCount = GetDigitsCount();
            if (digitsCount % 2 == 0) {
                var split = 10ul;
                for (var i = 1; i < digitsCount / 2; i++)
                    split *= 10;

                copy.Add(value / split, count);
                copy.Add(value % split, count); ;
                return;
            }

            copy.Add(value * 2024, count);
            return;

            uint GetDigitsCount() {
                var valueCopy = value;
                var digitsCount = 1u;
                while (valueCopy > 9) {
                    valueCopy /= 10;
                    digitsCount++;
                }
                return digitsCount;
            }
        }
    }

    public ulong GetCount()
        => current.GetCount();
}
