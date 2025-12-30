#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;

namespace KataBarcode;

[TestFixture]
public class HoughTransformPrototypeTests
{
    private const string AppliedName = "Applied.jpg";
    private const string FileHoughName = "hough_input.jpg";
    private const string FileHough_h_Name = "hough_h_input.jpg";
    private const string FileHough_v_Name = "hough_v_input.jpg";
    private const string HoughName = "Hough.jpg";
    private const string HoughNameVisualized = "HoughVisualized.jpg";

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void HoughTransformTest()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileHoughName);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);

            ImageProcessingPrototype.HoughTransform(source);
            var lines = ImageProcessingPrototype.FindLocalMaxima();
            VisualizeHoughLines(loader, source, HoughNameVisualized, lines);
            var target = ImageProcessingPrototype.RenderHoughMapToBitmap();

            var targetPath = Path.Combine(CurrentDirectory, HoughName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            source.Dispose();
            target.Dispose();
        }
    }

    [Test]
    public void HoughTransformHorizontalTest()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileHough_h_Name);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);

            ImageProcessingPrototype.HoughTransform(source);
            var lines = ImageProcessingPrototype.FindLocalMaxima();
            VisualizeHoughLines(loader, source, HoughNameVisualized, lines);
            var target = ImageProcessingPrototype.RenderHoughMapToBitmap();

            var targetPath = Path.Combine(CurrentDirectory, HoughName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            source.Dispose();
            target.Dispose();
        }
    }

    [Test]
    public void HoughTransformVerticalTest()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileHough_v_Name);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);

            ImageProcessingPrototype.HoughTransform(source);
            var lines = ImageProcessingPrototype.FindLocalMaxima();
            VisualizeHoughLines(loader, source, HoughNameVisualized, lines);
            var target = ImageProcessingPrototype.RenderHoughMapToBitmap();

            var targetPath = Path.Combine(CurrentDirectory, HoughName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            source.Dispose();
            target.Dispose();
        }
    }

    [Test]
    public void HoughTransformAppliedTest()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, AppliedName);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);

            var input = ImageProcessingPrototype.Transform(
                source,
                ImageProcessingPrototype.Luminescense
            );
            var dummy = ImageProcessingPrototype.Transform(
                input,
                ImageProcessingPrototype.HistogramTransform
            );
            var threshold = ImageProcessingPrototype.OtsuThresholding();
            input = ImageProcessingPrototype.Transform(
                input,
                ImageProcessingPrototype.Thresholding
            );

            ImageProcessingPrototype.HoughTransform(input);
            var lines = ImageProcessingPrototype.FindLocalMaxima();
            VisualizeHoughLines(loader, source, HoughNameVisualized, lines);
            var target = ImageProcessingPrototype.RenderHoughMapToBitmap();

            var targetPath = Path.Combine(CurrentDirectory, HoughName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            source.Dispose();
            target.Dispose();
        }
    }

    private static void VisualizeHoughLines(
        IImageLoader loader,
        Bitmap source,
        string fileName,
        IEnumerable<HoughLine> lines
    )
    {
        var target = new Bitmap(source);

        foreach (var line in lines)
        {
            var r = line.Radius;
            var t = (double)line.Theta;

            // check if line is in lower part of image
            if (r < 0)
            {
                t += 180;
                r = -r;
            }

            // convert degrees to radians
            t = (t / 180d) * Math.PI;

            var w2 = target.Width / 2;
            var h2 = target.Height / 2;

            double x0,
                x1,
                y0,
                y1;
            if (line.Theta != 0)
            {
                // non-vertical line
                x0 = -w2; // most left point
                x1 = w2; // most right point

                y0 = (-Math.Cos(t) * x0 + r) / Math.Sin(t);
                y1 = (-Math.Cos(t) * x1 + r) / Math.Sin(t);
            }
            else
            {
                // vertical line
                x0 = line.Radius;
                x1 = line.Radius;

                y0 = h2;
                y1 = -h2;
            }

            var lineX0 = (int)x0 + w2;
            var lineY0 = h2 - (int)y0;
            var lineX1 = (int)x1 + w2;
            var lineY1 = h2 - (int)y1;

            lineX0 = Math.Max(lineX0, 0);
            lineX0 = Math.Min(lineX0, target.Width);
            lineY0 = Math.Max(lineY0, 0);
            lineY0 = Math.Min(lineY0, target.Height);

            lineX1 = Math.Max(lineX1, 0);
            lineX1 = Math.Min(lineX1, target.Width);
            lineY1 = Math.Max(lineY1, 0);
            lineY1 = Math.Min(lineY1, target.Height);

            BresenhamLine.Draw(
                target,
                new Point(lineX0, lineY0),
                new Point(lineX1, lineY1),
                Color.Red
            );
        }

        var targetPath = Path.Combine(CurrentDirectory, fileName);
        loader.Save(target, targetPath, ImageFormat.Jpeg);

        target.Dispose();
    }
}
