using Core.Configs;
using Core.MessageHandling;
using User.Command.Application.Dispatcher;
using User.Command.Application.Repositories;
using User.Command.Domain.Events;
using User.Command.Domin.Stores;
using User.Common.PasswordService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<KafkaProducerConfig>(builder.Configuration.GetSection(nameof(KafkaProducerConfig)));
builder.Services.AddScoped<EventStoreRepository>();
builder.Services.AddScoped<EventProducer>();
builder.Services.AddScoped<EventStore>();
builder.Services.AddScoped<PasswordHashService>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddSingleton<IHandlerService, HandlerService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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
