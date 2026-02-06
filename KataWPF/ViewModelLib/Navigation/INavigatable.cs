#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Navigation;

public interface INavigatable
{
    INavigatable Previous { get; set; }
    INavigatable Next { get; set; }
    object? Target { get; }
    bool IsActive { get; set; }
}
