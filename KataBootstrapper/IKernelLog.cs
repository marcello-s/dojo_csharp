#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataBootstrapper;

public interface IKernelLog
{
    void Info(string format, params object[] args);
    void Warn(string format, params object[] args);
    void Error(Exception exception);
}
