namespace JH24Utils.Matrices;
public static class MatrixExtensions
{
    public static Matrix Translate(this Matrix points, params double[] units)
    {
        Matrix translation = new(points.Rows, points.Columns, units.Select(u => Enumerable.Repeat(u, points.Columns)));
        return translation + points;
    }
    public static Matrix Enlarge(this Matrix points, double factor) => factor * Matrix.Identity(points.Rows) * points;
    public static Matrix Stretch(this Matrix points, params double[] factors)
    {
        Matrix m = Matrix.Zero(points.Rows);
        for (int i = 0; i < points.Rows; i++) { m[i, i] = factors[i]; }

        return m;
    }
    public static Matrix Rescale(this Matrix points, params double[] upperBounds)
    {
        double[] oldUpperBounds = Enumerable.Repeat(double.MinValue, upperBounds.Length).ToArray();

        for (int i = 0; i < points.Columns; i++)
        {
            for (int j = 0; j < oldUpperBounds.Length; j++)
            {
                if (points[j, i] > oldUpperBounds[j]) oldUpperBounds[j] = points[j, i];
            }
        }

        return points.Stretch(upperBounds.Select((b, i) => b / oldUpperBounds[i]).ToArray()) * points;
    }

    public static Matrix EnlargeArea(this Matrix points, int factor)
    {
        Matrix enlarged = points.Enlarge(factor);

        Matrix widened = new(points.Rows, points.Columns * factor * factor);
        for (int i = 0; i < enlarged.Columns; i++)
        {
            var col = enlarged.GetCol(i).ToArray();
            for (int y = 0; y < factor; y++)
            {
                for (int x = 0; x < factor; x++)
                {
                    widened[0, i * factor * factor + y * factor + x] = col[0] + x - factor / 2;
                    widened[1, i * factor * factor + y * factor + x] = col[1] + y - factor / 2;
                }
            }
        }

        return widened;
    }
}