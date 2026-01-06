#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataMotionControl;

public static class Kinematics
{
    public static double CalculateDistanceFromSpeedAndConstantAcceleration(
        double speed,
        double constantAcceleration
    )
    {
        // V=a*t; S=a/2*t^2 => S=a/2*(V/a)^2 = V^2/(2*a)
        return Math.Pow(speed, 2.0) / (2.0 * constantAcceleration);
    }

    public static double CalculateTimeFromDistanceAndConstantAcceleration(
        double distance,
        double constantAcceleration
    )
    {
        // S=a/2*t^2 => t=sqrt(2*S/a)
        return Math.Sqrt(2.0 * Math.Abs(distance) / constantAcceleration);
    }

    public static double CalculateTimeFromSpeedAndConstantAcceleration(
        double speed,
        double constantAcceleration
    )
    {
        return speed / constantAcceleration;
    }

    public static double CalculateTimeFromDistanceAndSpeed(double distance, double speed)
    {
        return Math.Abs(distance / speed);
    }
}
