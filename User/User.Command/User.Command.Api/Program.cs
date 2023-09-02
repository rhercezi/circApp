using Core.Configs;
using Core.MessageHandling;
using Core.Repositories;
using User.Command.Application.Dispatcher;
using User.Command.Application.Handlers;
using User.Command.Application.Repositories;
using User.Command.Domain.Repositories;
using User.Command.Domin.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.AddScoped<IEventStoreRepository,EventStoreRepository>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddSingleton<IHandlerService, UserHandlerService>();
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
