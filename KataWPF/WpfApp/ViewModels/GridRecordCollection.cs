#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Collections.ObjectModel;
using System.ComponentModel;
using WpfApp.State;

namespace WpfApp.ViewModels;

public class GridRecordCollection : ObservableCollection<GridRecord>
{
    public delegate void RecordEndEditEventDelegate(IEditableObject sender);
    public event RecordEndEditEventDelegate RecordEndEdit = null!;

    protected override void InsertItem(int index, GridRecord item)
    {
        base.InsertItem(index, item);
        item.ItemEndEdit += new GridRecord.ItemEndEditEventDelegate(item_ItemEndEdit);
    }

    void item_ItemEndEdit(IEditableObject sender)
    {
        ValidateDuplicates(this, sender as GridRecord ?? new GridRecord());
        if (RecordEndEdit != null)
        {
            RecordEndEdit(sender);
        }
    }

    public static void ValidateDuplicates(GridRecordCollection collection, GridRecord record)
    {
        var remainingRecords = collection.SkipWhile((r) => !r.Equals(record)).Skip(1);
        var duplicates = remainingRecords.Where((r) => r.Barcode.Equals(record!.Barcode));
        foreach (var duplicate in duplicates)
        {
            ProcessingDataValidation.SetDuplicateBarcode(duplicate.Data);
            duplicate.NotifyView();
        }

        if (duplicates.Count() == 0)
        {
            duplicates = collection.Where(
                (r) => r.Status.Equals(ProcessingDataValidation.STATUS_DUPLICATE_BARCODE)
            );

            foreach (var duplicate in duplicates)
            {
                ProcessingDataValidation.AssumeValidRecord(duplicate.Data);
                duplicate.NotifyView();
            }
        }
    }
}
