var sum = 0;
Phase phase = WaitForM;
foreach (var character in GetInput())
    phase = phase(character);
Console.WriteLine(sum.ToString());

IEnumerable<char> GetInput() {
    while (true) {
        var value = Console.Read();
        if (value < 0)
            yield break;
        yield return Convert.ToChar(value);
    }
}

Phase WaitForM(char character) {
    if (character == 'm')
        return WaitForU;
    return WaitForM;
}

Phase WaitForU(char character) {
    if (character == 'u')
        return WaitForL;
    return WaitForM;
}

Phase WaitForL(char character) {
    if (character == 'l')
        return WaitForOpeningBrace;
    return WaitForM;
}

Phase WaitForOpeningBrace(char character) {
    if (character == '(')
        return ParseNumberFirst();
    return WaitForM;
}

Phase ParseNumberFirst(int number = 0) {
    return (character) => {
        if (character == ',')
            return ParseNumberSecond(firstNumber: number);
        if (char.IsDigit(character) && number < 1000)
            return ParseNumberFirst(Advance(number, character));
        return WaitForM;
    };
}

Phase ParseNumberSecond(int firstNumber, int secondNumber = 0) {
    return (character) => {
        if (character == ')')
            sum += firstNumber * secondNumber;
        else if (char.IsDigit(character) && secondNumber < 1000)
            return ParseNumberSecond(firstNumber, Advance(secondNumber, character));
        return WaitForM;
    };
}

int Advance(int sum, char digit) {
    return sum * 10 + (int)char.GetNumericValue(digit);
}

delegate Phase Phase(char character);
