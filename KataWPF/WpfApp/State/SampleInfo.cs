#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace WpfApp.State;

public struct SampleInfo
{
    public int SampleId { get; set; }
    public string RackLabel { get; set; }
    public int RackPosition { get; set; }
    public int Grid { get; set; }
    public int Site { get; set; }
}
