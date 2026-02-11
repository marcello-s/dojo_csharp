#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Xml.Linq;
using WpfApp.State;

namespace WpfApp.ActionResults;

public class DataStoreMapper
{
    public static object MapToDatastore(ProcessingData data)
    {
        var record = new XElement("D");
        record.SetAttributeValue("b", data.Barcode);
        record.SetAttributeValue("t", data.Tara.ToString());
        record.SetAttributeValue("w", data.SolidWeight.ToString());

        return record;
    }

    public static ProcessingData? MapFromDatastore(object item)
    {
        if (item is not XElement record)
        {
            return null;
        }

        var data = new ProcessingData()
        {
            Barcode = record.Attribute("b")!.Value,
            Tara = double.Parse(record.Attribute("t")!.Value),
            SolidWeight = double.Parse(record.Attribute("w")!.Value),
        };

        return data;
    }
}
