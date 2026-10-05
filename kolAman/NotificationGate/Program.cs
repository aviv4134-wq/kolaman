





using Confluent.Kafka;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections.ObjectModel;

namespace NotificationGate
{

    class Program
    {

        static void Main()
        {
            IConfiguration builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.notificationGate.json", false, true)
                .Build();

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder)
            .CreateLogger();

            Env.TraversePath().Load();

            Log.Information("starting notofocation gate service");

            string path = Path.Combine("../../../../../","alert-simulator");



            using var watcher = new FileSystemWatcher(path);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Created += OnCreated;

          


            watcher.Filter = "alert.ready";
           
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }


        

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            FindAlertJson(e);
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

                string rawAlert  = File.ReadAllText(fullPathAlertjson);

                await SendAlertToKafka(rawAlert);
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
                string kafkaBootServer = (Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS_KAFKA")!);

                var config = new ProducerConfig
                {
                    BootstrapServers = kafkaBootServer

                };

                var producer = new ProducerBuilder<Null, string>(config).Build();

                await producer.ProduceAsync("rawAlerts", new Message<Null, string> { Key = null, Value = rawAlert });

                producer.Flush(TimeSpan.FromSeconds(3));

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        
    }
}