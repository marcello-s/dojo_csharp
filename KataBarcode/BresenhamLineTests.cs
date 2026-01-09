#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;

namespace KataBarcode;

[TestFixture]
public class BresenhamLineTests
{
    private const string LineName = "line.png";
    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void Draw()
    {
        var bitmap = new Bitmap(640, 480);

        const int stepSize = 10;
        var p1 = new Point(bitmap.Width / 2, bitmap.Height / 2);
        var p2 = new Point(0, 0);
        var ySteps = bitmap.Height / stepSize;
        for (var y = 0; y < ySteps; ++y)
        {
            p2 = new Point(bitmap.Width, y * stepSize);
            BresenhamLine.Draw(bitmap, p1, p2, Color.White);
            p2 = new Point(0, y * stepSize);
            BresenhamLine.Draw(bitmap, p1, p2, Color.White);
        }

        var xSteps = bitmap.Width / stepSize;
        for (var x = 0; x <= xSteps; ++x)
        {
            p2 = new Point(x * stepSize, 0);
            BresenhamLine.Draw(bitmap, p1, p2, Color.White);
            p2 = new Point(x * stepSize, bitmap.Height - 1);
            BresenhamLine.Draw(bitmap, p1, p2, Color.White);
        }

        var filepath = Path.Combine(CurrentDirectory, LineName);
        bitmap.Save(filepath, ImageFormat.Png);
        bitmap.Dispose();
    }

    [Test]
    public void Draw_Clipped()
    {
        var bitmap = new Bitmap(640, 480);

        BresenhamLine.Draw(
            bitmap,
            new Point(0, 0),
            new Point(bitmap.Width - 1, bitmap.Height - 1),
            Color.White
        );
        BresenhamLine.Draw(
            bitmap,
            new Point(bitmap.Width - 1, 0),
            new Point(0, bitmap.Height - 1),
            Color.White
        );

        var p1 = new Point(50, -20);
        var p2 = new Point(100, 550);
        //BresenhamLine.Draw(bitmap, p1, p2, Color.White);

        var filepath = Path.Combine(CurrentDirectory, LineName);
        bitmap.Save(filepath, ImageFormat.Png);
        bitmap.Dispose();
    }
}
