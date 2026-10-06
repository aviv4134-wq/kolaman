using AlertApi;
using DotNetEnv;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);




Env.TraversePath().Load();

string MongoUri = Environment.GetEnvironmentVariable("MongoUri") ?? "mongodb://localhost:27017";

builder.Services.AddSingleton(new MongoClient(MongoUri));

builder.Services.AddScoped<IMongoDatabase>(s =>
{
    var mongoClient = s.GetRequiredService<MongoClient>();
    return mongoClient.GetDatabase("kolAman");
});



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositoryAlert, RepositoryAlert>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
