using AprsPersistenceService;
using AprsPersistenceService.Interfaces;
using AprsPersistenceService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APRS.Tests
{
    [TestClass]
    public class DirewolfLogReaderTests
    {
        protected DirewolfLogReader direwolfLogReader;


        public DirewolfLogReaderTests()
        {
            List<string> byPassList = new List<string>()
            {
                "Dire Wolf version",
                "Includes optional support for:",
                "Reading config file ",
                "Audio device for both receive and transmit:",
                "Channel 0: 1200 baud, AFSK 1200 & 2200 Hz",
                "Note: PTT not configured for channel 0. (Ignore this if using VOX.)",
                "Ready to accept AGW client application",
                "Ready to accept KISS client application",
                "Virtual KISS TNC is available on",
                "Created symlink",
                "KISS SEND - Discarding message because no one is listening.",
                "This happens when you use the -p option and don't read from the pseudo terminal."
            };

            DirewolfLogParserConfigs configs = new DirewolfLogParserConfigs()
            {
                BypassLines = byPassList,
                DebugMode = true,
                TimeToYieldFinalMessageSeconds = 5,
                PacketCaptureFilePath = "TestMessages.txt"
            };

            IAprsPacketParser packetParser = new AprsPacketParser(true);

            direwolfLogReader = new DirewolfLogReader(
                direwolfLogParserConfigs: configs,
                aprsPacketParser: packetParser,
                logger: NullLogger.Instance);
        }

        [TestMethod]
        public void TestParseContinuously()
        {
            if(direwolfLogReader == null)
            {
                throw new AssertFailedException("DirewolfLogReader has not been set!  Has it been instantiated?");
            }

            foreach (AprsModel model in direwolfLogReader.ParseContinuously())
            {
                AprsModel localModel = model;       //because yield return

                Assert.IsNotNull(localModel);
            }
        }
    }
}
