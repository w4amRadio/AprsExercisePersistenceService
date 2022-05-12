using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprsPersistenceService.Models
{
    public class PostgresConfigs
    {
        public string Host { get; set; }
        public uint Port { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool DebugMode { get; set; }

    }
}
