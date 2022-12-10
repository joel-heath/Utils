namespace JH24UtilsDemo;

internal class UtilsDemo
{
    static decimal ReadNum()
    {
        int reset = Console.CursorLeft; // for resetting newline from user input
        string rawUInput = Console.ReadLine() ?? "0";
        Console.CursorLeft = reset + rawUInput.Length;
        Console.CursorTop--;

        return decimal.Parse(rawUInput);
    }

    static void SimplexOneStage()
    {
        Console.WriteLine("The Simplex Algorithm");

        Console.Write("Enter the variable names (seperated by a space): ");
        string[] vars = (Console.ReadLine() ?? "x").Split(" ");

        Console.Write("Enter the number of constraints: ");
        int constCount = int.Parse(Console.ReadLine() ?? "1");

        decimal[,] tableux = new decimal[constCount + 1, vars.Length + 2];

        Console.WriteLine("(Enter each coefficient of each variable in the order that you entered the variables initially.)");
        Console.WriteLine("Enter the coefficients of the objective function.");
        Console.Write("P = ");
        tableux[0, 0] = 1;

        for (int i = 0; i < vars.Length; i++)
        {
            tableux[0, i + 1] = -ReadNum();
            Console.Write($"{vars[i]}{(i == vars.Length - 1 ? "" : " + ")}");
        }

        Console.WriteLine("\nNow enter each constraint in the same way.");

        for (int i = 1; i < tableux.GetLength(0); i++)
        {
            Console.Write($"Constraint {i}: ");
            for (int j = 1; j < tableux.GetLength(1); j++)
            {
                tableux[i, j] = ReadNum();

                if (j != vars.Length + 1)
                {
                    Console.Write($"{vars[j - 1]}{(j < vars.Length ? " + " : " = ")}");
                }
            }
            Console.WriteLine();
        }

        decimal[][] final = JH24Utils.Simplex.OneStageSimplex(tableux);

        string[] trueVars = new string[] { "P" }.Concat(vars).ToArray();

        // if it were x + y = 5, Item1 would be {x, y}, Item2 would be 5
        (string[], decimal)[] output = new (string[], decimal)[final.Length];

        for (int r = 0; r < final.Length; r++)
        {
            string[] rowVars = final[r].SkipLast(1).Select((v, i) => (v, trueVars[i])).Where(t => t.v > 0.0000000001M || t.v < -0.0000000001M).Select(t => t.Item2).ToArray();

            output[r] = (rowVars, final[r][^1]);
        }

        for (int i = 0; i < output.Length; i++)
        {
            Console.WriteLine($"{string.Join(" + ", output[i].Item1)} = {output[i].Item2:0.#####}");
        }

        Console.ReadKey(true);
    }

    static void ListFuncs()
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        for (int i = 0; i < funcs.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {funcs[i]}");
        }
        Console.ForegroundColor = ConsoleColor.White;
    }

    static readonly string[] funcs = { "Simplex" };
    static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Enter 'exit' to exit, and 'list' to list functions.");
        Console.ForegroundColor = ConsoleColor.White;
        bool finished = false;
        while (!finished)
        {
            Console.Write("Which algorithm would you like to try? ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string uInput = Console.ReadLine() ?? "Simplex";
            Console.ForegroundColor = ConsoleColor.White;

            switch (uInput.ToLower())
            {
                case "simplex": SimplexOneStage(); break;
                case "list": ListFuncs(); break;
                case "exit": finished = true; break;
            }
        }
    }
}