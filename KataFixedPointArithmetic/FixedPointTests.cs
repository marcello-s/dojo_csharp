#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataFixedPointArithmetic;

[TestFixture]
public class FixedPointTests
{
    private FixedPoint fixedPoint;

    [SetUp]
    public void Setup()
    {
        fixedPoint = new FixedPoint();
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(fixedPoint, Is.InstanceOf<FixedPoint>());
    }

    [Test]
    public void CanCreate_InstanceWithIntValue()
    {
        var f = new FixedPoint(123);
        Assert.That(f, Is.InstanceOf<FixedPoint>());
    }

    [Test]
    public void CanCreate_InstanceWithDoubleValue()
    {
        var f = new FixedPoint(12.345);
        Assert.That(f, Is.InstanceOf<FixedPoint>());
    }

    [Test]
    public void ToInt_WithPositiveValue_CorrectValue()
    {
        var f = new FixedPoint(123);
        var fint = f.ToInt();
        Assert.That(fint, Is.EqualTo(123));
    }

    [Test]
    public void ToInt_WithZero_CorrectValue()
    {
        var f = new FixedPoint(0);
        var fint = f.ToInt();
        Assert.That(fint, Is.EqualTo(0));
    }

    [Test]
    public void ToInt_WithNegativeValue_CorrectValue()
    {
        var f = new FixedPoint(-123);
        var fint = f.ToInt();
        Assert.That(fint, Is.EqualTo(-123));
    }

    [Test]
    public void ToDouble_WithPositiveValue_CorrectValue()
    {
        var f = new FixedPoint(12.345);
        var fdouble = f.ToDouble();
        Assert.That(Math.Round(fdouble, 3), Is.EqualTo(12.345));
    }

    [Test]
    public void ToDouble_WithZero_CorrectValue()
    {
        var f = new FixedPoint(0.0);
        var fdouble = f.ToDouble();
        Assert.That(Math.Round(fdouble, 1), Is.EqualTo(0.0));
    }

    [Test]
    public void ToDouble_WithNegativeValue_CorrectValue()
    {
        var f = new FixedPoint(-12.345);
        var fdouble = f.ToDouble();
        Assert.That(Math.Round(fdouble, 3), Is.EqualTo(-12.345));
    }

    [Test]
    public void Inverse_WithPositiveValue_CorrectValue()
    {
        var f = new FixedPoint(12.345);
        var finverse = f.Inverse;
        Assert.That(Math.Round(finverse.ToDouble(), 3), Is.EqualTo(-12.345));
    }

    [Test]
    public void Inverse_WithZero_CorrectValue()
    {
        var f = new FixedPoint(0.0);
        var finverse = f.Inverse;
        Assert.That(Math.Round(finverse.ToDouble(), 1), Is.EqualTo(0.0));
    }

    [Test]
    public void Inverse_WithNegativeValue_CorrectValue()
    {
        var f = new FixedPoint(-12.345);
        var finverse = f.Inverse;
        Assert.That(Math.Round(finverse.ToDouble(), 3), Is.EqualTo(12.345));
    }

    [Test]
    public void ExplicitDoubleConversion_CorrectValue()
    {
        var f = new FixedPoint(12.345);
        var fexplicitdouble = (double)f;
        Assert.That(Math.Round(fexplicitdouble, 3), Is.EqualTo(12.345));
    }

    [Test]
    public void ImplicitDoubleConversion_CorrectValue()
    {
        FixedPoint f = 12.345;
        Assert.That(Math.Round(f.ToDouble(), 3), Is.EqualTo(12.345));
    }

    [Test]
    public void UnaryPlusOperator_CorrectValue()
    {
        var f = new FixedPoint(12.345);
        var fplus = +f;
        Assert.That(Math.Round(fplus.ToDouble(), 3), Is.EqualTo(12.345));
    }

