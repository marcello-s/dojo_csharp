#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Drawing;
using System.Drawing.Imaging;

namespace KataBarcode;

static class Convolution
{
    public static Bitmap ExtendBitmap(Bitmap source, int kernelHalfWidth)
    {
        var extendedSize = 2 * kernelHalfWidth;
        var target = new Bitmap(source.Width + extendedSize, source.Height + extendedSize);

        // upper left corner
        var cornerColor = source.GetPixel(0, 0);
        FillCorner(target, cornerColor, kernelHalfWidth, 0, 0);

        // upper right corner
        cornerColor = source.GetPixel(source.Width - 1, 0);
        FillCorner(target, cornerColor, kernelHalfWidth, source.Width + kernelHalfWidth, 0);

        // lower left corner
        cornerColor = source.GetPixel(0, source.Height - 1);
        FillCorner(target, cornerColor, kernelHalfWidth, 0, source.Height + kernelHalfWidth);

        // lower right corner
        cornerColor = source.GetPixel(source.Width - 1, source.Height - 1);
        FillCorner(
            target,
            cornerColor,
            kernelHalfWidth,
            source.Width + kernelHalfWidth,
            source.Height + kernelHalfWidth
        );

        // top scan line
        FillScanlines(source, target, 0, kernelHalfWidth, 0);

        // bottom scan line
        FillScanlines(
            source,
            target,
            source.Height - 1,
            kernelHalfWidth,
            source.Height + kernelHalfWidth
        );

        // left side
        FillSides(source, target, 0, 0, kernelHalfWidth);

        // right side
        FillSides(
            source,
            target,
            source.Width - 1,
            source.Width + kernelHalfWidth,
            kernelHalfWidth
        );

        // copy original
        for (var y = 0; y < source.Height; ++y)
        {
            for (var x = 0; x < source.Width; ++x)
            {
                target.SetPixel(kernelHalfWidth + x, kernelHalfWidth + y, source.GetPixel(x, y));
            }
        }

        return target;
    }

    private static void FillCorner(Bitmap target, Color color, int kernelHalfWidth, int x, int y)
    {
        for (var j = y; j < y + kernelHalfWidth; ++j)
        {
            for (var i = x; i < x + kernelHalfWidth; ++i)
            {
                target.SetPixel(i, j, color);
            }
        }
    }

    private static void FillScanlines(
        Bitmap source,
        Bitmap target,
        int sourceY,
        int kernelHalfWidth,
        int targetY
    )
    {
        for (var i = 0; i < kernelHalfWidth; ++i)
        {
            for (var j = 0; j < source.Width; ++j)
            {
                target.SetPixel(kernelHalfWidth + j, targetY + i, source.GetPixel(j, sourceY));
            }
        }
    }

    private static void FillSides(
        Bitmap source,
        Bitmap target,
        int sourceX,
        int targetX,
        int kernelHalfWidth
    )
    {
        for (var i = 0; i < kernelHalfWidth; ++i)
        {
            for (var j = 0; j < source.Height; ++j)
            {
                target.SetPixel(targetX + i, kernelHalfWidth + j, source.GetPixel(sourceX, j));
            }
        }
    }

