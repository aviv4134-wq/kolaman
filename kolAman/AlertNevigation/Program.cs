
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using NotificationGate.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace AlertNevigation
{

    class Program
    {
        static async Task Main()
        {

            IConfiguration builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.AlertNevigation.json", false, true)
                .Build();



            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder)
            .CreateLogger();

            Log.Information("alert navigation serveis started");

            Env.TraversePath().Load();

            string MongoUri = Environment.GetEnvironmentVariable("MongoUri") ?? "mongodb://localhost:27017";
            string rabbitUri = Environment.GetEnvironmentVariable("Environment.GetEnvironmentVariable") ?? "amqp://guest:guest@localhost:5672" ;
            
            var services = new ServiceCollection();

            services.AddSingleton(new MongoClient(MongoUri));

            services.AddScoped<IMongoDatabase>(s =>
            {
               var mongoClient  = s.GetRequiredService<MongoClient>();
               return mongoClient.GetDatabase("kolAman");
            });

            ConnectionFactory factory = new ConnectionFactory { Uri = new Uri(rabbitUri) };
            var rabbitConnection = await factory.CreateConnectionAsync();
            var channel = await rabbitConnection.CreateChannelAsync();
            
            services.AddSingleton<IChannel>(channel);
            
            var serviesProvider = services.BuildServiceProvider();


            using (var scope = serviesProvider.CreateScope())
            {
                var mongoDb = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
                await mongoDb.CreateCollectionAsync("Alerts");
                Log.Information("craete mongo database and collection alerts ");
            }


            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var alert = JsonSerializer.Deserialize<Alert>(message);
                    if (alert != null)
                    {

                        using (var scope = serviesProvider.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
                            var collection = db.GetCollection<Alert>("Alerts");
                            await collection.InsertOneAsync(alert);
                        }
                    }
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    await channel.BasicAckAsync(ea.DeliveryTag, false);

                }

            };
            
            await channel.BasicConsumeAsync("classified_alerts", false, consumer);
            await Task.Delay(-1);
            
        }

    }
}