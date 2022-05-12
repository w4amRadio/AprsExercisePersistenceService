using AprsPersistenceService.Models;
using AprsPersistenceService.Utils;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading.Tasks;
using AprsPersistenceService.Interfaces;

namespace AprsPersistenceService
{
    public class Program
    {
        public async static Task<int> Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder();

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var direwolfLogParserConfigs = config.GetSection($"Settings:{nameof(DirewolfLogParserConfigs)}")?.Get<DirewolfLogParserConfigs>();
            var packetParserConfigs = config.GetSection($"Settings:{nameof(PacketParserConfigs)}")?.Get<PacketParserConfigs>();
            var postgresConfigs = config.GetSection($"Settings:{nameof(PostgresConfigs)}")?.Get<PostgresConfigs>();
            bool debugObjectModel = config.GetValue<bool>("DebugObjectModel");

            //TODO: Use serilog for logging and log based on ms standard

            PostgresDao postgres = new PostgresDao(postgresConfigs: postgresConfigs);

            IAprsPacketParser aprsPacketParser = new AprsPacketParser(packetParserConfigs);
            DirewolfLogReader direwolfLogReader = new DirewolfLogReader(direwolfLogParserConfigs, aprsPacketParser);
            foreach(AprsModel model in direwolfLogReader.ParseContinuously())
            {
                AprsModel localModel = model;       //because yield return

                if (debugObjectModel)
                {
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine(JsonConvert.SerializeObject(localModel));
                    Console.WriteLine(Environment.NewLine);

                    Console.WriteLine($"Converted Latitude: {LatLongParser.ConvertLatitude(localModel.Lat)}");
                    Console.WriteLine($"Longitude Original: {localModel.Long}");
                    Console.WriteLine($"Converted Longitude: {LatLongParser.ConvertLongitude(localModel.Long)}");
                    Console.WriteLine(Environment.NewLine);
                }

                //Task.Run(() => postgres.PersistAprs(localModel)).Wait();
                await postgres.PersistAprs(localModel);
            }

            //Task<string> result = Task.Run<string>(() => direwolfLogReader.ReadString(path));

            Console.ReadLine();

            return Environment.ExitCode;
        }

        /*
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    configuration.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
        */
    }
}
