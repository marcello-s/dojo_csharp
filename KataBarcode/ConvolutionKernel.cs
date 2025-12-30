#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataBarcode;

struct ConvolutionKernel
{
    public int KernelHalfWidth { get; private set; }
    public int KernelSize { get; private set; }
    public double[] Coefficients { get; private set; }
    public bool NeedNormalization { get; private set; }
    public double NormalizationFactor { get; private set; }

    public ConvolutionKernel(int kernelHalfWidth)
        : this()
    {
        if (kernelHalfWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(
                "kernelHalfWidth",
                kernelHalfWidth,
                "must be > 0"
            );
        }

        KernelHalfWidth = kernelHalfWidth;
        KernelSize = 2 * kernelHalfWidth + 1;
        Coefficients = new double[KernelSize * KernelSize];
        NormalizationFactor = 1d;
    }

    public void CalculatateNormalization()
    {
        var sum = Coefficients.Sum();
        if (!(Math.Abs(sum - 0) > double.Epsilon))
        {
            return;
        }

        NormalizationFactor = 1.0 / sum;
        NeedNormalization = true;
    }
}
