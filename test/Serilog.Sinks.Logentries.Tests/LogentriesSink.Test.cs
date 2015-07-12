using NUnit.Framework;
using Serilog.Tests.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Logentries.Tests
{
    [TestFixture]
    public class LogentriesSinkTests
    {
        [SetUp]
        public void Initialize()
        {

        }

        [Test]
        public void TestLogWrite()
        {
            var log = new LoggerConfiguration()
                .WriteTo.Logentries("", outputTemplate: "[{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            log.Write(Some.InformationEvent());

            System.Threading.Thread.Sleep(1000);
        }
    }
}
