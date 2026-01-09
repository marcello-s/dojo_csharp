#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataBootstrapper;

public class FrameworkRoot : IFrameworkStart
{
    readonly ILogService logService;

    public FrameworkRoot(ILogService logService)
    {
        if (logService == null)
        {
            throw new ArgumentNullException("logService");
        }

        this.logService = logService;
    }

    public void Start()
    {
        logService.Info("framework booted successfully");
    }

    public void Stop()
    {
        logService.Info("framework shutdown successfully");
    }
}
