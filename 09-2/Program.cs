using System.Security.Authentication;

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
            yield return new Block { fileID = fileID, taken = taken, fileLength = length };
        if (taken)
            fileID++;
        taken = !taken;
    }
}

void Compress(List<Block> filesystem) {
    var targetIndex = 0;
    var sourceIndex = filesystem.Count - 1;
    var lastMovedFileID = int.Max;
    while (sourceIndex > 0) {
        if (targetIndex >= sourceIndex) {
            targetIndex = 0;
            sourceIndex--;
        } else if (filesystem[targetIndex].taken)
            targetIndex++;
        else if (!filesystem[sourceIndex].taken)
            sourceIndex--;
        else if (filesystem[sourceIndex].fileLength > filesystem[targetIndex].fileLength)
            targetIndex++;
        else {
            var sourceLength = filesystem[sourceIndex].fileLength;
            var leftover = filesystem[targetIndex].fileLength - sourceLength;
            for (var i = 0; i < sourceLength; i++) {
                filesystem[targetIndex + i] = filesystem[sourceIndex - i];
                filesystem[sourceIndex - i] = default;
            }
            for (var i = sourceLength; i < sourceLength + leftover; i++)
                filesystem[targetIndex + i] = new Block { fileLength = leftover };
            sourceIndex -= sourceLength;
            targetIndex = 0;
        }
    }
}

ulong GetChecksum(List<Block> filesystem) {
    var sum = 0ul;
    for (var i = 0; i < filesystem.Count; i++) {
        var block = filesystem[i];
        if (block.taken)
            sum += (ulong)(block.fileID * i);
    }
    return sum;
}

// void Print(List<Block> filesystem)
//     => Console.WriteLine(string.Join("", filesystem.Select(x => x.taken ? x.fileID.ToString() : ".")));

struct Block {
    public uint fileID;
    public bool taken;
    public int fileLength;
}