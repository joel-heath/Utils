namespace JH24Utils.Numeracy;
public partial class ShuntingYard
{
    public static Queue<Token> InfixToPostfix(Queue<Token> tokens)
    {
        Stack<Operator> operators = new();
        Queue<Token> output = new();

        foreach (Token t in tokens)
        {
            if (t.Type == TokenType.Number)
            {
                output.Enqueue(t);
                continue;
            }

            Operator op = t.Op;

            if (op == Operator.OpenParenthesis || Functions.Contains(op))
            {
                operators.Push(op);
                continue;
            }

            if (op == Operator.CloseParenthesis)
            {
                while (operators.TryPeek(out Operator o) && o != Operator.OpenParenthesis)
                {
                    output.Enqueue(new Token(operators.Pop()));
                }

                operators.Pop(); // Discard the open parenthesis

                if (operators.TryPeek(out Operator s) && Functions.Contains(s))
                {
                    output.Enqueue(new Token(operators.Pop()));
                }
                continue;
            }

            if (op == Operator.Comma)
            {
                while (operators.TryPeek(out Operator o) && o != Operator.OpenParenthesis)
                {
                    output.Enqueue(new Token(operators.Pop()));
                }
                continue;
            }

            int currentP = Precedence[op];
            int stackP;

            while (operators.TryPeek(out Operator s) && s != Operator.OpenParenthesis && ((stackP = Precedence[s]) > currentP || stackP == currentP && Associativity[s]))
            {
                output.Enqueue(new Token(operators.Pop()));
            }

            operators.Push(op);
        }

        while (operators.TryPop(out Operator s))
        {
            output.Enqueue(new Token(s));
        }

        return output;
    }
}