#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataMotionControl;

public class MotionControlCalculator
{
    private readonly double speedMaximum;
    private readonly double speedMinimum;
    private readonly double acceleration;

    public MotionControlCalculator(double speedMinimum, double speedMaximum, double acceleration)
    {
        if (speedMinimum > speedMaximum)
        {
            throw new ArgumentException("must be less or equal to speed maximum", "speedMinimum");
        }

        if (acceleration < 0.0)
        {
            throw new ArgumentException("must not be negative", "acceleration");
        }

        this.speedMinimum = speedMinimum;
        this.speedMaximum = speedMaximum;
        this.acceleration = acceleration;
    }

    public IEnumerable<IntermediaryDistanceTimeSegment> CalculateDistanceTimeSegments(
        double distanceInMillimeters,
        double speedMaximumInPercent
    )
    {
        if (speedMaximumInPercent < 0.0 || speedMaximumInPercent > 1.0)
        {
            throw new ArgumentOutOfRangeException(
                "speedMaximumInPercent",
                speedMaximumInPercent,
                "range: 0.0-1.0"
            );
        }

        var speed = ScaleSpeedToPercentage(speedMaximum, speedMaximumInPercent);
        // are we below speed minimum?
        speed = Math.Max(speed, speedMinimum);
        var acceleration = this.acceleration;

        // 1) calculate the distance for the speed maximum
        var distanceAtSpeedMaximum = Kinematics.CalculateDistanceFromSpeedAndConstantAcceleration(
            speed,
            acceleration
        );

        var segmentList = DistanceIsToShortToReachSpeedMaximum(
            distanceAtSpeedMaximum,
            distanceInMillimeters
        )
            ? Calculate2DistanceTimeSegments(distanceInMillimeters, acceleration)
            : Calculate3DistanceTimeSegments(
                distanceInMillimeters,
                distanceAtSpeedMaximum,
                speed,
                acceleration
            );

        // rounding to increments and milliseconds should be done when converting to increments resolution
        return segmentList;
    }

    private static double ScaleSpeedToPercentage(double speed, double percentage)
    {
        return speed * percentage;
    }

    private static bool DistanceIsToShortToReachSpeedMaximum(
        double distanceAtSpeedMaximum,
        double distance
    )
    {
        return 2.0 * distanceAtSpeedMaximum > Math.Abs(distance);
    }

    private static IEnumerable<IntermediaryDistanceTimeSegment> Calculate2DistanceTimeSegments(
        double totalDistance,
        double acceleration
    )
    {
        // vMax --------
        // | v   /\
        // |    /  \
        // |   /    \
        // |  /s1  s2\
        // --+---+----+---- t
        //   --t1--t2--

        // total distance is to short to reach vMax for the given acceleration
        // 1) the total distance is halfed (area s1)
        // 2) calculate segment time t1 from s1 and acceleration
        // 3) the second segment is then duplicated from s1
        // -drive acceleration is transmitted the same way for both segments,
        // -unless the drive has got a break

        // calculate by halfing the distance
        var segmentDistance1 = totalDistance / 2.0;
        var segmentTime = Kinematics.CalculateTimeFromDistanceAndConstantAcceleration(
            segmentDistance1,
            acceleration
        );
        var segmentDistance2 = totalDistance - segmentDistance1;

        return new List<IntermediaryDistanceTimeSegment>
        {
            new IntermediaryDistanceTimeSegment(segmentDistance1, segmentTime),
            new IntermediaryDistanceTimeSegment(segmentDistance2, segmentTime),
        };
    }

    private static IEnumerable<IntermediaryDistanceTimeSegment> Calculate3DistanceTimeSegments(
        double totalDistance,
        double distanceAtSpeedMaximum,
        double speed,
        double acceleration
    )
    {
        // vMax---+---+--
        // |     /|   |\
        // |    / |   | \
        // |   /  |   |  \
        // |  /s1 |s2 | s3\
        // --+----+---+---- t
        //   --t1--t2---t3-

        // vMax is reached for the given acceleration
        // 1) calculate segment distance s1
        // 2) calculate segment time t1 from speed (Vmax) and acceleration (triangle)
        // 3) calculate segment distance s2 from total distance and duplicated segment s1
        // 4) calculate segment time t2 from distance s2 and speed
        // 5) for segment s3/t3 segment s1/t1 is duplicated

        var distanceSign = Math.Sign(totalDistance);
        var segmentDistance1 = distanceAtSpeedMaximum * distanceSign;
        var segmentTime1 =
            Kinematics.CalculateTimeFromSpeedAndConstantAcceleration(speed, acceleration) / 2.0;
        var segmentDistance2 = CalculateMiddleSegmentDistance(totalDistance, segmentDistance1);
        var segmentTime2 = Kinematics.CalculateTimeFromDistanceAndSpeed(segmentDistance2, speed);
        var segmentDistance3 = segmentDistance1;
        var segmentTime3 = segmentTime1;
        return new List<IntermediaryDistanceTimeSegment>
        {
            new IntermediaryDistanceTimeSegment(segmentDistance1, segmentTime1),
            new IntermediaryDistanceTimeSegment(segmentDistance2, segmentTime2),
            new IntermediaryDistanceTimeSegment(segmentDistance3, segmentTime3),
        };
    }

    private static double CalculateMiddleSegmentDistance(
        double totalDistance,
        double segmentDistance
    )
    {
        return totalDistance - (2.0 * segmentDistance);
    }
}
