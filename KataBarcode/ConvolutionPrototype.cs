#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Drawing;

namespace KataBarcode;

static class ConvolutionPrototype
{
    // convolution kernel protoype based on SetPixel()/GetPixel() and Color for algorithm proving
    public static Bitmap ApplyKernelPrototype(
        Bitmap extendedSource,
        ConvolutionKernel convolutionKernel
    )
    {
        convolutionKernel.CalculatateNormalization();
        var extendedSize = 2 * convolutionKernel.KernelHalfWidth;
        var target = new Bitmap(
            extendedSource.Width - extendedSize,
            extendedSource.Height - extendedSize
        );

        var sourceX = convolutionKernel.KernelHalfWidth;
        var sourceY = convolutionKernel.KernelHalfWidth;
        for (var y = 0; y < target.Height; ++y)
        {
            for (var x = 0; x < target.Width; ++x)
            {
                var colors = SourcePixelsPrototype(
                    extendedSource,
                    convolutionKernel,
                    sourceX + x,
                    sourceY + y
                );
                var color = ApplyKernelToColorsPrototype(convolutionKernel, colors);
                target.SetPixel(x, y, color);
            }
        }

        return target;
    }

    // returning source pixels protoype method
    private static Color[] SourcePixelsPrototype(
        Bitmap source,
        ConvolutionKernel convolutionKernel,
        int x,
        int y
    )
    {
        var colors = new Color[convolutionKernel.KernelSize * convolutionKernel.KernelSize];

        var startX = x - convolutionKernel.KernelHalfWidth;
        var startY = y - convolutionKernel.KernelHalfWidth;
        var colorIndex = 0;
        for (var j = 0; j < convolutionKernel.KernelSize; ++j)
        {
            for (var i = 0; i < convolutionKernel.KernelSize; ++i)
            {
                colors[colorIndex++] = source.GetPixel(startX + i, startY + j);
            }
        }

        return colors;
    }

    // apply kernel to pixels prototype method
    private static Color ApplyKernelToColorsPrototype(
        ConvolutionKernel convolutionKernel,
        Color[] colors
    )
    {
        var r = 0.0;
        var g = 0.0;
        var b = 0.0;
        for (var i = 0; i < convolutionKernel.Coefficients.Length; ++i)
        {
            r += convolutionKernel.Coefficients[i] * colors[i].R;
            g += convolutionKernel.Coefficients[i] * colors[i].G;
            b += convolutionKernel.Coefficients[i] * colors[i].B;
        }

        var nR = r;
        var nG = g;
        var nB = b;
        if (convolutionKernel.NeedNormalization)
        {
            nR = r * convolutionKernel.NormalizationFactor;
            nG = g * convolutionKernel.NormalizationFactor;
            nB = b * convolutionKernel.NormalizationFactor;
        }

        return Color.FromArgb(Clamp(nR), Clamp(nG), Clamp(nB));
    }

    // clamp value to int min/max
    private static int Clamp(double d)
    {
        var b = Math.Min(byte.MaxValue, d);
        b = Math.Max(byte.MinValue, b);
        return Convert.ToInt32(b);
    }
}
