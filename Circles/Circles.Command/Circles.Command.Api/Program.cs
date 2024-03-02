

using Circles.Command.Application.Dispatcher;
using Circles.Command.Application.EventConsumer;
using Circles.Command.Application.EventProducer;
using Circles.Command.Application.Handlers;
using Circles.Domain.Config;
using Circles.Domain.Repositories;
using Core.Configs;
using Core.Events;
using Core.MessageHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<KafkaConsumerConfig>(builder.Configuration.GetSection(nameof(KafkaConsumerConfig)));
builder.Services.Configure<MongoDbCirclesConfig>(builder.Configuration.GetSection(nameof(MongoDbCirclesConfig)));
builder.Services.Configure<MongoDbCircleUserMapConfig>(builder.Configuration.GetSection(nameof(MongoDbCircleUserMapConfig)));
builder.Services.Configure<MongoDbRequestsConfig>(builder.Configuration.GetSection(nameof(MongoDbRequestsConfig)));
builder.Services.Configure<MongoDbUsersConfig>(builder.Configuration.GetSection(nameof(MongoDbUsersConfig)));
builder.Services.Configure<KafkaProducerConfig>(builder.Configuration.GetSection(nameof(KafkaProducerConfig)));

builder.Services.AddScoped<CirclesRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserCircleRepository>();
builder.Services.AddScoped<JoinRequestRepository>();
builder.Services.AddScoped<JoinCircleEventProducer>();

builder.Services.AddScoped<AddUsersCommandHandler>();
builder.Services.AddScoped<ConfirmJoinCommandHandler>();
builder.Services.AddScoped<CreateCircleCommandHandler>();
builder.Services.AddScoped<DeleteCircleCommandHabndler>();
builder.Services.AddScoped<RemoveUserCommandHandler>();
builder.Services.AddScoped<UpdateCircleCommandHandler>();
builder.Services.AddScoped<UserCreatedEventHandler>();
builder.Services.AddScoped<UserUpdatedEventHandler>();
builder.Services.AddScoped<UserDeletedEventHandler>();

builder.Services.AddSingleton<IHandlerService, HandlerService>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddHostedService<EventHostedService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
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
