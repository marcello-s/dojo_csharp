#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Xml.Linq;

namespace WpfApp.Services;

[Export(typeof(IDataStore))]
public class XmlDataStore : IDataStore
{
    private static object @lock = new object();
    private const string RecordParent = "ProcessingDataRecords";
    private const string RecordElement = "D";
    private const string PrimaryKey = "b";

    public string StoreLocation { private get; set; } = string.Empty;

    public void Save(object item)
    {
        CheckLocation(StoreLocation);
        XElement? e = item as XElement;
        var record = QueryRecordByAttribute(
            StoreLocation,
            PrimaryKey,
            e!.Attribute(PrimaryKey)!.Value
        );

        if (record != null)
        {
            UpdateRecordByAttribute(StoreLocation, PrimaryKey, e);
        }
        else
        {
            InsertRecord(StoreLocation, e);
        }
    }

    public object? Load(object key)
    {
        CheckLocation(StoreLocation);

        return QueryRecordByAttribute(StoreLocation, PrimaryKey, key as string ?? string.Empty);
    }

    public static void CheckLocation(string location)
    {
        if (!System.IO.File.Exists(location))
        {
            throw new ArgumentException("location does not exist", "location");
        }
    }

    public static object? QueryRecordByAttribute(
        string location,
        string attributeName,
        string value
    )
    {
        lock (@lock)
        {
            XDocument doc = XDocument.Load(location);
            XElement? root = doc.Root;
            var records =
                from d in root?.Descendants(RecordElement).AsParallel()
                where d.Attribute(attributeName)!.Value.Equals(value)
                select d;

            if (records == null || records.Count() == 0)
            {
                return null;
            }

            return records.First();
        }
    }

    public static void InsertRecord(string location, XElement record)
    {
        lock (@lock)
        {
            XDocument doc = XDocument.Load(location);
            doc?.Root?.Element(RecordParent)!.Add(record);
            doc?.Save(location);
        }
    }

    public static void UpdateRecordByAttribute(
        string location,
        string attributeName,
        XElement record
    )
    {
        lock (@lock)
        {
            XDocument doc = XDocument.Load(location);
            XElement? root = doc.Root;
            var records =
                from d in root?.Descendants(RecordElement).AsParallel()
                where
                    d.Attribute(attributeName)!.Value.Equals(record.Attribute(attributeName)!.Value)
                select d;
            var item = records.First();
            foreach (var a in item.Attributes())
            {
                a.Value = record.Attribute(a.Name)!.Value;
            }

            doc.Save(location);
        }
    }
}
