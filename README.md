# Serilog.Sinks.Logentries

[![Build status](https://ci.appveyor.com/api/projects/status/ha18n45fe6wh7v9s/branch/dev?svg=true)](https://ci.appveyor.com/project/agecas/serilog-sinks-logentries/branch/dev)

A Serilog sink that writes events to Logentries.

[Logentries](http://www.logentries.com) allows you to collect machine statistics and log files. 
In your Logentries dashboard, select the option to create a new log and pick the .NET log type. You will see details about log4net and nlog which you can ignore. At the bottom, there is a button to configure your app. Clicking on this button will reveal a token in the form of a guid. This is the token you need to enter in the Logentries configuration. By default the sink will use a TCP token over SSL.

**Package** - [Serilog.Sinks.Logentries](http://nuget.org/packages/serilog.sinks.logentries)
| **Platforms** - .NET 4.5 / .NET Standard 1.3

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Logentries("token")
    .CreateLogger();
```

Properties are not sent to Logentries.  Only the message, which you can configure, is sent.
