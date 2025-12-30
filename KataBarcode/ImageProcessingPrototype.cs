#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Drawing;

namespace KataBarcode;

static class ImageProcessingPrototype
{
    // transform protoype based on GetPixel()/SetPixel() and Color for algorithm proving
    public static Bitmap Transform(Bitmap source, Func<Color, Color> transformFunc)
    {
        var target = new Bitmap(source.Width, source.Height);

        for (var y = 0; y < source.Height; ++y)
        {
            for (var x = 0; x < source.Width; ++x)
            {
                target.SetPixel(x, y, transformFunc(source.GetPixel(x, y)));
            }
        }

        return target;
    }

    private const double LumRedScale = 0.3d;
    private const double LumGreenScale = 0.59d;
    private const double LumBlueScale = 0.11d;

    // luminescense transformation prototype based on color
    public static Color Luminescense(Color c)
    {
        var gray = Convert.ToByte(LumRedScale * c.R + LumGreenScale * c.G + LumBlueScale * c.B);
        return Color.FromArgb(c.A, gray, gray, gray);
    }

    private const int HistogramSize = 256;
    public static readonly uint[] Histogram = new uint[HistogramSize];

    public static Color HistogramTransform(Color c)
    {
        Histogram[c.R]++;
        return c;
    }

    private static byte threshold;

    public static byte OtsuThresholding()
    {
        var v = new double[HistogramSize];
        for (var k = 1; k < HistogramSize - 1; k++)
        {
            var p1 = Probability(0, k, Histogram);
            var p2 = Probability(k + 1, HistogramSize, Histogram);
            var p12 = p1 * p2;
            p12 = p12 == 0 ? 1 : p12;
            var diff = (Mean(0, k, Histogram) * p2) - (Mean(k + 1, HistogramSize, Histogram) * p1);
            v[k] = (double)diff * diff / p12;
        }

        threshold = Convert.ToByte(IndexOfMax(v));
        return threshold;
    }

    public static Color Thresholding(Color c)
    {
        return c.R > threshold ? Color.White : Color.Black;
    }

    private static long Probability(int start, int end, uint[] histogram)
    {
        var sum = 0L;

        for (var i = start; i < end; ++i)
        {
            sum += histogram[i];
        }

        return sum;
    }

    private static long Mean(int start, int end, uint[] histogram)
    {
        var sum = 0L;

        for (var i = start; i < end; ++i)
        {
            sum += i * histogram[i];
        }

        return sum;
    }

    private static int IndexOfMax(double[] v)
    {
        var max = 0d;
        var index = 0;

        for (var i = 0; i < v.Length; i++)
        {
            if (v[i] < max)
                continue;
            max = v[i];
            index = i;
        }

        return index;
    }

    private static int[,]? houghMap;
    private static int houghHeight;
    private static int houghWidth;

    public static void HoughTransform(Bitmap source)
    {
        const int StepsPerDegree = 1;

        var stepsPerDegree = Math.Max(1, Math.Min(10, StepsPerDegree));
        houghHeight = 180 * stepsPerDegree;
        var thetaStep = Math.PI / houghHeight;

        // pre compute sine/cosine tables
        var sinMap = new double[houghHeight];
        var cosMap = new double[houghHeight];

        for (var i = 0; i < houghHeight; ++i)
        {
            sinMap[i] = Math.Sin(i * thetaStep);
            cosMap[i] = Math.Cos(i * thetaStep);
        }

        // setup hough map
        var halfWidth = source.Width / 2;
        var halfHeight = source.Height / 2;
        var halfHoughWidth = (int)Math.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
        houghWidth = halfHoughWidth * 2;

        houghMap = new int[houghHeight, houghWidth];

        // create hough map
        for (var y = 0; y < source.Height; ++y)
        {
            for (var x = 0; x < source.Width; ++x)
            {
                var c = source.GetPixel(x, y);
                var n = c.R + c.G + c.B;
                if (n == 0)
                    continue;

                for (var theta = 0; theta < houghHeight; ++theta)
                {
                    var radius =
                        (int)
                            Math.Round(
                                (x - halfWidth) * cosMap[theta] - (y - halfHeight) * sinMap[theta]
                            ) + halfHoughWidth;
                    if (radius < 0 || radius >= houghWidth)
                        continue;
                    houghMap[theta, radius]++;
                }
            }
        }
    }

    private static int FindMaxMapIntensity(int houghHeight, int houghWidth)
    {
        // find max map intensity
        var maxMapIntensity = 0;
        for (var i = 0; i < houghHeight; ++i)
        {
            for (var j = 0; j < houghWidth; ++j)
            {
                if (houghMap![i, j] > maxMapIntensity)
                {
                    maxMapIntensity = houghMap[i, j];
                }
            }
        }

        Console.WriteLine("max map intensity: '{0}'", maxMapIntensity);
        return maxMapIntensity;
    }

    public static Bitmap RenderHoughMapToBitmap()
    {
        // output hough map to bitmap
        var bitmap = new Bitmap(houghWidth, houghHeight);
        var scale = 255d / FindMaxMapIntensity(houghHeight, houghWidth);
        for (var y = 0; y < bitmap.Height; ++y)
        {
            for (var x = 0; x < bitmap.Width; ++x)
            {
                var h = Math.Min(255, (int)(scale * houghMap![y, x]));
                var c = Color.FromArgb(h, h, h);
                bitmap.SetPixel(x, y, c);
            }
        }

        return bitmap;
    }

    public static IEnumerable<HoughLine> FindLocalMaxima()
    {
        var maxTheta = houghHeight;
        var maxRadius = houghWidth;

        var halfHoughWidth = houghWidth / 2;
        var minLineIntensity = FindMaxMapIntensity(houghHeight, houghWidth) / 2;
        const int localPeakRadius = 4;

        var lineList = new List<HoughLine>();

        for (var theta = 0; theta < maxTheta; ++theta)
        {
            for (var radius = 0; radius < maxRadius; ++radius)
            {
                var intensity = houghMap![theta, radius];

                if (intensity < minLineIntensity)
                {
                    continue;
                }

                var foundGreater = false;
                for (
                    int tt = theta - localPeakRadius, ttMax = theta + localPeakRadius;
                    tt < ttMax;
                    tt++
                )
                {
                    if (foundGreater)
                    {
                        break;
                    }

                    var cycledTheta = tt;
                    var cycledRadius = radius;
                    if (cycledTheta < 0)
                    {
                        cycledTheta = maxTheta + cycledTheta;
                        cycledRadius = maxRadius - cycledRadius;
                    }

                    if (cycledTheta >= maxTheta)
                    {
                        cycledTheta -= maxTheta;
                        cycledRadius = maxRadius - cycledRadius;
                    }

                    for (
                        int tr = cycledRadius - localPeakRadius,
                            trMax = cycledRadius + localPeakRadius;
                        tr < trMax;
                        tr++
                    )
                    {
                        if (tr < 0)
                        {
                            continue;
                        }

                        if (tr > maxRadius)
                        {
                            break;
                        }

                        if (houghMap[cycledTheta, tr] > intensity)
                        {
                            foundGreater = true;
                            break;
                        }
                    }
                }

                if (!foundGreater)
                {
                    var lineTheta = theta;
                    var lineRadius = radius - halfHoughWidth;
                    Console.WriteLine(
                        "line at: theta '{0}', radius '{1}', intensity '{2}'",
                        lineTheta,
                        lineRadius,
                        intensity
                    );

                    lineList.Add(new HoughLine(lineTheta, lineRadius, intensity));
                }
            }
        }

        lineList.Sort();
        return lineList;
    }
}
