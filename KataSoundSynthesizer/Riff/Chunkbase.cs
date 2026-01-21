#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Riff;

class Chunkbase
{
    protected IList<ChunkField> fields = null!;
    protected IDictionary<string, object> valueMap = null!;

    public IEnumerable<ChunkField> Fields
    {
        get { return fields; }
    }

    public IDictionary<string, object> ValueMap
    {
        get { return valueMap; }
    }

    protected static IDictionary<string, object> CreateValueMapFromField(
        IEnumerable<ChunkField> fields
    )
    {
        var map = new Dictionary<string, object>();
        foreach (var field in fields)
        {
            map.Add(field.name, null!);
        }

        return map;
    }
}
