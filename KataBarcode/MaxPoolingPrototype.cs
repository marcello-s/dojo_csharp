#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Drawing;

namespace KataBarcode;

public static class MaxPoolingPrototype
{
    // max pooling based on SetPixel()/GetPixel() for algorithm proving
    public static Bitmap MaxPool(Bitmap source, int squareSize = 2)
    {
        var target = new Bitmap(source.Width / squareSize, source.Height / squareSize);

        var ty = 0;
        var tx = 0;
        var ci = 0;
        var colors = new Color[squareSize * 2];

        for (var y = 0; y < source.Height - 1; y += squareSize)
        {
            tx = 0;
            for (var x = 0; x < source.Width - 1; x += squareSize)
            {
                ci = 0;
                for (var yc = 0; yc < squareSize; ++yc)
                {
                    for (var xc = 0; xc < squareSize; ++xc)
                    {
                        colors[ci] = source.GetPixel(
                            Math.Min(x + xc, source.Width),
                            Math.Min(y + yc, source.Height)
                        );
                        ++ci;
                    }
                }

                target.SetPixel(tx, ty, Max(colors));
                ++tx;
            }

            ++ty;
        }

        return target;
    }

    private static Color Max(Color[] colors)
    {
        var maxColor = 0;
        var maxIndex = 0;
        var sum = 0;

        for (var i = 0; i < colors.Length; ++i)
        {
            sum = colors[i].R + colors[i].G + colors[i].B;
            if (sum > maxColor)
            {
                maxColor = sum;
                maxIndex = i;
            }
        }

        return colors[maxIndex];
    }
}
