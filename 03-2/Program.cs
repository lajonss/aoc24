var mul = new Mul();
var operations = new Operation[] { new Do(mul), new Dont(mul), mul };
foreach (var character in GetInput())
    foreach (var operation in operations)
        operation.Advance(character);
Console.WriteLine(mul.Sum.ToString());

IEnumerable<char> GetInput() {
    while (true) {
        var value = Console.Read();
        if (value < 0)
            yield break;
        yield return Convert.ToChar(value);
    }
}

abstract class Operation {
    readonly string name;

    Phase phase;

    protected Operation(string name) {
        this.name = name;
        phase = ParseName();
    }

    public void Advance(char character) {
        phase = phase(character);
    }

    public void Disable()
        => phase = Noop;

    public void Enable()
        => phase = ParseName();

    protected Phase ParseName(int pointer = 0) {
        return character => {
            if (pointer == name.Length) {
                if (character == '(')
                    return ParseArguments;
            } else if (character == name[pointer])
                return ParseName(pointer + 1);
            return ParseName();
        };
    }

    Phase Noop(char character)
        => Noop;

    protected abstract Phase ParseArguments(char character);

    protected delegate Phase Phase(char character);
}

class Do : Operation {
    readonly Mul mul;

    public Do(Mul mul) : base("do") {
        this.mul = mul;
    }

    protected override Phase ParseArguments(char character) {
        if (character == ')')
            mul.Enable();
        return ParseName();
    }
}

class Dont : Operation {
    readonly Mul mul;

    public Dont(Mul mul) : base("don't") {
        this.mul = mul;
    }

    protected override Phase ParseArguments(char character) {
        if (character == ')')
            mul.Disable();
        return ParseName();
    }
}

class Mul : Operation {
    public int Sum => sum;

    int sum;

    public Mul() : base("mul") { }

    protected override Phase ParseArguments(char character)
        => ParseNumberFirst()(character);

    Phase ParseNumberFirst(int number = 0) {
        return character => {
            if (character == ',')
                return ParseNumberSecond(firstNumber: number);
            if (char.IsDigit(character) && number < 1000)
                return ParseNumberFirst(Advance(number, character));
            return ParseName();
        };
    }

    Phase ParseNumberSecond(int firstNumber, int secondNumber = 0) {
        return character => {
            if (character == ')')
                sum += firstNumber * secondNumber;
            else if (char.IsDigit(character) && secondNumber < 1000)
                return ParseNumberSecond(firstNumber, Advance(secondNumber, character));
            return ParseName();
        };
    }

    int Advance(int sum, char digit) {
        return sum * 10 + (int)char.GetNumericValue(digit);
    }
}
