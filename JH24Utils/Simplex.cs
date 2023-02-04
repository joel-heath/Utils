namespace JH24Utils;
public class Simplex
{
    public static decimal[][] OneStageSimplex(decimal[,] tableau)
    {
        bool maximised = false;
        while (!maximised)
        {
            if (Iteration(tableau)) { maximised = true; }
        }

        return ToBasicJagArray(tableau);
    }
    static int TopRow(decimal[,] table)
    {
        // returns -1 if top row has no negatives (maximised)
        (int col, decimal val) = (0, 0);
        for (int i = 1; i < table.GetLength(1) - 1; i++)
        {
            if (table[0, i] < val) { col = i; val = table[0, i]; }
        }

        return val == 0 ? -1 : col;
    }

    static int RatioTest(decimal[,] table, int var)
    {
        (int row, decimal val) min = (0, decimal.MaxValue);
        for (int i = 1; i < table.GetLength(0); i++)
        {
            if (table[i, var] != 0)
            {
                decimal val = table[i, table.GetLength(1) - 1] / table[i, var];
                if (val > 0 && val < min.val) min = (i, val);
            }
        }

        return min.row;
    }

    static bool Iteration(decimal[,] table)
    {
        // returns true if maximum value has been found
        int chosenCol = TopRow(table);
        if (chosenCol == -1) return true;

        int chosenRow = RatioTest(table, chosenCol);

        decimal dividend = table[chosenRow, chosenCol];

        // divide chosen row first
        for (int i = 0; i < table.GetLength(1); i++)
        {
            table[chosenRow, i] = table[chosenRow, i] / dividend;
        }

        // reduce all other values of variable to 0
        for (int r = 0; r < table.GetLength(0); r++)
        {
            if (r == chosenRow) continue;
            decimal scalar = -table[r, chosenCol];

            for (int c = 0; c < table.GetLength(1); c++)
            {
                table[r, c] = table[r, c] + (scalar * table[chosenRow, c]);
            }
        }

        return false;
    }

    static decimal[][] ToBasicJagArray(decimal[,] table)
    {
        // converts 2d array into jagged array, as well as removing any non-basic columns
        int rows = table.GetLength(0);
        int cols = table.GetLength(1);

        List<int> nonBasics = new();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols - 1; c++)
            {
                if (table[r, c] != 0 && table[r, c] != 1)
                {
                    if (!nonBasics.Contains(c)) nonBasics.Add(c);
                }
            }
        }

        decimal[][] output = new decimal[rows][];

        for (int r = 0; r < rows; r++)
        {
            output[r] = new decimal[cols - nonBasics.Count];

            // t is table index, c is output index 
            for (int t = 0, c = 0; t < cols; t++)
            {
                if (nonBasics.Contains(t)) continue;

                output[r][c++] = table[r, t];
            }
        }

        return output;
    }
}