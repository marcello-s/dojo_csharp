#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Drawing;
using System.Drawing.Imaging;

namespace KataBarcode;

static class ImageProcessing
{
    // unchecked method to compute Otsu threshold
    private static byte threshold;

    public static byte OtsuThresholding()
    {
        var v = new double[HistogramSize];
        unchecked
        {
            for (var k = 1; k < HistogramSize - 1; k++)
            {
                var p1 = Probability(0, k, Histogram);
                var p2 = Probability(k + 1, HistogramSize, Histogram);
                var p12 = p1 * p2;
                p12 = p12 == 0 ? 1 : p12;
                var diff =
                    (Mean(0, k, Histogram) * p2) - (Mean(k + 1, HistogramSize, Histogram) * p1);
                v[k] = (double)diff * diff / p12;
            }
        }

        threshold = Convert.ToByte(IndexOfMax(v));
        return threshold;
    }

    // unchecked method to sum up probability from histogram
    private static long Probability(int start, int end, uint[] histogram)
    {
        var sum = 0L;
        unchecked
        {
            for (var i = start; i < end; ++i)
            {
                sum += histogram[i];
            }
        }

        return sum;
    }

    // unchecked method to compute mean from histogram
    private static long Mean(int start, int end, uint[] histogram)
    {
        var sum = 0L;
        unchecked
        {
            for (var i = start; i < end; ++i)
            {
                sum += i * histogram[i];
            }
        }

        return sum;
    }

    // unchecked method to search index of max value
    private static int IndexOfMax(double[] v)
    {
        var max = 0d;
        var index = 0;
        unchecked
        {
            for (var i = 0; i < v.Length; i++)
            {
                if (v[i] < max)
                    continue;
                max = v[i];
                index = i;
            }
        }

        return index;
    }

    // fast transform that uses unsafe pointers and raw pixel format
    public static Bitmap Transform(Bitmap source, Func<uint, uint> transformFunc)
    {
        // create target image, 32bit ARGB, lock data bytes
        var target = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
        var sourceData = source.LockBits(
            new Rectangle(0, 0, source.Width, source.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb
        );
        var targetData = target.LockBits(
            new Rectangle(0, 0, target.Width, target.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb
        );

        // loop on source and call transformation
        UnsafeLoop(sourceData, targetData, transformFunc);

        // unlock data bytes
        source.UnlockBits(sourceData);
        target.UnlockBits(targetData);
        return target;
    }

    // unsafe nested loop over pixels in Argb format, call transformation for each pixel
    private static void UnsafeLoop(
        BitmapData source,
        BitmapData target,
        Func<uint, uint> transformFunc
    )
    {
        // boundary checks off
        unchecked
        {
            // direct memory access
            unsafe
            {
                // uint pointer to source and target scan line
                var sourceLine = (uint*)source.Scan0;
                var targetLine = (uint*)target.Scan0;

                // nested loop height/width
                for (var y = 0; y < source.Height; ++y)
                {
                    for (var x = 0; x < source.Width; ++x)
                    {
                        // call transformation
                        var t = transformFunc(sourceLine[0]);

                        // set transformation result
                        targetLine[0] = t;

                        // advance pointers by 4 bytes, the size of uint
                        ++sourceLine;
                        ++targetLine;
                    }

                    // advance pointers by stride/width minus padding
                    sourceLine += source.Stride - (source.Width * 4);
                    targetLine += source.Stride - (source.Width * 4);
                }
            }
        }
    }

    private const double LumRedScale = 0.3d;
    private const double LumGreenScale = 0.59d;
    private const double LumBlueScale = 0.11d;

    // luminescense transformation based on uint
    public static uint Luminescence(uint p)
    {
        // separate Argb, shift and mask
        const uint mask = 0x000000ff;
        var b = p & mask;
        var g = (p >> 8) & mask;
        var r = (p >> 16) & mask;
        var a = (p >> 24) & mask;

        // calculate luminescense, copy transparency a
        var lum = Convert.ToUInt32(LumRedScale * r + LumGreenScale * g + LumBlueScale * b);

        // combine Argb
        //var result = (a << 24) | (r << 16) | (g << 8) | b;
        var result = (a << 24) | (lum << 16) | (lum << 8) | lum;
        return result;
    }

    // histogram transformation based on uint
    private const int HistogramSize = 256;
    public static readonly uint[] Histogram = new uint[HistogramSize];

    public static uint HistogramTransform(uint p)
    {
        // separate Argb, shift and mask
        const uint mask = 0x000000ff;
        var b = p & mask;

        Histogram[b]++;

        return p;
    }

    // black&white threshold based on uint
    public static uint Threshold(uint p)
    {
        // separate Argb, shift and mask
        const uint mask = 0x000000ff;
        var b = p & mask;
        var a = (p >> 24) & mask;

        // return black or white, copy transparency a
        var th = b > threshold ? (uint)255 : 0;

        var result = (a << 24) | (th << 16) | (th << 8) | th;
        return result;
    }
}
