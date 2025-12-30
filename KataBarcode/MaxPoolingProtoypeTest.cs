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
public class MaxPoolingProtoypeTest
{
    private const string FileOtsuName = "otsu_input.jpg";
    private const string File7Name = "7.jpg";
    private const string FileSubject1Name = "subject01.normal.gif";
    private const string MaxPoolName = "MaxPool.jpg";

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void MaxPoolTest1()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileOtsuName);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);
            var target = MaxPoolingPrototype.MaxPool(source);
            var targetPath = Path.Combine(CurrentDirectory, MaxPoolName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            target.Dispose();
        }
    }

    [Test]
    public void MaxPoolTest2()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, File7Name);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);
            var target = MaxPoolingPrototype.MaxPool(source);
            var targetPath = Path.Combine(CurrentDirectory, MaxPoolName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            target.Dispose();
        }
    }

    [Test]
    public void MaxPoolTest3()
    {
        const int LoopSize = 3;
        Bitmap? target = null;

        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileSubject1Name);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);

            for (var i = 0; i < LoopSize; ++i)
            {
                target = MaxPoolingPrototype.MaxPool(source);
                source = target;
            }

            if (target != null)
            {
                var targetPath = Path.Combine(CurrentDirectory, MaxPoolName);
                loader.Save(target, targetPath, ImageFormat.Jpeg);

                target.Dispose();
            }
        }
    }
}
