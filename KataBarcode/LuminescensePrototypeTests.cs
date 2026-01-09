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
public class LuminescensePrototypeTests
{
    private const string FileName = "7.jpg";
    private const string LumName = "Lum.jpg";

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void LuminescensePrototypeTest()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileName);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);

            var target = ImageProcessingPrototype.Transform(
                source,
                ImageProcessingPrototype.Luminescense
            );

            var targetPath = Path.Combine(CurrentDirectory, LumName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            source.Dispose();
            target.Dispose();
        }
    }
}
