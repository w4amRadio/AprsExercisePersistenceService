using AprsPersistenceService.Interfaces;
using AprsPersistenceService.Models;
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

        public DirewolfLogReader(DirewolfLogParserConfigs direwolfLogParserConfigs, IAprsPacketParser aprsPacketParser)
        {

            if(direwolfLogParserConfigs == null)
            {
                throw new ArgumentNullException("DirewolfLogParserConfigs cannot be null!  Has the appsettings.json been moved?");
            }

            if(aprsPacketParser == null)
            {
                throw new ArgumentNullException("AprsPacketParser implementation passed to DirewolfLogReader must not be null!  Was IoC wireup successful?");
            }
            
            this._path = direwolfLogParserConfigs.PacketCaptureFilePath;
            this._includeDebug = direwolfLogParserConfigs.DebugMode;

            if(direwolfLogParserConfigs.BypassLines != null && direwolfLogParserConfigs.BypassLines.Count > 0)
            {
                this._bypassLines = direwolfLogParserConfigs.BypassLines;
            }

            this._aprsPacketParser = aprsPacketParser;

            //Timer logTimer = new Timer();

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
            List<string> textblocks = new List<string>();

            foreach (string result in ReadLogBlob(_path))
            {
                //TODO: need to include timer to add /n when nothing comes back so the last packet is processed or change how this is processed

                //necessary to have enumeration only once?
                string aprsString = result;

                if(_bypassLines.Any(x => aprsString.Contains(x)))
                {
                    if (_includeDebug)
                    {
                        Console.WriteLine($"Bypassing line '{aprsString}'...");
                    }

                    continue;
                }

                /*
                if (aprsString.Contains("Dire Wolf version") ||
                    aprsString.Contains("Includes optional support for:") ||
                    aprsString.Contains("Reading config file ") ||
                    aprsString.Contains("Audio device for both receive and transmit:") ||
                    aprsString.Contains("Channel 0: 1200 baud, AFSK 1200 & 2200 Hz") ||
                    aprsString.Contains("Note: PTT not configured for channel 0. (Ignore this if using VOX.)") ||
                    aprsString.Contains("Ready to accept AGW client application") ||
                    aprsString.Contains("Ready to accept KISS client application") ||
                    aprsString.Contains("Virtual KISS TNC is available on") ||
                    aprsString.Contains("Created symlink"))
                {
                    Console.WriteLine("Bypassing line...");
                    continue;
                }
                */

                //this demarcates new packets
                if (aprsString.Equals((char)10) || aprsString.Length == 0)       //0x0A LF
                {
                    //finish parsing the last text block
                    if (blockLine > 0 && textblocks.Count > 0)
                    {
                        var aprs = _aprsPacketParser.ParseAprsPacket(textblocks.ToArray());

                        if (_includeDebug)
                        {
                            Console.WriteLine(Environment.NewLine);
                            Console.WriteLine("JSON Structure of APRS...");
                            Console.WriteLine(JsonConvert.SerializeObject(aprs));
                        }

                        yield return aprs;
                    }

                    blockLine = 0;
                    continue;
                }

                blockLine++;

                if (_includeDebug)
                {
                    Console.WriteLine($"Blockline: {blockLine}");
                    Console.WriteLine($"Length of aprsString: {aprsString.Length}");
                }

                Console.WriteLine(aprsString);

                if (_includeDebug)
                {
                    Encoding.UTF8.GetBytes(aprsString).ToList().ForEach(x => Console.Write(x + " "));
                }

                textblocks.Add(aprsString);
            }
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
                        Thread.Sleep(1000);
                    }

                    read = streamReader.ReadLine();
                    //Console.WriteLine(read);

                    Console.WriteLine("Yielding Results...");
                    yield return read;
                }
            }
        }
    }
}
