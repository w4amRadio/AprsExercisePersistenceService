using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprsPersistenceService.Models
{
    public class DirewolfLogParserConfigs
    {
        public string PacketCaptureFilePath { get; set; }
        public bool DebugMode { get; set; }
        public int TimeToYieldFinalMessageSeconds { get; set; }
        public List<string> BypassLines { get; set; }
    }
}
