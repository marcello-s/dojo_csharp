#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;
using System.Xml.Serialization;

namespace KataTubeMap;

public static class MapLoader
{
    public static TubeMap DeserializeFromFile(string currentDirectory = "")
    {
        var fileName = Path.Combine(currentDirectory, "TubeMap.xml");
        var serializer = new XmlSerializer(typeof(TubeMap));
        TubeMap map;
        using (var fs = new FileStream(fileName, FileMode.Open))
        {
            map = (TubeMap)(serializer.Deserialize(fs) ?? new TubeMap());
        }

        return map;
    }

    public static void SerializeToFile(TubeMap map, string fileName)
    {
        var serializer = new XmlSerializer(typeof(TubeMap));
        using (var ms = new MemoryStream())
        {
            serializer.Serialize(ms, map);
            var buffer = new byte[ms.Length];
            ms.Position = 0;
            var bytesRead = ms.Read(buffer, 0, buffer.Length);
            Console.WriteLine(Encoding.Default.GetString(buffer));
        }
    }
}

[Serializable]
public class TubeMap
{
    public TubeMap()
    {
        Lines = new List<Line>();
    }

    public List<Line> Lines { get; set; }
}

[Serializable]
public class Line
{
    public Line()
    {
        Stops = new List<Stop>();
    }

    [XmlAttribute]
    public string? Name { get; set; }

    public List<Stop> Stops { get; set; }
}

[Serializable]
public class Stop
{
    [XmlAttribute]
    public string? Name { get; set; }

    [XmlAttribute]
    public int Duration { get; set; }
}
