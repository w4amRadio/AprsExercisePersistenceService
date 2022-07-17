using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprsPersistenceService.Models
{
    public class AprsModel
    {
        public string TncHeader { get; set; }
        public string AprsHeader { get; set; }
        public string Text { get; set; }
        public string RepeatingLocation { get; set; }
        public string Extra1 { get; set; }

        public bool FromDigipeater { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string TextMessage { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Course { get; set; }
        public string Altitude { get; set; }
        public string Radio { get; set; }
        public string Origin { get; set; }
        public string WeatherInformation { get; set; }
        public string GateVoltage { get; set; }

        public string Location
        {
            get
            {
                return Long + ", " + Lat;
            }
        }

        public Dictionary<string, string> lines = new Dictionary<string, string>();

        /*  JSON.NET chokes when trying to serialize this
        private ConcurrentDictionary<string, string> lines = new ConcurrentDictionary<string, string>();

        public void AddLine(string key, string value)
        {
            lines.TryAdd(key, value);
        }

        public Dictionary<string, string> Lines
        {
            get
            {
                return this.Lines;
            }
        }
        */
    }
}
