# Proc File System

The Linux proc filesystem is an interface to interact with the kernel. 

## Install Dotnet on Linux
Used Ubuntu 20.04 - [Install .NET on Linux
](https://docs.microsoft.com/en-us/dotnet/core/install/linux).

## Build, Publish and Execute
Navigate to solution and use `dotnet` cli commands to build and publish - [Publish using `dotnet`](https://docs.microsoft.com/en-us/dotnet/core/deploying/deploy-with-cli#framework-dependent-executable).

```bash
cd Solution
dotnet build
dotnet publish -c Release -r linux-x64 --no-self-contained # If non-Framework dependent application required --self-contained flag should be used
```

Executable will be generated in `ProcFS/bin/Release/net6.0/linux-x64/publish` directory.
```bash
cd ./ProcFS/bin/Release/net6.0/linux-x64/publish
./ProcFS
```

## Test and coverage
MSTest with Moq framework is used for testing - [Unit test using MSTest](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows#integrate-with-net-test). Tests can be executed resulting in coverage report. `Coverlet Cobertura` is used for generation of ceverage reports.

```bash
dotnet test --collect:"XPlat Code Coverage"
```
Report can be generated to be viewed in HTML format - [Coeverage report](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows#generate-reports)
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool # to be executed only once
reportgenerator -reports:"ProcFS.Tests/TestResults/0a711f04-378c-4214-a04a-7e4c9773d0c4/coverage.cobertura.xml" -targetdir:"ProcFS.Tests/TestResults/coveragereport" -reporttypes:Html
```
