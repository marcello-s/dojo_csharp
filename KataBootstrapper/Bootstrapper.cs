#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.ExceptionServices;

namespace KataBootstrapper;

public abstract class Bootstrapper
{
    static readonly IKernelLog Log = LogManager.GetLog(typeof(Bootstrapper));
    protected static bool isInitialized;
    protected readonly bool useAppDomain;

    public AppDomain AppDomain { get; protected set; } = null!;

    protected Bootstrapper(bool useAppDomain = true)
    {
        if (isInitialized)
        {
            return;
        }

        this.useAppDomain = useAppDomain;
        isInitialized = true;
    }

    ~Bootstrapper()
    {
        DoShutdown();
        isInitialized = false;
    }

    public void Startup()
    {
        DoStartup();
        isInitialized = true;
    }

    public void Shutdown()
    {
        DoShutdown();
        isInitialized = false;
    }

    protected virtual void DoStartup()
    {
        Log.Info("bootstrapper startup");
        if (useAppDomain)
        {
            if (AppDomain == null)
            {
                AppDomain = AppDomain.CurrentDomain;
            }

            AppDomain.DomainUnload += OnDomainUnload;
            AppDomain.ProcessExit += OnProcessExit;
            AppDomain.UnhandledException += OnUnhandledException;
            AppDomain.FirstChanceException += OnFirstChanceException;
        }

        Configure();

        IoC.GetInstance = IocGetInstance;
        IoC.GetAllInstances = IocGetAllInstances;
        IoC.BuildUp = IocBuildUp;
        IoC.RegisterService = IocRegisterService;
        IoC.RegisterServiceSingleton = IocRegisterServiceSingleton;

        OnStartup();
        Log.Info("bootstrapper startup finished");
    }

    protected virtual void DoShutdown()
    {
        Log.Info("bootstrapper shutdown");
        if (AppDomain != null)
        {
            AppDomain.DomainUnload -= OnDomainUnload;
            AppDomain.ProcessExit -= OnProcessExit;
            AppDomain.UnhandledException -= OnUnhandledException;
            AppDomain.FirstChanceException -= OnFirstChanceException;
        }

        OnShutdown();
        Log.Info("bootstrapper shutdown finished");
    }

    protected virtual void OnDomainUnload(object? sender, EventArgs e) { }

    protected virtual void OnProcessExit(object? sender, EventArgs e) { }

    protected virtual void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            Log.Error(ex);
        }
    }

    protected virtual void OnFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        Log.Error(e.Exception);
    }

    protected abstract void Configure();

    protected virtual void OnStartup() { }

    protected virtual void OnShutdown() { }

    protected virtual object? IocGetInstance(Type service, string key)
    {
        return Activator.CreateInstance(service);
    }

    protected virtual IEnumerable<object> IocGetAllInstances(Type service)
    {
        return new object[] { Activator.CreateInstance(service)! };
    }

    protected virtual void IocBuildUp(object instance) { }

    protected virtual void IocRegisterService(Type interfaceType, Type service) { }

    protected virtual void IocRegisterServiceSingleton(Type interfaceType, Type service) { }
}

public abstract class Bootstrapper<T> : Bootstrapper
    where T : IFrameworkStart
{
    protected IFrameworkStart? frameworkStart = null!;

    protected override void OnStartup()
    {
        frameworkStart = IoC.Get<T>();
        if (frameworkStart != null)
        {
            frameworkStart.Start();
        }
    }

    protected override void OnShutdown()
    {
        if (frameworkStart != null)
        {
            frameworkStart.Stop();
        }
    }
}
