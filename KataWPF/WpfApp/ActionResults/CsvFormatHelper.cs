#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;
using System.Text.RegularExpressions;
using WpfApp.State;

namespace WpfApp.ActionResults;

public class CsvFormatHelper
{
    public const string Delimiter = ";";

    public static string RenderTaraExport(IEnumerable<ProcessingData> data)
    {
        var columns = new List<string> { "Barcode", "Tara" };

        var rows = new List<List<string>>();
        foreach (var record in data)
        {
            var rowData = new List<string> { record.Barcode, record.Tara.ToString() };
            rows.Add(rowData);
        }

        return RenderCSV(columns, rows);
    }

    public static string RenderWeighSolidExport(IEnumerable<ProcessingData> data)
    {
        var columns = new List<string> { "Barcode", "Tara", "Weight" };

        var rows = new List<List<string>>();
        foreach (var record in data)
        {
            var rowData = new List<string>
            {
                record.Barcode,
                record.Tara.ToString(),
                record.SolidWeight.ToString(),
            };

            rows.Add(rowData);
        }

        return RenderCSV(columns, rows);
    }

    public static string RenderDiluteExport(
        IEnumerable<ProcessingData> data,
        IEnumerable<SampleInfo> sampleInfo
    )
    {
        var columns = new List<string>
        {
            "Barcode",
            "RackLabel",
            "Position",
            "Concentration",
            "Volume",
            "CompoundId",
            "Processed",
        };

        var rows = new List<List<string>>();
        foreach (var record in data)
        {
            string racklabel = string.Empty;
            if (sampleInfo.Any((s) => s.SampleId == record.Id))
            {
                racklabel = sampleInfo.Where((s) => s.SampleId == record.Id).Single().RackLabel;
            }

            var rowData = new List<string>
            {
                record.Barcode,
                racklabel,
                record.Id.ToString(),
                record.TargetConcentration_mMol > 0.0
                    ? record.TargetConcentration_mMol.ToString()
                    : record.TargetConcentration_mgml.ToString(),
                (record.Volume_l * Math.Pow(10, 6)).ToString(),
                record.CompoundId,
                record.Processing.ToLower().Equals("process") == true
                    ? true.ToString()
                    : false.ToString(),
            };

            rows.Add(rowData);
        }

        return RenderCSV(columns, rows);
    }

    public static IList<ProcessingData> ParseTaraImport(string text)
    {
        IList<string> columns = null!;
        List<List<string>> rows = null!;
        ScanCSV(text, out columns, out rows);

        // check header for Barcode and Tara
        var hasId = columns.Contains("Barcode");
        var hasTara = columns.Contains("Tara");

        if (!(hasId && hasTara))
        {
            throw new Exception("The first row must contain the fields: \"Barcode\" and \"Tara\"");
        }

        var data = new List<ProcessingData>();
        foreach (var rowData in rows)
        {
            data.Add(
                new ProcessingData()
                {
                    Barcode = rowData[columns.IndexOf("Barcode")],
                    Tara = double.Parse(rowData[columns.IndexOf("Tara")]),
                }
            );
        }

        return data;
    }

    public static IList<ProcessingData> ParseWeighSolidImport(string text)
    {
        IList<string> columns = null!;
        List<List<string>> rows = null!;
        ScanCSV(text, out columns, out rows);

        // check header for Barcode, Tara and Weight
        var hasId = columns.Contains("Barcode");
        var hasTara = columns.Contains("Tara");
        var hasWeight = columns.Contains("Weight");

        if (!(hasId && hasTara && hasWeight))
        {
            throw new Exception(
                "The first row must contain the fields: \"Barcode\", \"Tara\" and \"Weight\""
            );
        }

        var data = new List<ProcessingData>();
        foreach (var rowData in rows)
        {
            data.Add(
                new ProcessingData()
                {
                    Barcode = rowData[columns.IndexOf("Barcode")],
                    Tara = double.Parse(rowData[columns.IndexOf("Tara")]),
                    SolidWeight = double.Parse(rowData[columns.IndexOf("Weight")]),
                }
            );
        }

        return data;
    }

    public static IList<ProcessingData> ParseDiluteImport(string text)
    {
        IList<string> columns = null!;
        List<List<string>> rows = null!;
        ScanCSV(text, out columns, out rows);

        // check header for Barcode, Concentration_mol and Molar Weight, or Concentration mg/ml
        var hasId = columns.Contains("Barcode");
        var hasCxmr = columns.Contains("CxmMol");
        var hasMolarWeight = columns.Contains("MolarWeight");
        var hasCxmgml = columns.Contains("Cxmgml");
        var hasCompoundId = columns.Contains("CompoundId");

        if (!(hasId && (hasCxmr && hasMolarWeight) || (hasCxmgml)))
        {
            throw new Exception(
                "The first row must contain the fields: \"Barcode\", \"CxmMol\" and \"MolarWeight\", or \"Cxmgml\""
            );
        }

        var data = new List<ProcessingData>();
        foreach (var rowData in rows)
        {
            data.Add(
                new ProcessingData()
                {
                    Barcode = rowData[columns.IndexOf("Barcode")],
                    TargetConcentration_mMol = hasCxmr
                        ? double.Parse(rowData[columns.IndexOf("CxmMol")])
                        : 0.0,
                    CompoundMolarWeight = hasMolarWeight
                        ? double.Parse(rowData[columns.IndexOf("MolarWeight")])
                        : 0.0,
                    TargetConcentration_mgml =
                        hasCxmgml && !hasCxmr && !hasMolarWeight
                            ? double.Parse(rowData[columns.IndexOf("Cxmgml")])
                            : 0.0,
                    CompoundId =
                        hasCompoundId == true
                            ? rowData[columns.IndexOf("CompoundId")]
                            : string.Empty,
                }
            );
        }

        return data;
    }

    public static string RenderCSV(IList<string> columns, List<List<string>> rows)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(string.Join(Delimiter, columns.ToArray()));
        foreach (var rowData in rows)
        {
            sb.AppendLine(string.Join(Delimiter, rowData.ToArray()));
        }

        return sb.ToString();
    }

    public static void ScanCSV(string text, out IList<string> columns, out List<List<string>> rows)
    {
        // split into lines
        Regex regex = new Regex(Environment.NewLine);
        var lines = regex.Split(text);
        if (lines.GetUpperBound(0) <= 1)
        {
            throw new Exception("incorrect file format");
        }

        // split column header
        columns = new List<string>();
        regex = new Regex(Delimiter);
        var header = regex.Split(lines[0]);
        foreach (string column in header)
        {
            columns.Add(column);
        }

        // split data rows into lists
        rows = new List<List<string>>();
        for (int i = 1; i < lines.GetUpperBound(0); i++)
        {
            var rowData = new List<string>();
            var line = regex.Split(lines[i]);
            foreach (string value in line)
            {
                rowData.Add(value);
            }

            rows.Add(rowData);
        }
    }
}
