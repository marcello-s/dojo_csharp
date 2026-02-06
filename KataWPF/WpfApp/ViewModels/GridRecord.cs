#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ViewModelLib;
using WpfApp.State;

namespace WpfApp.ViewModels;

public class GridRecord : ViewModelBase, IEditableObject
{
    private ProcessingData data;

    public GridRecord()
    {
        data = new ProcessingData();
    }

    public GridRecord(ProcessingData data)
    {
        this.data = data;
    }

    public ProcessingData Data
    {
        get { return data; }
    }

    public int Id
    {
        get { return data.Id; }
        set
        {
            data.Id = value;
            NotifyOfPropertyChange(() => Id);
        }
    }

    [CustomValidation(typeof(GridRecord), "BarcodeValidation")]
    public string Barcode
    {
        get { return data.Barcode; }
        set
        {
            data.Barcode = value;
            NotifyOfPropertyChange(() => Barcode);
        }
    }

    [CustomValidation(typeof(GridRecord), "TaraValidation")]
    public double Tara
    {
        get { return data.Tara; }
        set
        {
            data.Tara = value;
            NotifyOfPropertyChange(() => Tara);
        }
    }

    [CustomValidation(typeof(GridRecord), "SolidWeightValidation")]
    public double SolidWeight
    {
        get { return data.SolidWeight; }
        set
        {
            data.SolidWeight = value;
            NotifyOfPropertyChange(() => SolidWeight);
        }
    }

    public double Weight
    {
        get { return Math.Round(data.SolidWeight - data.Tara, 3); }
    }

    [CustomValidation(typeof(GridRecord), "TargetConcentration_mMolValidation")]
    public double TargetConcentration_mMol
    {
        get { return data.TargetConcentration_mMol; }
        set
        {
            data.TargetConcentration_mMol = value;
            NotifyOfPropertyChange(() => TargetConcentration_mMol);
        }
    }

    [CustomValidation(typeof(GridRecord), "TargetConcentration_mgmlValidation")]
    public double TargetConcentration_mgml
    {
        get { return data.TargetConcentration_mgml; }
        set
        {
            data.TargetConcentration_mgml = value;
            NotifyOfPropertyChange(() => TargetConcentration_mgml);
        }
    }

    [CustomValidation(typeof(GridRecord), "CompoundMolarWeightValidation")]
    public double CompoundMolarWeight
    {
        get { return data.CompoundMolarWeight; }
        set
        {
            data.CompoundMolarWeight = value;
            NotifyOfPropertyChange(() => CompoundMolarWeight);
        }
    }

    public string CompoundId
    {
        get { return data.CompoundId; }
        set
        {
            data.CompoundId = value;
            NotifyOfPropertyChange(() => CompoundId);
        }
    }

    public string Status
    {
        get { return data.Status; }
        set
        {
            data.Status = value;
            NotifyOfPropertyChange(() => Status);
        }
    }

    public string Processing
    {
        get { return data.Processing; }
        set
        {
            data.Processing = value;
            NotifyOfPropertyChange(() => Processing);
        }
    }

    public delegate void ItemEndEditEventDelegate(IEditableObject sender);
    public event ItemEndEditEventDelegate ItemEndEdit = delegate { };

    public void BeginEdit() { }

    public void CancelEdit() { }

    public void EndEdit()
    {
        ItemEndEdit(this);
    }

    public void NotifyView()
    {
        NotifyOfPropertyChange(() => Status);
        NotifyOfPropertyChange(() => Processing);
    }

    public void TaraNotifyView()
    {
        NotifyView();
        NotifyOfPropertyChange(() => Barcode);
        NotifyOfPropertyChange(() => Tara);
    }

    public void WeighSolidNotifyView()
    {
        NotifyView();
        NotifyOfPropertyChange(() => Barcode);
        NotifyOfPropertyChange(() => Tara);
        NotifyOfPropertyChange(() => SolidWeight);
    }

    public void DiluteNotifyView()
    {
        NotifyView();
        NotifyOfPropertyChange(() => TargetConcentration_mMol);
        NotifyOfPropertyChange(() => TargetConcentration_mgml);
        NotifyOfPropertyChange(() => CompoundMolarWeight);
    }

    public static ValidationResult? BarcodeValidation(string value, ValidationContext context)
    {
        var record = context.ObjectInstance as GridRecord;
        ProcessingDataValidation.ValidateBarcode(record!.Data);
        if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
        {
            return new ValidationResult(record.data.Status, new[] { "Barcode" });
        }

        return ValidationResult.Success;
    }

    public static ValidationResult? TaraValidation(double value, ValidationContext context)
    {
        var record = context.ObjectInstance as GridRecord;
        ProcessingDataValidation.ValidateRegisterTara(record!.Data);
        if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
        {
            return new ValidationResult(record.data.Status, new[] { "Tara" });
        }

        return ValidationResult.Success;
    }

    public static ValidationResult? SolidWeightValidation(double value, ValidationContext context)
    {
        var record = context.ObjectInstance as GridRecord;
        ProcessingDataValidation.ValidateRegisterWeight(record!.Data);
        if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
        {
            return new ValidationResult(record.data.Status, new[] { "SolidWeight" });
        }

        return ValidationResult.Success;
    }

    public static ValidationResult? TargetConcentration_mMolValidation(
        double value,
        ValidationContext context
    )
    {
        var record = context.ObjectInstance as GridRecord;
        ProcessingDataValidation.ValidateTargetConcentration_mMolLtZero(record!.Data);
        if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
        {
            return new ValidationResult(record.data.Status, new[] { "TargetConcentration_mMol" });
        }

        return ValidationResult.Success;
    }

    public static ValidationResult? TargetConcentration_mgmlValidation(
        double value,
        ValidationContext context
    )
    {
        var record = context.ObjectInstance as GridRecord;
        ProcessingDataValidation.ValidateTargetConcentration_mgmlLtZero(record!.Data);
        if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
        {
            return new ValidationResult(record.data.Status, new[] { "TargetConcentration_mgml" });
        }

        return ValidationResult.Success;
    }

    public static ValidationResult? CompoundMolarWeightValidation(
        double value,
        ValidationContext context
    )
    {
        var record = context.ObjectInstance as GridRecord;
        ProcessingDataValidation.ValidateCompoundMolarWeight(record!.Data);
        if (record.Data.Processing != ProcessingDataValidation.PROCESSING_PROCESS)
        {
            return new ValidationResult(record.data.Status, new[] { "CompoundMolarWeight" });
        }

        return ValidationResult.Success;
    }
}
