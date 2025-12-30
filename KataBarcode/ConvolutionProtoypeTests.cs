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
public class ConvolutionProtoypeTests
{
    private const string FileName = "7.jpg";
    private const string AppliedName = "Applied.jpg";
    private const int KernelHalfWidth = 1;

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void ApplyKernelPrototype()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileName);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);
            var extendedSource = Convolution.ExtendBitmap(source, KernelHalfWidth);

            //var convolutionKernel = Original(KernelHalfWidth);
            //var convolutionKernel = Blur(KernelHalfWidth);
            //var convolutionKernel = BlurMore(KernelHalfWidth);
            //var convolutionKernel = EdgeDetect(KernelHalfWidth);
            //var convolutionKernel = LowPass(KernelHalfWidth);
            var convolutionKernel = Sharpen(KernelHalfWidth);

            var target = ConvolutionPrototype.ApplyKernelPrototype(
                extendedSource,
                convolutionKernel
            );

            var targetPath = Path.Combine(CurrentDirectory, AppliedName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            source.Dispose();
            target.Dispose();
        }
    }

    [Test]
    public void StrongBlurAndEdgePrototype()
    {
        var loader = new ImageLoader() as IImageLoader;
        var path = Path.Combine(CurrentDirectory, FileName);
        using (var img = loader.LoadFromFile(path))
        {
            var source = new Bitmap(img);
            Bitmap? target = null;

            var blurMore = BlurMore(KernelHalfWidth);
            for (var i = 0; i < 4; ++i)
            {
                var extendedSource = Convolution.ExtendBitmap(source, KernelHalfWidth);
                target = ConvolutionPrototype.ApplyKernelPrototype(extendedSource, blurMore);
                source = target;
            }

            var edgeDetect = EdgeDetect(KernelHalfWidth);
            target = ConvolutionPrototype.ApplyKernelPrototype(source, edgeDetect);

            var targetPath = Path.Combine(CurrentDirectory, AppliedName);
            loader.Save(target, targetPath, ImageFormat.Jpeg);

            source.Dispose();
            target.Dispose();
        }
    }

    private static ConvolutionKernel Original(int kernelHalfWidth)
    {
        var convolutionKernel = new ConvolutionKernel(kernelHalfWidth);
        convolutionKernel.Coefficients[4] = 1d;
        return convolutionKernel;
    }

    private static ConvolutionKernel Blur(int kernelHalfWidth)
    {
        var convolutionKernel = new ConvolutionKernel(kernelHalfWidth);
        for (var i = 0; i < convolutionKernel.Coefficients.Length; ++i)
        {
            convolutionKernel.Coefficients[i] = 1d;
        }

        return convolutionKernel;
    }

    private static ConvolutionKernel BlurMore(int kernelHalfWidth)
    {
        var convolutionKernel = new ConvolutionKernel(kernelHalfWidth);
        convolutionKernel.Coefficients[0] = 1d;
        convolutionKernel.Coefficients[1] = 2d;
        convolutionKernel.Coefficients[2] = 1d;
        convolutionKernel.Coefficients[3] = 2d;
        convolutionKernel.Coefficients[4] = 4d;
        convolutionKernel.Coefficients[5] = 2d;
        convolutionKernel.Coefficients[6] = 1d;
        convolutionKernel.Coefficients[7] = 2d;
        convolutionKernel.Coefficients[8] = 1d;

        return convolutionKernel;
    }

    private static ConvolutionKernel EdgeDetect(int kernelHalfWidth)
    {
        var convolutionKernel = new ConvolutionKernel(kernelHalfWidth);
        convolutionKernel.Coefficients[0] = -1d;
        convolutionKernel.Coefficients[1] = -1d;
        convolutionKernel.Coefficients[2] = -1d;
        convolutionKernel.Coefficients[3] = -1d;
        convolutionKernel.Coefficients[4] = 8d;
        convolutionKernel.Coefficients[5] = -1d;
        convolutionKernel.Coefficients[6] = -1d;
        convolutionKernel.Coefficients[7] = -1d;
        convolutionKernel.Coefficients[8] = -1d;

        return convolutionKernel;
    }

    private static ConvolutionKernel LowPass(int kernelHalfWidth)
    {
        var convolutionKernel = new ConvolutionKernel(kernelHalfWidth);
        convolutionKernel.Coefficients[0] = 1d / 16d;
        convolutionKernel.Coefficients[1] = 1d / 16d;
        convolutionKernel.Coefficients[2] = 1d / 16d;
        convolutionKernel.Coefficients[3] = 1d / 16d;
        convolutionKernel.Coefficients[4] = 1d / 2d;
        convolutionKernel.Coefficients[5] = 1d / 16d;
        convolutionKernel.Coefficients[6] = 1d / 16d;
        convolutionKernel.Coefficients[7] = 1d / 16d;
        convolutionKernel.Coefficients[8] = 1d / 16d;

        return convolutionKernel;
    }

    private static ConvolutionKernel Sharpen(int kernelHalfWidth)
    {
        var convolutionKernel = new ConvolutionKernel(kernelHalfWidth);
        convolutionKernel.Coefficients[0] = 0d;
        convolutionKernel.Coefficients[1] = -1d;
        convolutionKernel.Coefficients[2] = 0d;
        convolutionKernel.Coefficients[3] = -1d;
        convolutionKernel.Coefficients[4] = 5d;
        convolutionKernel.Coefficients[5] = -1d;
        convolutionKernel.Coefficients[6] = 0d;
        convolutionKernel.Coefficients[7] = -1d;
        convolutionKernel.Coefficients[8] = 0d;

        return convolutionKernel;
    }
}
