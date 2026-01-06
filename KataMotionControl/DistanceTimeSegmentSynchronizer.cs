#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataMotionControl;

public class DistanceTimeSegmentSynchronizer
{
    public IEnumerable<IEnumerable<IntermediaryDistanceTimeSegment>> SyncToLongestTime(
        IEnumerable<IEnumerable<IntermediaryDistanceTimeSegment>> segmentsList
    )
    {
        // There are lists that contain 2 or 3 segments. The 2 segments
        // may become 3 segments when speed max can be reached.
        // They may also be padded with zero distance segments.

        return null;
    }
}
