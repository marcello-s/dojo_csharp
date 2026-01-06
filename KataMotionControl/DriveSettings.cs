#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataMotionControl;

public record DriveSettings(
    double IncrementPerMillimeterResolution,
    double SpeedMinimumInIncrementsPerMilliSecond,
    double SpeedMaximumInIncrementsPerMilliSecond,
    double AccelerationMinimumInIncrementsPerMilliSecond2,
    double AccelerationMaximumInIncrementsPerMilliSecond2
);
