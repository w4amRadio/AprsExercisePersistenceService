using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AprsPersistenceService.Models;
using AprsPersistenceService.Utils;
using Npgsql;

namespace AprsPersistenceService
{
    public class PostgresDao
    {
        private string _connectionString = string.Empty;
        protected bool _includeDebug = false;

        public PostgresDao(PostgresConfigs postgresConfigs)
        {
            this._includeDebug = postgresConfigs.DebugMode;
            this._connectionString = CreateConnectionString(postgresConfigs);
        }

        private string CreateConnectionString(PostgresConfigs postgresConfigs)
        {
            return $"Server={postgresConfigs.Host};Port={postgresConfigs.Port};Database={postgresConfigs.Database};User Id={postgresConfigs.UserId};Password={postgresConfigs.Password};";
        }

        public async Task PersistTestApris(AprsModel aprs)
        {
            await using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                if (_includeDebug)
                {
                    Console.WriteLine("Opening Connection...");
                }

                await connection.OpenAsync();

                if (_includeDebug)
                {
                    Console.WriteLine("Connection opened.");
                    Console.WriteLine("Beginning insert statement...");
                }

                await using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO public.\"EnterpriseSouth2022Aprs\" (\"Timestamp\", \"TncHeader\") VALUES (@Timestamp, @TncHeader)", connection)
                {
                    Parameters =
                    {
                        new NpgsqlParameter("Timestamp", DateTime.UtcNow),
                        new NpgsqlParameter("TncHeader", aprs.TncHeader)
                    }
                })
                {
                    await command.ExecuteNonQueryAsync();
                }

                if (_includeDebug)
                    Console.WriteLine("Insert statement executed.");
            }
        }

        public async Task PersistAprs(AprsModel aprs)
        {
            await using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                if (_includeDebug)
                {
                    Console.WriteLine("Opening Connection...");
                }

                await connection.OpenAsync();

                if (_includeDebug)
                {
                    Console.WriteLine("Connection opened.");
                    Console.WriteLine("Beginning insert statement...");
                }

                await using (NpgsqlCommand command = new NpgsqlCommand("CALL sp_persist_aprs(@TncHeader, @AprsHeader, @Location, @Text, @WeatherInformation, @Altitude);", connection)
                {
                    Parameters =
                    {
                        new NpgsqlParameter("TncHeader", aprs.TncHeader),
                        new NpgsqlParameter("AprsHeader", aprs.AprsHeader),
                        new NpgsqlParameter("Location", $"Point({LatLongParser.ConvertLongitude(aprs.Long)} {LatLongParser.ConvertLatitude(aprs.Lat)})"),
                        //new NpgsqlParameter("From", aprs.From ),
                        //new NpgsqlParameter("To", aprs.To ),
                        new NpgsqlParameter("Text", aprs.Text),
                        new NpgsqlParameter("WeatherInformation", aprs.Extra1 ?? ""),
                        new NpgsqlParameter("Altitude", "400 ft")
                        //new NpgsqlParameter("Lat", aprs.Lat),
                        //new NpgsqlParameter("Long", aprs.Long)
                    }
                })
                {
                    await command.ExecuteNonQueryAsync();
                }

                if (_includeDebug)
                    Console.WriteLine("Insert statement executed.");
            }
        }

    }
}
