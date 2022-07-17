using AprsPersistenceService.Interfaces;
using AprsPersistenceService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AprsPersistenceService
{
    public class DirewolfLogReader
    {
        protected string _path;
        protected bool _includeDebug;
        protected List<string> _bypassLines = new List<string>();
        protected IAprsPacketParser _aprsPacketParser;
        private readonly ILogger _logger;

        const char packetDemarcationCharacter = (char)10;   //0x0A LF
        private readonly string packetDemarcationString = Encoding.UTF8.GetString(new byte[] { 0x0A });
        private int sleepInterval = 5000;

        public DirewolfLogReader(
            DirewolfLogParserConfigs direwolfLogParserConfigs,
            IAprsPacketParser aprsPacketParser,
            ILogger logger)
        {

            if(direwolfLogParserConfigs == null)
            {
                throw new ArgumentNullException("DirewolfLogParserConfigs cannot be null!  Has the appsettings.json been moved?");
            }

            if(aprsPacketParser == null)
            {
                throw new ArgumentNullException("AprsPacketParser implementation passed to DirewolfLogReader must not be null!  Was IoC wireup successful?");
            }

            if(logger == null)
            {
                throw new ArgumentNullException("Implementation of ILogger has not been passed to DirewolfLogReader!  Was IoC wireup successful?");
            }
            
            this._path = direwolfLogParserConfigs.PacketCaptureFilePath;
            this._includeDebug = direwolfLogParserConfigs.DebugMode;

            if(direwolfLogParserConfigs.BypassLines != null && direwolfLogParserConfigs.BypassLines.Count > 0)
            {
                this._bypassLines = direwolfLogParserConfigs.BypassLines;
            }

            this._aprsPacketParser = aprsPacketParser;
            this._logger = logger;

            if(direwolfLogParserConfigs.TimeToYieldFinalMessageSeconds > 0)
            {
                sleepInterval = direwolfLogParserConfigs.TimeToYieldFinalMessageSeconds * 1000;
            }
            
            /*
            Task<IEnumerable<string>> pack = Task.Run<IEnumerable<string>>(() => ReadLogBlob(path));
            //var pack = ReadLogBlob(path);

            //pack.Wait();

            pack.Result.ToList().ForEach(x =>
            {
                Console.WriteLine("Read ToList().ForEach()");
                Console.WriteLine(x);
                var aprs = aprsPacketParser.ParseAprsPacket(x);
                Task persistTask = Task.Run(async () => await postgres.PersistAprs(aprs));
                persistTask.Wait();
            });
            */

            /*
            var pack = ReadLogBlob(path).ToList();

            pack.ForEach(x =>
            {
                Console.WriteLine("Read ToList().ForEach()");
                Console.WriteLine(x);
                var aprs = aprsPacketParser.ParseAprsPacket(x);
                Task persistTask = Task.Run(async () => await postgres.PersistAprs(aprs));
                persistTask.Wait();
            });
            */

            //read each line until we have an empty line, probably /n/n back to back, there may be 3-4 lines
        }

        public IEnumerable<AprsModel> ParseContinuously()
        {
            int blockLine = 0;
            List<string> _textblocks = new List<string>();

            foreach (string result in ReadLogBlob(_path))
            {
                //necessary to have enumeration only once?
                string aprsString = result;

                if (IsBypassLine(aprsString))
                {
                    continue;
                }

                if (IsNewPacket(aprsString))
                {
                    //finish parsing the last text block
                    if (blockLine > 0 && _textblocks.Count > 0)
                    {
                        var aprs = _aprsPacketParser.ParseAprsPacket(_textblocks);

                        if (_includeDebug)
                        {
                            _logger.LogDebug(Environment.NewLine);
                            _logger.LogDebug("JSON Structure of APRS...");
                            _logger.LogDebug(JsonConvert.SerializeObject(aprs));
                        }

                        _textblocks.Clear();

                        yield return aprs;
                    }

                    blockLine = 0;
                    continue;
                }

                blockLine++;

                if (_includeDebug)
                {
                    _logger.LogDebug($"Blockline: {blockLine}");
                    _logger.LogDebug($"Length of aprsString: {aprsString.Length}");
                    _logger.LogDebug($"APRS String: {aprsString}");
                }

                if (_includeDebug)
                {
                    StringBuilder sb = new StringBuilder();
                    Encoding.UTF8.GetBytes(aprsString).ToList().ForEach(x => sb.Append(x + " "));
                    _logger.LogDebug(sb.ToString());
                }

                _textblocks.Add(aprsString);
            }
        }

        private bool IsBypassLine(string line)
        {
            bool retval = false;

            if (_bypassLines.Any(x => line.Contains(x)))
            {
                if (_includeDebug)
                {
                    _logger.LogDebug($"Bypassing line '{line}'...");
                }

                retval = true;
            }

            return retval;
        }

        private bool IsNewPacket(string line)
        {
            //demarcates new packets
            return (line.Equals(packetDemarcationCharacter) || line.Equals(packetDemarcationString) || line.Length == 0) ? true : false;
        }

        public IEnumerable<string> ReadLogBlob(string path)
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                //tried ReadToEnd() and ReadLine(), created too much blank space
                string read = "";
                while (true)
                {
                    while (streamReader.EndOfStream)
                    {
                        Thread.Sleep(sleepInterval);
                        yield return packetDemarcationString;
                    }

                    read = streamReader.ReadLine();

                    if (_includeDebug)
                    {
                        _logger.LogDebug("Yielding Results...");
                    }
                    
                    yield return read;
                }
            }
        }
    }
}
