#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataMotionControl;

public class KinematicsTests
{
    [Test]
    public void CalculateDistanceFromSpeedAndConstantAcceleration_WithPositiveSpeed_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateDistanceFromSpeedAndConstantAcceleration(2.0, 4.0),
            Is.EqualTo(0.5).Within(0.0001)
        );
    }

    [Test]
    public void CalculateDistanceFromSpeedAndConstantAcceleration_WithNeagtiveSpeed_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateDistanceFromSpeedAndConstantAcceleration(-2.0, 4.0),
            Is.EqualTo(0.5).Within(0.0001)
        );
    }

    [Test]
    public void CalculateDistanceFromSpeedAndConstantAcceleration_WithNegativeAcceleration_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateDistanceFromSpeedAndConstantAcceleration(2.0, -4.0),
            Is.EqualTo(-0.5).Within(0.0001)
        );
    }

    [Test]
    public void CalculateDistanceFromSpeedAndConstantAcceleration_WithZeroAcceleratiion_ReturnsPositiveInfinty()
    {
        Assert.That(
            Kinematics.CalculateDistanceFromSpeedAndConstantAcceleration(2.0, 0.0),
            Is.EqualTo(double.PositiveInfinity)
        );
    }

    [Test]
    public void CalculateDistanceFromSpeedAndConstantAcceleration_WithZeroSpeed_ReturnsZero()
    {
        Assert.That(
            Kinematics.CalculateDistanceFromSpeedAndConstantAcceleration(0.0, 4.0),
            Is.EqualTo(0.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromDistanceAndConstantAcceleration_WithPositiveDistance_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateTimeFromDistanceAndConstantAcceleration(32.0, 4.0),
            Is.EqualTo(4.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromDistanceAndConstantAcceleration_WithNegativeDistance_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateTimeFromDistanceAndConstantAcceleration(-32.0, 4.0),
            Is.EqualTo(4.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromDistanceAndConstantAcceleration_WithZeroDistance_ReturnsZero()
    {
        Assert.That(
            Kinematics.CalculateTimeFromDistanceAndConstantAcceleration(0.0, 4.0),
            Is.EqualTo(0.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromDistanceAndConstantAcceleration_WithNegativeAcceleration_ReturnsNaN()
    {
        Assert.That(
            Kinematics.CalculateTimeFromDistanceAndConstantAcceleration(32.0, -4.0),
            Is.NaN
        );
    }

    [Test]
    public void CalculateTimeFromSpeedAndConstantAcceleration_WithPositiveSpeed_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateTimeFromSpeedAndConstantAcceleration(32.0, 4.0),
            Is.EqualTo(8.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromSpeedAndConstantAcceleration_WithNegativeSpeed_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateTimeFromSpeedAndConstantAcceleration(-32.0, 4.0),
            Is.EqualTo(-8.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromSpeedAndConstantAcceleration_WithZeroSpeed_ReturnsZero()
    {
        Assert.That(
            Kinematics.CalculateTimeFromSpeedAndConstantAcceleration(0.0, 4.0),
            Is.EqualTo(0.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromSpeedAndConstantAcceleration_WithNegativeAcceleration_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateTimeFromSpeedAndConstantAcceleration(32.0, -4.0),
            Is.EqualTo(-8.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromSpeedAndConstantAcceleration_WithZeroAcceleration_ReturnsPositiveInfinity()
    {
        Assert.That(
            Kinematics.CalculateTimeFromSpeedAndConstantAcceleration(32.0, 0.0),
            Is.EqualTo(double.PositiveInfinity)
        );
    }

    [Test]
    public void CalculateTimeFromDistanceAndSpeed_WithPositiveDistance_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateTimeFromDistanceAndSpeed(8.0, 4.0),
            Is.EqualTo(2.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromDistanceAndSpeed_WithNegativeDistance_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateTimeFromDistanceAndSpeed(-8.0, 4.0),
            Is.EqualTo(2.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromDistanceAndSpeed_WithNegativeSpeed_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateTimeFromDistanceAndSpeed(8.0, -4.0),
            Is.EqualTo(2.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromDistanceAndSpeed_WithNegatives_ReturnsCorrectResult()
    {
        Assert.That(
            Kinematics.CalculateTimeFromDistanceAndSpeed(-8.0, -4.0),
            Is.EqualTo(2.0).Within(0.0001)
        );
    }

    [Test]
    public void CalculateTimeFromDistanceAndSpeed_WithZeroSpeed_ReturnsPositiveInfinity()
    {
        Assert.That(
            Kinematics.CalculateTimeFromDistanceAndSpeed(8.0, 0.0),
            Is.EqualTo(double.PositiveInfinity)
        );
    }

    [Test]
    public void CalculateTimeFromDistanceAndSpeed_WithNegativesAndZeroSpeed_ReturnsPositiveInfinity()
    {
        Assert.That(
            Kinematics.CalculateTimeFromDistanceAndSpeed(-8.0, 0.0),
            Is.EqualTo(double.PositiveInfinity)
        );
    }
}
