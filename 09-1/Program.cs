var filesystem = ParseInput(ReadInput().Select(ToInt)).ToList();
Compress(filesystem);
Console.WriteLine(GetChecksum(filesystem).ToString());

IEnumerable<char> ReadInput() {
    while (true) {
        const int SIZE = 100;
        var buffer = new char[SIZE];
        var readCount = Console.In.ReadBlock(buffer);
        for (var i = 0; i < readCount; i++)
            yield return buffer[i];
        if (readCount < SIZE)
            yield break;
    }
}

int ToInt(char c)
    => c - '0';

IEnumerable<Block> ParseInput(IEnumerable<int> input) {
    var taken = true;
    var fileID = 0u;
    foreach (var length in input) {
        for (var i = 0; i < length; i++)
            yield return new Block { fileID = fileID, taken = taken };
        if (taken)
            fileID++;
        taken = !taken;
    }
}

void Compress(List<Block> filesystem) {
    var targetIndex = 0;
    var sourceIndex = filesystem.Count - 1;
    while (targetIndex < sourceIndex) {
        if (filesystem[targetIndex].taken)
            targetIndex++;
        else if (!filesystem[sourceIndex].taken)
            sourceIndex--;
        else {
            filesystem[targetIndex] = filesystem[sourceIndex];
            filesystem[sourceIndex] = default;
        }
    }
}

ulong GetChecksum(List<Block> filesystem) {
    var sum = 0ul;
    for (var i = 0; i < filesystem.Count; i++) {
        var block = filesystem[i];
        if (!block.taken)
            return sum;

        sum += (ulong)(block.fileID * i);
    }
    return sum;
}

struct Block {
    public uint fileID;
    public bool taken;
}
