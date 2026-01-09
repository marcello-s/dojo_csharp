#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataTubeMap;

public abstract class GraphEdgeData
{
    public virtual double Weight
    {
        get { throw new NotImplementedException(); }
        protected set { throw new NotImplementedException(); }
    }
}
