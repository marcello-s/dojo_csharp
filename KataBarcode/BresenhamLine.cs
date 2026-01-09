#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Drawing;
using System.Runtime.CompilerServices;

namespace KataBarcode;

public static class BresenhamLine
{
    public static void Draw(Bitmap bitmap, Point p1, Point p2, Color color)
    {
        var line = new Line(p1, p2, color);
        var lc = ClipToVisibleArea(line, bitmap.Width, bitmap.Height);

        int x0 = lc.P1.X;
        int y0 = lc.P1.Y;
        int x1 = lc.P2.X;
        int y1 = lc.P2.Y;

        if (y0 == y1)
        {
            DrawHorizontal(bitmap, x0, x1, y0, lc.Color);
            return;
        }

        if (x0 == x1)
        {
            DrawVertical(bitmap, x0, y0, y1, lc.Color);
            return;
        }

        var dy = Math.Abs(y1 - y0);
        var dx = Math.Abs(x1 - x0);
        var steep = dy > dx;
        if (steep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        dx = x1 - x0;
        dy = Math.Abs(y1 - y0);
        var error = dx / 2;
        int yStep = (y0 < y1) ? 1 : -1;
        int y = y0;

        if (steep)
        {
            for (int x = x0; x < x1; ++x)
            {
                bitmap.SetPixel(y, x, lc.Color);
                error -= dy;
                if (error < 0)
                {
                    y += yStep;
                    error += dx;
                }
            }
        }
        else
        {
            for (int x = x0; x < x1; ++x)
            {
                bitmap.SetPixel(x, y, lc.Color);
                error -= dy;
                if (error < 0)
                {
                    y += yStep;
                    error += dx;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref int x, ref int y)
    {
        int t = x;
        x = y;
        y = t;
    }

    public static void DrawHorizontal(Bitmap bitmap, int x0, int x1, int y, Color color)
    {
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
        }

        for (int x = x0; x < x1; ++x)
        {
            bitmap.SetPixel(x, y, color);
        }
    }

    public static void DrawVertical(Bitmap bitmap, int x, int y0, int y1, Color color)
    {
        if (y0 > y1)
        {
            Swap(ref y0, ref y1);
        }

        for (int y = y0; y < y1; ++y)
        {
            bitmap.SetPixel(x, y, color);
        }
    }

    private static Line ClipToVisibleArea(Line l, int width, int height)
    {
        if (
            (l.P1.X >= 0 && l.P1.X < width)
            && (l.P1.Y >= 0 && l.P1.Y < height)
            && (l.P2.X >= 0 && l.P2.Y < width)
            && (l.P2.Y >= 0 && l.P2.Y < height)
        )
        {
            return l;
        }

        return l;
    }

    private struct Line
    {
        public Point P1 { get; }
        public Point P2 { get; }
        public Color Color { get; }

        public Line(Point p1, Point p2, Color color)
            : this()
        {
            P1 = p1;
            P2 = p2;
            Color = color;
        }
    }
}
