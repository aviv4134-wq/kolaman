using Confluent.Kafka;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

namespace NotificationGate
{

    class Program
    {

        private static IProducer<Null, string> _producer;


        static void Main()
        {
            IConfiguration builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.notificationGate.json", false, true)
                .Build();

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder)
            .CreateLogger();

            Log.Information("starting notofocation gate service");



            Env.TraversePath().Load();

            try
            {


                string kafkaBootServer = (Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS_KAFKA")!);

                var config = new ProducerConfig
                {
                    BootstrapServers = kafkaBootServer

                };

                _producer = new ProducerBuilder<Null, string>(config).Build();

                Log.Information("create kafka producer");



                string path = Path.Combine("../../../../../", "alert-simulator");

                using var watcher = new FileSystemWatcher(path);

                watcher.InternalBufferSize = 65536;

                watcher.NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
                                     | NotifyFilters.Size;

                Log.Information("start filewatcher loop");

                watcher.Created += OnCreated;




                watcher.Filter = "alert.ready";

                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;

                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }
            catch (Exception  ex)
            {
                Log.Error(ex.Message);
                
            }
            finally
            {
                _producer.Flush();
                _producer.Dispose();
            }
        }


        

        private static async void OnCreated(object sender, FileSystemEventArgs e)
        {
            await FindAlertJson(e);
        }


        public static async Task FindAlertJson( FileSystemEventArgs e)
        {
            try
            {

                int fileFinishNameIndex = 3;

                List<string> fileName = e.Name.Split("\\").ToList();
                fileName.RemoveAt(fileFinishNameIndex);
                fileName.Add("alert.json");
                string fileNameAlertJson = string.Join("\\", fileName);
                string fullPathAlertjson = Path.Combine("../../../../../", "alert-simulator", fileNameAlertJson);

                string rawAlertStr  = File.ReadAllText(fullPathAlertjson);

             
                
                await SendAlertToKafka(rawAlertStr);
                return;

                
            }
            catch (FileNotFoundException ex)
            {
                Log.Error("the json file not exisits");
                return;
            }


            


        }

        public static async Task SendAlertToKafka(string rawAlert)
        {
            try
            {
                await _producer.ProduceAsync("rawAlerts", new Message<Null, string> { Key = null, Value = rawAlert });

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return;
            }
        }

        
    }
}