    // fast convolution kernel that uses unsafe pointers and raw pixel format
    public static Bitmap ApplyKernel(Bitmap extendedSource, ConvolutionKernel convolutionKernel)
    {
        convolutionKernel.CalculatateNormalization();
        var extendedSize = 2 * convolutionKernel.KernelHalfWidth;

        // create target image, 32bit ARGB, lock data bytes
        var target = new Bitmap(
            extendedSource.Width - extendedSize,
            extendedSource.Height - extendedSize
        );
        var sourceData = extendedSource.LockBits(
            new Rectangle(0, 0, extendedSource.Width, extendedSource.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb
        );
        var targetData = target.LockBits(
            new Rectangle(0, 0, target.Width, target.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb
        );

        // loop on source and apply convolution kernel
        UnsafeLoop(sourceData, targetData, convolutionKernel);

        // unlock data bytes
        extendedSource.UnlockBits(sourceData);
        target.UnlockBits(targetData);
        return target;
    }

    // unsafe nested loop over pixels in Argb format, apply convolution kernel for each pixel
    private static void UnsafeLoop(
        BitmapData source,
        BitmapData target,
        ConvolutionKernel convolutionKernel
    )
    {
        // boundary checks off
        unchecked
        {
            // direct memory access
            unsafe
            {
                // uint pointer to source and target scan line
                //var sourceLine = (uint*)source.Scan0;
                var targetLine = (uint*)target.Scan0;

                // shift right by kernel half width
                //sourceLine += convolutionKernel.KernelHalfWidth;
                // shift down by kernel half width
                //sourceLine += (source.Stride - (source.Width * 4) + convolutionKernel.KernelHalfWidth * 2)
                //              * convolutionKernel.KernelHalfWidth
                //              + target.Width * convolutionKernel.KernelHalfWidth;

                // uint pointer to pixels scan line
                var pixelsLine = (uint*)source.Scan0;
                // source pixels array
                var pixels = new uint[convolutionKernel.KernelSize * convolutionKernel.KernelSize];
                // next source pixel line - constant
                var nextLine =
                    source.Stride
                    - (source.Width * 4)
                    + convolutionKernel.KernelHalfWidth * 2
                    + (source.Width - convolutionKernel.KernelHalfWidth * 2);

                // nested loop height/width
                for (var y = 0; y < target.Height; ++y)
                {
                    for (var x = 0; x < target.Width; ++x)
                    {
                        // direct copy
                        //targetLine[0] = sourceLine[0];

                        // get source pixels
                        pixels = SourcePixels(
                            pixelsLine,
                            pixels,
                            nextLine,
                            convolutionKernel.KernelSize
                        );
                        //targetLine[0] = pixels[4];
                        targetLine[0] = ApplyKernelToPixels(pixels, convolutionKernel);

                        // advance pointers by 4 bytes, the size of uint
                        //++sourceLine;
                        ++targetLine;
                        ++pixelsLine;
                    }

                    // advance pointer by stride/width minus padding plus 2*kernel half width
                    //sourceLine += source.Stride - (source.Width * 4) + convolutionKernel.KernelHalfWidth * 2;
                    pixelsLine +=
                        source.Stride - (source.Width * 4) + convolutionKernel.KernelHalfWidth * 2;
                    // advance pointer by stride/width minus padding
                    targetLine += target.Stride - (target.Width * 4);
                }
            }
        }
    }

    // return a kernel_size^2 array of source pixels
    private static unsafe uint[] SourcePixels(
        uint* sourcePtr,
        uint[] pixels,
        int nextLine,
        int kernelSize
    )
    {
        var pixelsIndex = 0;
        unchecked
        {
            for (var j = 0; j < kernelSize; ++j)
            {
                for (var i = 0; i < kernelSize; ++i)
                {
                    pixels[pixelsIndex++] = sourcePtr[i];
                }

                sourcePtr += nextLine;
            }
        }

        return pixels;
    }

    // apply convolution kernel to pixels based on uint
    private static uint ApplyKernelToPixels(uint[] pixels, ConvolutionKernel convolutionKernel)
    {
        // separate Argb, shift and mask
        const uint mask = 0x000000ff;

        double pB = 0;
        double pG = 0;
        double pR = 0;
        double pA = 0;
        unchecked
        {
            for (var i = 0; i < convolutionKernel.Coefficients.Length; ++i)
            {
                var b = pixels[i] & mask;
                var g = (pixels[i] >> 8) & mask;
                var r = (pixels[i] >> 16) & mask;
                var a = (pixels[i] >> 24) & mask;

                pB += b * convolutionKernel.Coefficients[i];
                pG += g * convolutionKernel.Coefficients[i];
                pR += r * convolutionKernel.Coefficients[i];
                pA += a * convolutionKernel.Coefficients[i];
            }
        }

        if (convolutionKernel.NeedNormalization)
        {
            pB = pB * convolutionKernel.NormalizationFactor;
            pG = pG * convolutionKernel.NormalizationFactor;
            pR = pR * convolutionKernel.NormalizationFactor;
            pA = pA * convolutionKernel.NormalizationFactor;
        }

        var result =
            (ClampToUint(pA) << 24)
            | (ClampToUint(pR) << 16)
            | (ClampToUint(pG) << 8)
            | ClampToUint(pB);
        return result;
    }

    // clamp value to int min/max
    private static uint ClampToUint(double d)
    {
        var b = Math.Min(byte.MaxValue, d);
        b = Math.Max(byte.MinValue, b);
        return Convert.ToUInt32(b);
    }
}
