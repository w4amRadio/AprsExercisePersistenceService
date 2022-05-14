using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AprsPersistenceService.Models;
using AprsPersistenceService.Utils;
using Npgsql;
using NpgsqlTypes;

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
            if(aprs == null)
            {
                throw new Exception("APRS model cannot be null in PersistAprs!");
            }

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

                string locationString = string.Empty;

                if (!string.IsNullOrEmpty(aprs.Long) && !string.IsNullOrEmpty(aprs.Lat))
                {
                    locationString = $"Point({LatLongParser.ConvertLongitude(aprs.Long)} {LatLongParser.ConvertLatitude(aprs.Lat)})";
                }

                await using (NpgsqlCommand command = new NpgsqlCommand("CALL sp_persist_aprs(@TncHeader, @AprsHeader, @Location, @Text, @WeatherInformation, @Altitude, @Radio, @Course, @Voltage, @From, @To);", connection)
                {
                    Parameters =
                    {
                        new NpgsqlParameter("TncHeader", aprs.TncHeader),
                        new NpgsqlParameter("AprsHeader", aprs.AprsHeader),
                        new NpgsqlParameter("Location", locationString),
                        new NpgsqlParameter("Text", aprs.Text),
                        new NpgsqlParameter("WeatherInformation", aprs.Extra1 ?? string.Empty),
                        new NpgsqlParameter("Altitude", aprs.Altitude ?? string.Empty),
                        new NpgsqlParameter("Radio", aprs.Radio ?? string.Empty),
                        new NpgsqlParameter("Course", aprs.Course ?? string.Empty),
                        new NpgsqlParameter("Voltage", aprs.GateVoltage ?? string.Empty),
                        //new NpgsqlParameter("Lat", aprs.Lat),
                        //new NpgsqlParameter("Long", aprs.Long)
                        new NpgsqlParameter("From", aprs.From ?? string.Empty),
                        new NpgsqlParameter("To", aprs.To ?? string.Empty)
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
