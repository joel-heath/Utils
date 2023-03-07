using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace JH24Utils.Statistics;
public class Regression
{
    private readonly ImmutableArray<(double x, double y)> _data;
    private readonly SummaryStats stats;
    public SummaryStats SummaryStatistics { get => stats; }
    public ImmutableArray<(double x, double y)> Data { get => _data; }
    //public void UpdateSummaryStats() { stats = new SummaryStats(_data); }

    /// <summary>
    /// Finds the Person's Product Moment Correlation Coefficient (r) value for a given set of data.
    /// </summary>
    /// <returns>The PMCC (-1 ≤ r ≤ 1).</returns>
    public double PMCC() => stats.Sxy / Math.Sqrt(stats.Sxx * stats.Syy);

    /// <summary>
    /// Calculates the Y on X Least Squares Regression Line.
    /// </summary>
    /// <returns>The gradient followed by the y-intercept of the line in a ValueTuple<double></returns>
    public (double m, double c) LeastSquaresYonX()
    {
        double b = stats.Sxy / stats.Sxx;
        double c = b * -stats.x̄ + stats.ȳ;

        return (b, c);
    }
    /// <summary>
    /// Calculates the X on Y Least Squares Regression Line.
    /// </summary>
    /// <returns>The gradient followed by the x-intercept of the line in a ValueTuple<double></returns>
    public (double m, double c) LeastSquaresXonY()
    {
        double bʹ = stats.Sxy / stats.Syy;
        double cʹ = bʹ * -stats.ȳ + stats.x̄;

        return (bʹ, cʹ);
    }

    public Regression(IEnumerable<(double x, double y)> data)
    {
        _data = data.ToImmutableArray();
        stats = new SummaryStats(_data);
    }
    public Regression(IEnumerable<double> xValues, IEnumerable<double> yValues)
    {
        _data = xValues.Zip(yValues, (x, y) => (x, y)).ToImmutableArray();
        stats = new SummaryStats(_data);
    }
}