namespace JH24Utils.Numeracy;

public enum Operator
{
    Add, Subtract, Multiply, Divide, Modulo,
    Power,
    Sin, Cos, Tan,
    Max, Min,
    Comma,
    OpenParenthesis, CloseParenthesis
}

partial class ShuntingYard
{
    private static readonly Dictionary<string, Operator> Opcodes = new()
    {
        { "+", Operator.Add },
        { "-", Operator.Subtract },
        { "×", Operator.Multiply },
        { "*", Operator.Multiply },
        { "÷", Operator.Divide },
        { "/", Operator.Divide },
        { "%", Operator.Modulo },
        { "^", Operator.Power },
        { "sin", Operator.Sin },
        { "cos", Operator.Cos },
        { "tan", Operator.Tan },
        { "max", Operator.Max },
        { "min", Operator.Min },
        { ",", Operator.Comma },
        { "(", Operator.OpenParenthesis },
        { ")", Operator.CloseParenthesis },
    };

    private static readonly Dictionary<Operator, int> Precedence = new()
    {
        { Operator.Add, 0 },
        { Operator.Subtract, 0 },
        { Operator.Multiply, 1 },
        { Operator.Divide, 1 },
        { Operator.Modulo, 1 },
        { Operator.Power, 2 },
        { Operator.OpenParenthesis, 3 },
        { Operator.CloseParenthesis, 3 }
    };

    private static readonly Dictionary<Operator, bool> Associativity = new() // True is left-associativity, False is right-associativity
    {
        { Operator.Add, true },
        { Operator.Subtract, true },
        { Operator.Multiply, true },
        { Operator.Divide, true },
        { Operator.Modulo, true },
        { Operator.Power, false }
    };

    private static readonly HashSet<Operator> Functions = new() { Operator.Sin, Operator.Cos, Operator.Tan, Operator.Max, Operator.Min };
}