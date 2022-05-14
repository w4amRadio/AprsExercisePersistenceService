using AprsPersistenceService.Interfaces;
using AprsPersistenceService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprsPersistenceService
{
    //TODO: Refactor this, needs to be more stronk
    public class AprsPacketParser : IAprsPacketParser
    {
        public bool DebugMode { get; set; }
        private ILogger _logger; 

        public AprsPacketParser(ILogger logger, PacketParserConfigs packetParserConfigs)
        {
            if(logger == null)
            {
                throw new ArgumentNullException("ILogger passed into AprsPacketParser cannot be null!  Has it been instantiated?");
            }

            DebugMode = packetParserConfigs == null ? false : packetParserConfigs.DebugMode;
            this._logger = logger;
        }

        public AprsModel ParseAprsPacket(string[] textBlock)
        {
            AprsModel retval = new AprsModel();

            if (DebugMode)
            {
                _logger.LogDebug("Parsing APRS packet...");
            }

            //tnc header
            if(textBlock.Length >= 1)
            {
                retval.TncHeader = textBlock[0];

                //does this line contain the word digipeater?
                retval.FromDigipeater = retval.TncHeader.Contains("Digipeater ", StringComparison.CurrentCultureIgnoreCase);

                if (DebugMode)
                {
                    _logger.LogDebug($"TNC Header: {retval.TncHeader}");
                    _logger.LogDebug($"Is From Digipeater: {retval.FromDigipeater}");
                }
            }

            if (textBlock.Length >= 2)
            { 

                retval.AprsHeader = textBlock[1]; 

                if (DebugMode)
                {
                    _logger.LogDebug($"APRS Header: {retval.AprsHeader}");
                }
                
            }

            //Location might also be in line 3
            if (textBlock.Length >= 3)
            {

                retval.Text = textBlock[2];

                if (DebugMode)
                {
                    _logger.LogDebug($"Text Line: {retval.Text}");
                }
            }

            //Location might be in line 4
            if (textBlock.Length >= 4)
            {

                //Location must include a N, E, S, W
                retval.RepeatingLocation = textBlock[3];

                var latLongArray = retval.RepeatingLocation.Split(',');

                retval.Lat = latLongArray[0].TrimStart();
                retval.Long = latLongArray[1].TrimStart();

                if (DebugMode)
                {
                    _logger.LogDebug($"Repeating Location: {retval.RepeatingLocation}");
                    _logger.LogDebug($"Lattitude: {retval.Lat}");
                    _logger.LogDebug($"Longitude: {retval.Long}");
                }
            }

            //Location might be in line 5, need a function or regex to look for it
            if(textBlock.Length >= 5) 
            {
                retval.Extra1 = textBlock[4];

                if(DebugMode)
                {
                    _logger.LogDebug($"Extra1: {retval.Extra1}");
                }
            }

            return retval;
        }
    }
}
