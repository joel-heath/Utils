namespace JH24Utils.Numeracy;

partial class ShuntingYard
{
    public enum TokenType { Operator, Number }
    public readonly record struct Token
    {
        private readonly Operator? _operator;
        private readonly double? _number;

        public Operator Op
        {
            get
            {
                if (_operator is null) throw new InvalidCastException();
                return _operator.Value;
            }
        }
        public double Value
        {
            get
            {
                if (_number is null) throw new InvalidCastException();
                return _number.Value;
            }
        }

        public TokenType Type { get => _number is null ? TokenType.Operator : TokenType.Number; }

        public Token(Operator op)
        {
            _operator = op;
            _number = null;
        }
        public Token(double num)
        {
            _number = num;
            _operator = null;
        }

        public override string ToString()
        {
            Operator? op = _operator;
            return op is null ? _number.ToString()! : Opcodes.First(o => o.Value == op)!.Key;
        }

        private static readonly HashSet<char> _decimalTokens = new() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
        public static HashSet<char> DecimalTokens { get => _decimalTokens; }

        public static Queue<Token> Tokenize(string input)
        {
            var queue = new Queue<Token>();

            string current = string.Empty;
            bool isNumber = false;

            foreach (char c in input.Where(c => c != ' '))
            {
                if (DecimalTokens.Contains(c))
                {
                    current += c;

                    isNumber = true;
                    continue;
                }

                if (isNumber)
                {
                    queue.Enqueue(new Token(double.Parse(current)));
                    isNumber = false;
                    current = string.Empty;
                }

                current += c;

                if (Opcodes.TryGetValue(current, out Operator op))
                {
                    current = string.Empty;
                    queue.Enqueue(new Token(op)); // DOESNT ALLOW sinh()
                }
            }

            if (isNumber)
            {
                queue.Enqueue(new Token(double.Parse(current)));
            }

            return queue;
        }
    }
}