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
public class LuminescenseTests
{
    private const string FileName = "7.jpg";
    private const string LumName = "Lum.jpg";

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void FastLuminescenceTest()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileName);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);
            var target = ImageProcessing.Transform(source, ImageProcessing.Luminescence);

            var targetPath = Path.Combine(CurrentDirectory, LumName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            source.Dispose();
            target.Dispose();
        }
    }

    [TestCase((uint)0xff000000, (uint)4278190080)]
    [TestCase((uint)0x00ff0000, (uint)5000268)]
    [TestCase((uint)0x0000ff00, (uint)9868950)]
    [TestCase((uint)0x000000ff, (uint)1842204)]
    public void LuminescencseByteTest(uint pixel, uint expected)
    {
        Assert.That(ImageProcessing.Luminescence(pixel), Is.EqualTo(expected));
    }
}
