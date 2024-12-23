const int BLINK_COUNT = 25;

var stones = ParseInput();
for (var i = 0; i < BLINK_COUNT; i++)
    Blink();
Console.WriteLine(stones.Count);

LinkedList<ulong> ParseInput() {
    var output = new LinkedList<ulong>();
    var input = Console.In.ReadToEnd().Split(' ', StringSplitOptions.RemoveEmptyEntries);
    foreach (var token in input)
        output.AddLast(ulong.Parse(token));
    return output;
}

void Blink() {
    var node = stones.First;
    while (node != null) {
        ApplyRules(node);
        node = node.Next;
    }
}

void ApplyRules(LinkedListNode<ulong> node) {
    if (node.Value == 0) {
        node.Value = 1;
        return;
    }

    var digitsCount = GetDigitsCount();
    if (digitsCount % 2 == 0) {
        var split = 10ul;
        for (var i = 1; i < digitsCount / 2; i++)
            split *= 10;
        stones.AddBefore(node, node.Value / split);
        node.Value = node.Value % split;
        return;
    }

    node.Value = node.Value * 2024;
    return;

    uint GetDigitsCount() {
        var value = node.Value;
        var digitsCount = 1u;
        while (value > 9) {
            value /= 10;
            digitsCount++;
        }
        return digitsCount;
    }
}
