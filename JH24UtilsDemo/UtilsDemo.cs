using JH24Utils;
using JH24Utils.Matrices;
using JH24Utils.Statistics;
using static JH24Utils.Tree;

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

    static void DrawMatrix(Matrix points, int width = 1)
    {
        for (int i = 0; i < points.Columns; i++)
        {
            Console.SetCursorPosition((int)points[0, i] - width + 1, (int)points[1, i] - width + 1);
            var x = Console.CursorLeft;
            for (int h = 0; h < width; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    Console.Write('*');
                }
                Console.CursorTop++;
                Console.CursorLeft = x;
            }
        }
    }
    static void TextDemo()
    {
        Matrix bitmap = new(new double[,]
        {
            { 0, 0, 0, 0, 0, 1, 2, 3, 4, 4, 4, 4, 4,    6, 6, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9,    11, 11, 11, 11, 11, 12, 13, 14,    16, 16, 16, 16, 16, 17, 18, 19,   21, 21, 21, 21, 21, 22, 22, 23, 23, 24, 24, 24, 24, 24  },
            { 0, 1, 2, 3, 4, 2, 2, 2, 0, 1, 2, 3, 4,    0, 1, 2, 3, 4, 0, 2, 4, 0, 2, 4, 0, 2, 4,    0,  1,  2,  3,  4,  4,  4,  4,     0,  1,  2,  3,  4,  4,  4,  4,    0,  1,  2,  3,  4,  0,  4,  0,  4,  0,  1,  2,  3,  4   }
        });

        const int SHEAR_SCALE_FACTOR = 3;

        Matrix shear3D = new(new double[,]
        {
            { 1, SHEAR_SCALE_FACTOR },
            { 0, 1 }
        });


        Console.WriteLine("Fullscreen your console window and zoom out a lot. Then press any key to continue.");
        Console.ReadKey();
        var big = bitmap.EnlargeArea(10).Translate(5, 5);
        var perspective = shear3D * big;

        Console.ForegroundColor = ConsoleColor.DarkGray;
        DrawMatrix(perspective.Translate(1, 1));


        var height = big.GetRow(1).Max() - big.GetRow(1).Min();
        big = big.Translate(height * SHEAR_SCALE_FACTOR, 0);


        var border = big.Translate(1, 1);
        Console.ForegroundColor = ConsoleColor.White;
        DrawMatrix(border.Translate(1, 1), width: 3);


        Console.ForegroundColor = ConsoleColor.Red;
        DrawMatrix(big.Translate(1, 1));



        Matrix shear3D2 = new(new double[,]
        {
            { 1, -SHEAR_SCALE_FACTOR },
            { 0, 1 }
        });

        Console.ReadKey();
        big = bitmap.EnlargeArea(10).Translate(5, 5);

        perspective = shear3D2 * big;

        Console.ForegroundColor = ConsoleColor.DarkGray;
        DrawMatrix(perspective.Translate(700, 1));


        height = big.GetRow(1).Max() - big.GetRow(1).Min();
        big = big.Translate(-height * SHEAR_SCALE_FACTOR, 0);


        border = big.Translate(1, 1);
        Console.ForegroundColor = ConsoleColor.White;
        DrawMatrix(border.Translate(700, 1), 3);


        Console.ForegroundColor = ConsoleColor.Blue;
        DrawMatrix(big.Translate(700, 1));
    }

    static void Pathfind()
    {
        HashSet<Node> tree = CreateGraph("1,2:3,5:8" + "\r\n"
          + "2,1:3,4:5,3:4" + "\r\n"
          + "3,2:4,7:7,6:1" + "\r\n"
          + "4,2:5,7:3" + "\r\n"
          + "5,1:8,6:1" + "\r\n"
          + "6,5:1,3:1,7:9" + "\r\n"
          + "7,6:9,3:7,4:3");

        var result = Tree.Dijkstras(tree.First());
        int i = 0;
        foreach (var item in result)
        {
            Console.WriteLine($"Node {i++}: {item.Value}");
        }
    }

    static void SimplexOneStage()
    {
        Console.WriteLine("The Simplex Algorithm");

        Console.Write("Enter the variable names (separated by a space): ");
        string[] vars = (Console.ReadLine() ?? "x").Split(" ");

        Console.Write("Enter the number of constraints: ");
        int constCount = int.Parse(Console.ReadLine() ?? "1");

        decimal[,] tableau = new decimal[constCount + 1, vars.Length + 2];

        Console.WriteLine("(Enter each coefficient of each variable in the order that you entered the variables initially.)");
        Console.WriteLine("Enter the coefficients of the objective function.");
        Console.Write("P = ");
        tableau[0, 0] = 1;

        for (int i = 0; i < vars.Length; i++)
        {
            tableau[0, i + 1] = -ReadNum();
            Console.Write($"{vars[i]}{(i == vars.Length - 1 ? "" : " + ")}");
        }

        Console.WriteLine("\nNow enter each constraint in the same way.");

        for (int i = 1; i < tableau.GetLength(0); i++)
        {
            Console.Write($"Constraint {i}: ");
            for (int j = 1; j < tableau.GetLength(1); j++)
            {
                tableau[i, j] = ReadNum();

                if (j != vars.Length + 1)
                {
                    Console.Write($"{vars[j - 1]}{(j < vars.Length ? " + " : " = ")}");
                }
            }
            Console.WriteLine();
        }

        decimal[][] final = Simplex.OneStageSimplex(tableau);

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

    static readonly string[] funcs = { "Simplex", "Path(find)", "Matrix" };
    static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Enter 'exit' to exit, and 'list' to list functions.");
        Console.ForegroundColor = ConsoleColor.White;
        bool finished = false;
        while (!finished)
        {
            Console.Write("Which utility would you like to demo? ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string uInput = Console.ReadLine() ?? "Simplex";
            Console.ForegroundColor = ConsoleColor.White;

            switch (uInput.ToLower())
            {
                case "simplex": SimplexOneStage(); break;
                case "path": Pathfind(); break;
                case "matrix": TextDemo(); break;
                case "list": ListFuncs(); break;
                case "exit": finished = true; break;
            }
        }
    }
}