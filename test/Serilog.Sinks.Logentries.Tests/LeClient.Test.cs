using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Sinks.Logentries;

namespace Serilog.Sinks.Logentries.Tests
{
    [TestFixture]
    public class LeClientTests
    {
        [SetUp]
        public void Initialize()
        {

        }

        [Test]
        public void TestTcpTokenWriteHttp()
        {
            var client = new LeClient(false, false);

            ExecuteTest(client);
        }

        [Test]
        public void TestTcpTokenWriteHttps()
        {
            var client = new LeClient(false, true);

            ExecuteTest(client);
        }

        [Test]
        public void TestHttpPutWriteHttp()
        {
            var client = new LeClient(true, false, accountKey: "", hostKey: "General Tests", logKey: "");

            ExecuteTest(client);
        }

        [Test]
        public void TestHttpPutWriteHttps()
        {
            var client = new LeClient(true, true, accountKey: "", hostKey:"General Tests", logKey: "");

            ExecuteTest(client);
        }

        void ExecuteTest(LeClient client)
        {
            client.Connect();
            client.Write("605885c4-a4d4-42c7-a3f4-f2f97b124ca1" + " HTTPS test log entry\n");
            client.Flush();
            client.Close();
        }
    }
}
