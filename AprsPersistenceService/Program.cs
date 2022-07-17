using AprsPersistenceService.Models;
using AprsPersistenceService.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading.Tasks;
using AprsPersistenceService.Interfaces;
using Serilog;
using Serilog.Extensions.Logging;

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

            var serilogLogger = Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var serilogFactory = new SerilogLoggerFactory(serilogLogger);

            Microsoft.Extensions.Logging.ILogger logger = serilogFactory.CreateLogger<Program>();

            try
            {

                var direwolfLogParserConfigs = config.GetSection($"Settings:{nameof(DirewolfLogParserConfigs)}")?.Get<DirewolfLogParserConfigs>();
                var packetParserConfigs = config.GetSection($"Settings:{nameof(PacketParserConfigs)}")?.Get<PacketParserConfigs>();
                var postgresConfigs = config.GetSection($"Settings:{nameof(PostgresConfigs)}")?.Get<PostgresConfigs>();
                bool debugObjectModel = config.GetValue<bool>("DebugObjectModel");

                //TODO: Use serilog for logging and log based on ms standard

                PostgresDao postgres = new PostgresDao(postgresConfigs: postgresConfigs, logger: logger);

                IAprsPacketParser aprsPacketParser = new AprsPacketParser(serilogFactory.CreateLogger<AprsPacketParser>(), packetParserConfigs);
                DirewolfLogReader direwolfLogReader = new DirewolfLogReader(direwolfLogParserConfigs, aprsPacketParser, logger);
                foreach (AprsModel model in direwolfLogReader.ParseContinuously())
                {
                    AprsModel localModel = model;       //because yield return

                    if (debugObjectModel)
                    {
                        logger.LogDebug(Environment.NewLine);
                        logger.LogDebug(JsonConvert.SerializeObject(localModel));
                        logger.LogDebug(Environment.NewLine);

                        logger.LogDebug($"Converted Latitude: {LatLongParser.ConvertLatitude(localModel.Lat)}");
                        logger.LogDebug($"Longitude Original: {localModel.Long}");
                        logger.LogDebug($"Converted Longitude: {LatLongParser.ConvertLongitude(localModel.Long)}");
                        logger.LogDebug(Environment.NewLine);
                    }

                    //Task.Run(() => postgres.PersistAprs(localModel)).Wait();
                    await postgres.PersistAprs(localModel);
                }
            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "Exception occured...");
            }

            //Task<string> result = Task.Run<string>(() => direwolfLogReader.ReadString(path));

            Console.ReadLine();

            Log.Debug("Finished shutting down app.");

            Log.CloseAndFlush();

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
