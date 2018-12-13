using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using Serilog.Events;

namespace HousePrice.Api.ImportFileWatcher
{
    static class Program
    {
        private static readonly ManualResetEvent ResetEvent = new ManualResetEvent(false);

        static FileListener GetPoller(string watchDirectory, string processingDirectory, string successDirectory,
            RestClient client)
        {
            return new FileListener(watchDirectory, async (f) =>
            {
                Log.Information($"Processing {f.FullName}");
                var filename = $"{f.Name}-{Guid.NewGuid().ToString()}";
                var processingFile = Path.Combine(processingDirectory, filename);

                File.Move(f.FullName, processingFile);

                using (var stream = new FileStream(processingFile, FileMode.Open, FileAccess.Read))
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        using (var csvReader = new CsvReader(streamReader))
                        {
                            csvReader.Configuration.HasHeaderRecord = false;
                            csvReader.Configuration.RegisterClassMap<HousePriceMap>();
                            while (await csvReader.ReadAsync())
                            {
                                var data = csvReader.GetRecord<HousePrice>();
                                Log.Debug(JsonConvert.SerializeObject(data));

                                await AddRecord(f, data, client);
                            }
                        }
                    }
                }

                var destFilename = Path.Combine(successDirectory, filename);

                File.Move(processingFile, destFilename);
            });
        }

        static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) => { configApp.AddCommandLine(args); })
                .ConfigureServices((hostContext, services) =>
                {
                    // Add Coravel's Scheduling
                    services.AddScheduler();
                })
                .Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();


            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var apiEndpoint = configuration["endPoint"];

            var client = new RestClient(apiEndpoint);

            var watchDirectory = configuration["watchDirectory"];
            var successDirectory = configuration["successDirectory"];
            var processingDirectory = configuration["processingDirectory"];
            var poller = GetPoller(watchDirectory, processingDirectory, successDirectory, client);

            // Configure the scheduled tasks....
            host.Services.UseScheduler(scheduler =>
                    {
                        scheduler.Schedule(
                                () =>
                                {
                                    Console.WriteLine("Checking drop folder...");
                                    poller.CheckModifications();
                                })
                            .EveryMinute()
                            .PreventOverlapping("CheckDropFolder");
                    }
                )
                .LogScheduledTaskProgress(host.Services.GetService<ILogger<IScheduler>>());

            host.Run();
        }

        private static async Task AddRecord(FileInfo f, HousePrice record, RestClient client)
        {
            var req = new RestRequest($"api/transaction");

            req.AddParameter("application/json", JsonConvert.SerializeObject(record), ParameterType.RequestBody);
            req.RequestFormat = DataFormat.Json;
            Log.Information("Calling transaction import");
            var result = await client.ExecutePostTaskAsync(req);
            if (!result.IsSuccessful)
            {
                Log.Error(
                    $"post failed, status: {result.StatusCode}, message:{result.ErrorMessage}, content: {result.Content}");
            }
        }
    }
}