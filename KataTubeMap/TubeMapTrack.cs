#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataTubeMap;

public class TubeMapTrack : GraphEdgeData
{
    public override double Weight { get; protected set; }

    public ISet<Tuple<int, string>> Lines { get; set; }

    public TubeMapTrack(double weight)
    {
        Weight = weight;
        Lines = new HashSet<Tuple<int, string>>();
    }
}
