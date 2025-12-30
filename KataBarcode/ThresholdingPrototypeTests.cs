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
public class ThresholdingPrototypeTests
{
    private const string FileOtsuName = "otsu_input.jpg";
    private const string File7Name = "7.jpg";
    private const string ThresName = "Thres.jpg";

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void ThresholdingPrototypeTest1()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileOtsuName);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);

            var target = ImageProcessingPrototype.Transform(
                source,
                ImageProcessingPrototype.Luminescense
            );
            var dummy = ImageProcessingPrototype.Transform(
                target,
                ImageProcessingPrototype.HistogramTransform
            );
            var threshold = ImageProcessingPrototype.OtsuThresholding();
            Console.WriteLine(threshold);
            target = ImageProcessingPrototype.Transform(
                target,
                ImageProcessingPrototype.Thresholding
            );

            foreach (var c in ImageProcessingPrototype.Histogram)
            {
                Console.WriteLine(c);
            }

            var targetPath = Path.Combine(CurrentDirectory, ThresName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            source.Dispose();
            target.Dispose();
        }
    }

    [Test]
    public void ThresholdingPrototypeTest2()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, File7Name);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);

            var target = ImageProcessingPrototype.Transform(
                source,
                ImageProcessingPrototype.Luminescense
            );
            var dummy = ImageProcessingPrototype.Transform(
                target,
                ImageProcessingPrototype.HistogramTransform
            );
            var threshold = ImageProcessingPrototype.OtsuThresholding();
            Console.WriteLine(threshold);
            target = ImageProcessingPrototype.Transform(
                target,
                ImageProcessingPrototype.Thresholding
            );

            foreach (var c in ImageProcessingPrototype.Histogram)
            {
                Console.WriteLine(c);
            }

            var targetPath = Path.Combine(CurrentDirectory, ThresName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            source.Dispose();
            target.Dispose();
        }
    }
}
