#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataBarcode;

public class HoughLine(int theta, int radius, int intensity) : IComparable
{
    public int Theta { get; private set; } = theta;
    public int Radius { get; private set; } = radius;
    public int Intensity { get; private set; } = intensity;

    public int CompareTo(object? obj)
    {
        return -Intensity.CompareTo(((HoughLine)obj!).Intensity);
    }
}
