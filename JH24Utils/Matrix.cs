using System;
using System.Linq;
using System.Text;

namespace JH24Utils;
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
                    widened[0, i * factor * factor + y * factor + x] = col[0] + x - (factor / 2);
                    widened[1, i * factor * factor + y * factor + x] = col[1] + y - (factor / 2);
                }
            }
        }

        return widened;
    }
}
public class DimensionLessThanOneException : Exception { }
public class MatrixNotSquareException : Exception { }
public class MatrixSingularException : Exception { }
public class Matrix
{
    private readonly double[,] values;
    public readonly int Rows;
    public readonly int Columns;
    public readonly bool Square;
    public double Determinant
    {
        get
        {
            if (!Square) throw new MatrixNotSquareException();
            if (Rows == 1) return values[0, 0];
            if (Rows == 2) return values[0, 0] * values[1, 1] - values[0, 1] * values[1, 0];

            double determinant = 0;
            double[] topRow = GetRow(0).ToArray();
            for (int i = 0; i < Columns; i++)
            {
                double scalar = topRow[i];
                Matrix minor = Minor(0, i);

                if (i % 2 == 0)
                    determinant += scalar * minor.Determinant;
                else
                    determinant -= scalar * minor.Determinant;
            }

            return determinant;
        }
    }

    public Matrix Transpose
    {
        get
        {
            if (!Square) throw new MatrixNotSquareException();
            Matrix m = new(Rows);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    m[i, j] = values[j, i];
                }
            }

            return m;
        }
    }

    public Matrix Adjacent
    {
        get
        {
            if (!Square) throw new MatrixNotSquareException();
            Matrix t = Transpose;
            Matrix m = new(Rows);

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    double minor = t.Minor(i, j).Determinant;
                    int cofactor = (i % 2 == 0) ? (j % 2 == 0 ? 1 : -1) : (j % 2 == 0 ? -1 : 1);
                    m[i, j] = cofactor * minor;
                }
            }

            return m;
        }
    }

    public Matrix Inverse
    {
        get
        {
            double det = Determinant;
            if (det == 0) throw new MatrixSingularException();
            return (1 / det) * Adjacent;
        }
    }

    public double this[int i, int j]
    {
        get => values[i, j];
        set => values[i, j] = value;
    }

    public Matrix(IEnumerable<IEnumerable<double>> vals) : this(vals.Count(), vals.First().Count(), vals) { }
    public Matrix(int rows, int cols, IEnumerable<IEnumerable<double>> vals)
    {
        Rows = rows;
        Columns = cols;
        Square = Rows == Columns;
        values = new double[Rows, Columns];

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                values[i, j] = vals.ElementAt(i).ElementAt(j);
            }
        }
    }

    public Matrix(int rows, int cols, IEnumerable<double> vals) : this(rows, cols)
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                values[i, j] = vals.ElementAt(i * Columns + j);
            }
        }
    }

    public Matrix(double[,] vals)
    {
        Rows = vals.GetLength(0);
        Columns = vals.GetLength(1);
        Square = Rows == Columns;
        values = vals;
    }

    public Matrix(int rows, int cols)
    {
        if (rows < 1 || cols < 1) throw new DimensionLessThanOneException();
        Rows = rows;
        Columns = cols;
        Square = rows == cols;
        values = new double[Rows, Columns];
    }

    public Matrix(int dimension)
    {
        if (dimension < 1) throw new DimensionLessThanOneException();
        Rows = dimension;
        Columns = dimension;
        Square = true;
        values = new double[Rows, Columns];
    }

    public static Matrix operator -(Matrix m)
    {
        Matrix output = new(m.Rows, m.Columns);

        for (int i = 0; i < m.Rows; i++)
        {
            for (int j = 0; j < m.Columns; j++)
            {
                output[i, j] = -m[i, j];
            }
        }

        return m;
    }

    public static Matrix operator +(Matrix a, Matrix b)
    {
        Matrix m = new(a.Rows, a.Columns);

        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < a.Columns; j++)
            {
                m[i, j] = a[i, j] + b[i, j];
            }
        }

        return m;
    }

    public static Matrix operator -(Matrix a, Matrix b) => a + -b;

    public IEnumerable<double> GetRow(int rowIndex)
    {
        for (int i = 0; i < Columns; i++)
        {
            yield return values[rowIndex, i];
        }
    }
    public IEnumerable<double> GetCol(int colIndex)
    {
        for (int i = 0; i < Rows; i++)
        {
            yield return values[i, colIndex];
        }
    }

    public static Matrix operator *(double scalar, Matrix matrix)
    {
        Matrix @new = new(matrix.Rows, matrix.Columns);

        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                @new[i, j] = scalar * matrix[i, j];
            }
        }

        return @new;
    }

    public static Matrix operator *(Matrix a, Matrix b)
    {
        Matrix m = new(a.Rows, b.Columns);

        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < b.Columns; j++)
            {
                double[] row = a.GetRow(i).ToArray();
                m[i, j] = b.GetCol(j).Select((e, i) => e * row[i]).Sum();
            }
        }
        return m;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (int i = 0; i < Rows; i++)
        {
            sb.Append('[');
            for (int j = 0; j < Columns; j++)
            {
                sb.Append(values[i, j]);
                if (j != Columns - 1)
                    sb.Append(' ');
            }
            sb.AppendLine("]");
        }

        return sb.ToString().TrimEnd('\n').TrimEnd('\r');
    }

    public static Matrix Identity(int dimension)
    {
        Matrix m = Zero(dimension);
        for (int i = 0; i < dimension; i++) { m[i, i] = 1; }

        return m;
    }
    public static Matrix Zero(int dimension) => new(dimension, dimension);

    public Matrix Minor(int row, int col)
    {
        if (!Square) throw new MatrixNotSquareException();
        List<double> contents = new((row - 1) * (row - 1));
        for (int i = 0; i < Rows; i++)
        {
            if (i == row) continue;
            for (int j = 0; j < Columns; j++)
            {
                if (j == col) continue;
                contents.Add(values[i, j]);
            }
        }

        return new Matrix(Rows - 1, Columns - 1, contents);
    }
}