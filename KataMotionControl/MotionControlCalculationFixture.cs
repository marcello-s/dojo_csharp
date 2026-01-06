#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataMotionControl;

[TestFixture]
public class MotionControlCalculationFixture
{
    private MotionControlCalculator motionControlCalculator = null!;

    [SetUp]
    public void Setup()
    {
        var settings = new DriveSettings(13.888888888889, 0.1, 9300, 0.1, 1500);
        motionControlCalculator = new MotionControlCalculator(
            settings.SpeedMinimumInIncrementsPerMilliSecond,
            settings.SpeedMaximumInIncrementsPerMilliSecond,
            settings.AccelerationMaximumInIncrementsPerMilliSecond2
        );
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(motionControlCalculator, Is.InstanceOf<MotionControlCalculator>());
    }

    [Test]
    public void CanCreate_WithSpeedMinimumHigherSpeedMaximum_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new MotionControlCalculator(9300, 0.1, 1500));
    }

    [Test]
    public void CanCreate_WithAccelerationLessThenZero_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new MotionControlCalculator(0.1, 9300, -0.1));
    }

    [Test]
    public void CalculateDistanceTimeSegments_WithSpeedMaximumLessThanZero_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            motionControlCalculator.CalculateDistanceTimeSegments(290, -0.1)
        );
    }

    [Test]
    public void CalculateDistanceTimeSegments_WithSpeedMaximumGreaterThaneOne_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            motionControlCalculator.CalculateDistanceTimeSegments(290, 1.1)
        );
    }

    private static void PrintIntermediaryDistanceTimeSegments(
        IEnumerable<IntermediaryDistanceTimeSegment> segments
    )
    {
        segments
            .ToList()
            .ForEach(segment =>
                Console.WriteLine(
                    "distance: {0}, time: {1}",
                    segment.DistanceInMillimeters,
                    segment.TimeInSeconds
                )
            );
    }

    [Test]
    public void CalculateDistanceTimeSegments1Test()
    {
        var distanceTimeSegments = motionControlCalculator.CalculateDistanceTimeSegments(290, 1.0);
        var segments = distanceTimeSegments.ToList();
        PrintIntermediaryDistanceTimeSegments(segments);
        Assert.That(segments, Is.Not.Null);
        Assert.That(segments.Count(), Is.EqualTo(2));
        Assert.That(segments.First().DistanceInMillimeters, Is.EqualTo(145));
        Assert.That(
            Math.Round(segments.First().TimeInSeconds, 6),
            Is.EqualTo(Math.Round(0.439696865275764, 6))
        );
        Assert.That(segments.Last().DistanceInMillimeters, Is.EqualTo(145));
        Assert.That(
            Math.Round(segments.First().TimeInSeconds, 6),
            Is.EqualTo(Math.Round(0.439696865275764, 6))
        );
    }

    [Test]
    public void CalculateDistanceTimeSegments2Test()
    {
        var distanceTimeSegments = motionControlCalculator.CalculateDistanceTimeSegments(-290, 1.0);
        var segments = distanceTimeSegments.ToList();
        PrintIntermediaryDistanceTimeSegments(segments);
        Assert.That(segments, Is.Not.Null);
        Assert.That(segments.Count(), Is.EqualTo(2));
        Assert.That(segments.First().DistanceInMillimeters, Is.EqualTo(-145));
        Assert.That(
            Math.Round(segments.First().TimeInSeconds, 6),
            Is.EqualTo(Math.Round(0.439696865275764, 6))
        );
        Assert.That(segments.Last().DistanceInMillimeters, Is.EqualTo(-145));
        Assert.That(
            Math.Round(segments.First().TimeInSeconds, 6),
            Is.EqualTo(Math.Round(0.439696865275764, 6))
        );
    }

    [Test]
    public void CalculateDistanceTimeSegments3Test()
    {
        var distanceTimeSegments = motionControlCalculator.CalculateDistanceTimeSegments(
            28830 * 3,
            1.0
        );
        var segments = distanceTimeSegments.ToList();
        PrintIntermediaryDistanceTimeSegments(segments);
        Assert.That(segments, Is.Not.Null);
        Assert.That(segments.Count(), Is.EqualTo(3));
        Assert.That(segments.First().DistanceInMillimeters, Is.EqualTo(28830));
        Assert.That(Math.Round(segments.First().TimeInSeconds, 6), Is.EqualTo(Math.Round(3.1, 6)));
        Assert.That(segments.ElementAt(1).DistanceInMillimeters, Is.EqualTo(28830));
        Assert.That(
            Math.Round(segments.ElementAt(1).TimeInSeconds, 6),
            Is.EqualTo(Math.Round(3.1, 6))
        );
        Assert.That(segments.Last().DistanceInMillimeters, Is.EqualTo(28830));
        Assert.That(Math.Round(segments.Last().TimeInSeconds, 6), Is.EqualTo(Math.Round(3.1, 6)));
    }

    [Test]
    public void CalculateDistanceTimeSegments4Test()
    {
        var distanceTimeSegments = motionControlCalculator.CalculateDistanceTimeSegments(
            -28830 * 3,
            1.0
        );
        var segments = distanceTimeSegments.ToList();
        PrintIntermediaryDistanceTimeSegments(segments);
        Assert.That(segments, Is.Not.Null);
        Assert.That(segments.Count(), Is.EqualTo(3));
        Assert.That(segments.First().DistanceInMillimeters, Is.EqualTo(-28830));
        Assert.That(Math.Round(segments.First().TimeInSeconds, 6), Is.EqualTo(Math.Round(3.1, 6)));
        Assert.That(segments.ElementAt(1).DistanceInMillimeters, Is.EqualTo(-28830));
        Assert.That(
            Math.Round(segments.ElementAt(1).TimeInSeconds, 6),
            Is.EqualTo(Math.Round(3.1, 6))
        );
        Assert.That(segments.Last().DistanceInMillimeters, Is.EqualTo(-28830));
        Assert.That(Math.Round(segments.Last().TimeInSeconds, 6), Is.EqualTo(Math.Round(3.1, 6)));
    }
}
