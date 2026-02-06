# KataWPF

## WpfApp Demo

The app showcases the use of ViewModelLib, a simple MVVM library. It it includes a UserControls library that includes 
touchscreen buttons. The demo includes a center area for screens and a navigation sidebar.

### Demo screens

- welcome screen
- slider control
- error overview
- result screen

|![welcome screen](/docs/images/1_welcome_screen.png)|![slider control](/docs/images/2_slider_control.png)|
|----------------------------------------------------|----------------------------------------------------|
|![error overview](/docs/images/3_error_overview.png)|![result screen](/docs/images/4_result_screen.png)  |

## Setup

https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-sln

Create a solution
```dotnet new sln```

Migrate to slnx format
```dotnet sln migrate```

Add a new project to the solution
```
dotnet new classlib --output project
dotnet sln add project
```

Add a WPF class library
```
dotnet new wpflib --output ViewModelLib
dotnet add package System.ComponentModel.Composition
```

Add WPF application
```
dotnet new wpf --output WpfApp
cd WpfApp
dotnet add reference ..\ViewModelLib\ViewModelLib.csproj
```

UserControl Library
```
dotnet new wpfusercontrollib --output AppShell.Controls
```

Add a package dependency, https://www.nuget.org/
```
dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package Microsoft.NET.Test.Sdk
```

Add project reference
```
dotnet add reference ..\path\project.csproj
```


Build & Run, in the project folder
```
dotnet build
dotnet run
dotnet test --logger "console;verbosity=detailed"
```

SDK upgrade
```
dotnet clean
dotnet new globaljson --sdk-version 10.0.101 --roll-forward latestFeature
dotnet workload restore
```
update <TargetFramework> in .csproj to new version
