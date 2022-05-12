using AprsPersistenceService.Interfaces;
using AprsPersistenceService.Models;
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

        public AprsPacketParser(PacketParserConfigs packetParserConfigs)
        {
            DebugMode = packetParserConfigs == null ? false : packetParserConfigs.DebugMode;
        }

        public AprsPacketParser(bool debugMode)
        {
            DebugMode = debugMode;
        }

        public AprsModel ParseAprsPacket(string[] textBlock)
        {
            AprsModel retval = new AprsModel();

            if (DebugMode)
            {
                Console.WriteLine("Parsing APRS packet...");
            }

            //tnc header
            if(textBlock.Length >= 1)
            {
                retval.TncHeader = textBlock[0];

                //does this line contain the word digipeater?
                retval.FromDigipeater = retval.TncHeader.Contains("Digipeater ", StringComparison.CurrentCultureIgnoreCase);

                if (DebugMode)
                {
                    Console.WriteLine($"TNC Header: {retval.TncHeader}");
                    Console.WriteLine($"Is From Digipeater: {retval.FromDigipeater}");
                }
            }

            if (textBlock.Length >= 2)
            { 

                retval.AprsHeader = textBlock[1]; 

                if (DebugMode)
                {
                    Console.WriteLine($"APRS Header: {retval.AprsHeader}");
                }
                
            }

            //Location might also be in line 3
            if (textBlock.Length >= 3)
            {

                retval.Text = textBlock[2];

                if (DebugMode)
                {
                    Console.WriteLine($"Text Line: {retval.Text}");
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
                    Console.WriteLine($"Repeating Location: {retval.RepeatingLocation}");
                    Console.WriteLine($"Lattitude: {retval.Lat}");
                    Console.WriteLine($"Longitude: {retval.Long}");
                }
            }

            //Location might be in line 5, need a function or regex to look for it
            if(textBlock.Length >= 5) 
            {
                retval.Extra1 = textBlock[4];

                if(DebugMode)
                {
                    Console.WriteLine($"Extra1: {retval.Extra1}");
                }
            }

            return retval;
        }
    }
}