    [Test]
    public void BinaryPlusOperator_CorrectValue()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        var fsum = f1 + f2;
        Assert.That(Math.Round(fsum.ToDouble(), 3), Is.EqualTo(35.801));
    }

    [Test]
    public void UnaryMinusOperator_CorrectValue()
    {
        var f = new FixedPoint(12.345);
        var fminus = -f;
        Assert.That(Math.Round(fminus.ToDouble(), 3), Is.EqualTo(-12.345));
    }

    [Test]
    public void BinaryMinusOperator_CorrectValue()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        var fdifference = f1 - f2;
        Assert.That(Math.Round(fdifference.ToDouble(), 3), Is.EqualTo(-11.111));
    }

    [Test]
    public void BinaryMultiplicationOperator_CorrectValue()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        var fmultiplication = f1 * f2;
        Assert.That(Math.Round(fmultiplication.ToDouble(), 3), Is.EqualTo(289.564));
    }

    [Test]
    public void BinaryDivisionOperator_CorrectValue()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        var fdivision = f1 / f2;
        Assert.That(Math.Round(fdivision.ToDouble(), 4), Is.EqualTo(0.5263));
    }

    [Test]
    public void BinaryModuloOperator_CorrectValue()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        var fmodulo = f2 % f1;
        Assert.That(Math.Round(fmodulo.ToDouble(), 3), Is.EqualTo(11.111));
    }

    [Test]
    public void EqualsOperator_WithTwoEqualValues_True()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(12.345);
        var fequal = f1 == f2;
        Assert.That(fequal, Is.True);
    }

    [Test]
    public void EqualsOperator_WithTwoNotEqualValues_False()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        var fequal = f1 == f2;
        Assert.That(fequal, Is.False);
    }

    [Test]
    public void GreaterThanOperator_WithF1GreaterThanF2_True()
    {
        var f1 = new FixedPoint(23.456);
        var f2 = new FixedPoint(12.345);
        var fgreaterthan = f1 > f2;
        Assert.That(fgreaterthan, Is.True);
    }

    [Test]
    public void GreaterThanOperator_WithF2GreaterThanF1_False()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        var fgreaterthan = f1 > f2;
        Assert.That(fgreaterthan, Is.False);
    }

    [Test]
    public void LessThanOperator_WithF1LessThanF2_True()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        var flessthan = f1 < f2;
        Assert.That(flessthan, Is.True);
    }

    [Test]
    public void LessThanOperator_WithF2LessThanF1_False()
    {
        var f1 = new FixedPoint(23.456);
        var f2 = new FixedPoint(12.345);
        var flessthan = f1 < f2;
        Assert.That(flessthan, Is.False);
    }

    [Test]
    public void GreaterOrEqualThanOperator_WithF1GreaterThanF2_True()
    {
        var f1 = new FixedPoint(23.456);
        var f2 = new FixedPoint(12.345);
        var fgreaterorequal = f1 >= f2;
        Assert.That(fgreaterorequal, Is.True);
    }

    [Test]
    public void GreaterOrEqualThanOperator_WithF1EqualF2_True()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(12.345);
        var fgreaterorequal = f1 >= f2;
        Assert.That(fgreaterorequal, Is.True);
    }

    [Test]
    public void GreaterOrEqualThanOperator_WithF2GreaterF1_False()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        var fgreaterorequal = f1 >= f2;
        Assert.That(fgreaterorequal, Is.False);
    }

    [Test]
    public void LessOrEqualThanOperator_WithF1LessThanF2_True()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        var flessorequal = f1 <= f2;
        Assert.That(flessorequal, Is.True);
    }

    [Test]
    public void LessOrEqualThanOperator_WithF1EqualF2_True()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(12.345);
        var flessorequal = f1 <= f2;
        Assert.That(flessorequal, Is.True);
    }

    [Test]
    public void LessOrEqualThanOperator_WithF2LessThanF1_False()
    {
        var f1 = new FixedPoint(23.456);
        var f2 = new FixedPoint(12.345);
        var flessorequal = f1 <= f2;
        Assert.That(flessorequal, Is.False);
    }

    [Test]
    public void Equals_WithSameObject_True()
    {
        var f = new FixedPoint(12.345);
        Assert.That(f.Equals(f), Is.True);
    }

    [Test]
    public void Equals_WithSameValue_True()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(12.345);
        Assert.That(f1.Equals(f2), Is.True);
    }

    [Test]
    public void Equals_WithDifferentObject_False()
    {
        var f1 = new FixedPoint(12.345);
        var f2 = new FixedPoint(23.456);
        Assert.That(f1.Equals(f2), Is.False);
    }

    [Test]
    public void Equals_WithSameDoubleValue_False()
    {
        var f = new FixedPoint(12.345);
        Assert.That(f.Equals(12.345), Is.False);
    }

    [Test]
    public void GetHashCode_809042()
    {
        var f = new FixedPoint(12.345);
        Assert.That(f.GetHashCode(), Is.EqualTo(809042));
    }

    [Test]
    public void ToString_809042()
    {
        var f = new FixedPoint(12.345);
        Assert.That(f.ToString(), Is.EqualTo("809042"));
    }

    [Test]
    public void MaxValue_32768()
    {
        Assert.That(FixedPoint.MaxValue, Is.EqualTo(32768));
    }

    [Test]
    public void MinValue_neg32768()
    {
        Assert.That(FixedPoint.MinValue, Is.EqualTo(-32768));
    }

    [Test]
    public void ReachMaxInt_32768()
    {
        var max = 0L;
        while (true)
        {
            max++;
            var f = new FixedPoint(max);
            if (Math.Abs(Convert.ToDouble(max) - f.ToDouble()) > double.Epsilon)
            {
                break;
            }
        }

        Assert.That(max, Is.EqualTo(32768));
    }

    [Test]
    public void ReachMinInt_neg32769()
    {
        var min = 0L;
        while (true)
        {
            min--;
            var f = new FixedPoint(min);
            if (Math.Abs(Convert.ToDouble(min) - f.ToDouble()) > double.Epsilon)
            {
                break;
            }
        }

        Assert.That(min, Is.EqualTo(-32769));
    }

    [Test]
    public void ReachMaxDouble_32768()
    {
        var max = 0.0;
        while (true)
        {
            max += 0.5;
            var f = new FixedPoint(max);
            if (Math.Abs(max - f.ToDouble()) > double.Epsilon)
            {
                break;
            }
        }

        Assert.That(max, Is.EqualTo(32768.0));
    }

    [Test]
    public void ReachMinDouble_neg32768Point5()
    {
        var min = 0.0;
        while (true)
        {
            min -= 0.5;
            var f = new FixedPoint(min);
            if (Math.Abs(min - f.ToDouble()) > double.Epsilon)
            {
                break;
            }
        }

        Assert.That(min, Is.EqualTo(-32768.5));
    }

    [Test]
    public void ReachEpsilon()
    {
        var one = new FixedPoint(1);
        const int factor = 10;
        var divisorMagnitude = 1;
        var epsilon = new FixedPoint(0);
        var previous = new FixedPoint(2);
        while (true)
        {
            epsilon = one / new FixedPoint(divisorMagnitude);
            if (epsilon >= previous)
            {
                break;
            }

            divisorMagnitude *= factor;
            previous = epsilon;
        }

        Assert.That(1.0 / divisorMagnitude, Is.EqualTo(1.0 / 1000000));
    }
}
