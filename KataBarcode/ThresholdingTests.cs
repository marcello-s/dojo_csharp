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
public class ThresholdingTests
{
    private const string FileOtsuName = "otsu_input.jpg";
    private const string ThresName = "Thres.jpg";

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void FastThresholding()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileOtsuName);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);

            var target = ImageProcessing.Transform(source, ImageProcessing.Luminescence);
            var dummy = ImageProcessing.Transform(target, ImageProcessing.HistogramTransform);
            var threshold = ImageProcessing.OtsuThresholding();
            Console.WriteLine(threshold);
            target = ImageProcessing.Transform(target, ImageProcessing.Threshold);

            foreach (var c in ImageProcessing.Histogram)
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
