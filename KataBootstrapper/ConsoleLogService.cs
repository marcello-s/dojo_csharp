#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataBootstrapper;

public class ConsoleLogService : ILogService
{
    public void Info(string format, params object[] args)
    {
        Console.Write("{0} [CONSOLE LOG] ", "INFO:");
        Console.WriteLine(format, args);
    }
}
