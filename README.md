# Serilog.Sinks.Logentries

[![Build status](https://ci.appveyor.com/api/projects/status/4tfqyatqgioc1njc/branch/master?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-logentries/branch/master)

A Serilog sink that writes events to Logentries.

[Logentries](http://www.logentries.com) allows you to collect machine statistics and log files. 
In your Logentries dashboard, select the option to create a new log and pick the .NET log type. You will see details about log4net and nlog which you can ignore. At the bottom, there is a button to configure your app. Clicking on this button will reveal a token in the form of a guid. This token you need to enter in the Logentries configuration. By default the sink will use a TCP token using SSL.

**Package** - [Serilog.Sinks.Logentries](http://nuget.org/packages/serilog.sinks.logentries)
| **Platforms** - .NET 4.5

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Logentries("token")
    .CreateLogger();
```

The properties are not send to Logentries, it only consists of the message which you can configure.
