# dojo_csharp
Excercises to excel at the programming language .NET C#

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

Add a package dependency, https://www.nuget.org/
```
dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package Microsoft.NET.Test.Sdk
```

Build & Run, in the project folder
```
dotnet build
dotnet run
dotnet test --logger "console;verbosity=detailed"
```
