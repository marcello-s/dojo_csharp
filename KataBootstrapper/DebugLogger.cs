#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Diagnostics;

namespace KataBootstrapper;

internal class DebugKernelLog : IKernelLog
{
    public void Info(string format, params object[] args)
    {
        Debug.Write("[" + DateTime.Now.ToString("o") + "] ", "INFO");
        Debug.WriteLine(format, args);
    }

    public void Warn(string format, params object[] args)
    {
        Debug.Write("[" + DateTime.Now.ToString("o") + "] ", "WARN");
        Debug.WriteLine(format, args);
    }

    public void Error(Exception exception)
    {
        Debug.Write("[" + DateTime.Now.ToString("o") + "] ", "ERROR");
        Debug.WriteLine(exception.Message);
    }
}
