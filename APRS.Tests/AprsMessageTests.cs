using AprsPersistenceService;
using AprsPersistenceService.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APRS.Tests
{
    [TestClass]
    public class AprsMessageTests
    {
        string testString = "Digipeater WIDE2 (probably KA4EMA-3) audio level = 2(1/1)   [NONE]   __||||||_" + Environment.NewLine +
                            "[0.4] KD4YDD-1>APU25N,N4NE-2,KA4EMA-3,WIDE2*:=3403.77N/08355.46WIKD4YDD-1 Dacula, Ga iGate/Digi<0x0d>" + Environment.NewLine +
                            "Position, TcpIp on air network stn, UIview 32 bit apps" + Environment.NewLine +
                            "N 34 03.7700, W 083 55.4600" + Environment.NewLine +
                            "KD4YDD-1 Dacula, Ga iGate/Digi";

        string testCorpus = string.Empty;

        public AprsMessageTests()
        {
            testCorpus = File.ReadAllText("TestMessages.txt");
        }

        [TestMethod]
        public void TestMessageParsing()
        {
            AprsPacketParser aprsPacketParser = new AprsPacketParser(NullLogger.Instance, new PacketParserConfigs() { DebugMode = true });
            var result = aprsPacketParser.ParseAprsPacket(testString.Split(Environment.NewLine).ToList<string>());

            Assert.IsNotNull(result);
            //Assert.AreEqual();
        }
    }
}
