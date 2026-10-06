
using AlertMangment.Serveis;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NotificationGate.Models;
using Serilog;

namespace AlertNevigation
{

    class Program
    {
        static async Task Main()
        {

            IConfiguration builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.AlertMangment.json", false, true)
                .Build();



            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder)
            .CreateLogger();

            Log.Information("alert managment serveis started");

            Env.TraversePath().Load();

            string MongoUri = Environment.GetEnvironmentVariable("MongoUri") ?? "mongodb://localhost:27017";

            var services = new ServiceCollection();

            services.AddSingleton(new MongoClient(MongoUri));

            services.AddScoped<IMongoDatabase>(s =>
            {
                var mongoClient = s.GetRequiredService<MongoClient>();
                return mongoClient.GetDatabase("kolAman");
            });

            services.AddSingleton<ManageAlert>();

            var serviesProvider = services.BuildServiceProvider();


            while (true)
            {
                
                using (var scope = serviesProvider.CreateScope())
                {
                    var manegmentAlertServeis = scope.ServiceProvider.GetRequiredService<ManageAlert>();
                    
                    var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
                    var collection = db.GetCollection<Alert>("Alerts");
                    var alerts = collection.AsQueryable();
                    
                    var newAlerts = await alerts.Where(a => a.Status == "WAITING").ToListAsync();
                    
                    foreach (var alert in newAlerts)
                    {
                       
                       if ( manegmentAlertServeis.is_importent_alert(alert) == true)
                        {
                            DateTime startProcessing = DateTime.UtcNow;
                            alert.Status = "INPROGRESS";
                            
                            await Task.Delay(10);
                            
                            alert.Status = "DONE";
                            var endProcessingTime = DateTime.UtcNow - startProcessing;
                            
                            Log.Information($"proceess time {endProcessingTime.Seconds}");
                        }
                        else
                        {
                            alert.Status = "CANCEL";
                        }

                        var filter = Builders<Alert>.Filter.Eq("_id", alert.Alert_id);

                        var update = Builders <Alert>.Update
                            .Set("Status", alert.Status);

                        var result = collection.UpdateOne(filter, update);



                    }


                }
                await Task.Delay(60);
            }


        }

    }
}