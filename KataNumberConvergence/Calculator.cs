#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataNumberConvergence;

public class Calculator
{
    private const double Epsilon = double.Epsilon;
    private readonly object lockOnList = new object();

    public static double Sqrt(double a)
    {
        var sqrt = 0.0;
        var xn = (a + 1.0) / 2.0;
        do
        {
            sqrt = xn;
            xn = (xn + a / xn) / 2.0;
        } while (Math.Abs(xn - sqrt) > Epsilon);

        return sqrt;
    }

    public void SqrtToListLockOnList(double a, IList<string> results)
    {
        const int iterations = 10000000; // deliberately waste cpu cycles
        var sqrt = 0.0;
        var xn = (a + 1.0) / 2.0;
        for (var i = 0; i < iterations; i++)
        {
            sqrt = xn;
            xn = (xn + a / xn) / 2.0;
        }

        var result = string.Format(
            "sqrt({0}) = {1}; task={2} hash={3}",
            a,
            sqrt,
            Task.CurrentId,
            GetHashCode()
        );

        lock (lockOnList)
        {
            results.Add(result);
        }
    }
}
