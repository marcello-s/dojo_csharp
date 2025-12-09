#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataBootstrapper;

public static class LogManager
{
    static readonly IKernelLog NullKernelLogInstance = new NullLog();

    public static Func<Type, IKernelLog> GetLog = type => NullKernelLogInstance;

    private class NullLog : IKernelLog
    {
        public void Info(string format, params object[] args) { }

        public void Warn(string format, params object[] args) { }

        public void Error(Exception exception) { }
    }
}
