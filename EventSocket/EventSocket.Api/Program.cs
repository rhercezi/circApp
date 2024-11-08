using Core.Configs;
using Core.DTOs;
using Core.Events;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Utilities;
using EventSocket.Application.Commands;
using EventSocket.Application.Config;
using EventSocket.Application.Dispatcher;
using EventSocket.Application.DTOs;
using EventSocket.Application.Handlers;
using EventSocket.Application.Services;
using EventSocket.Domain.Config;
using EventSocket.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KafkaConsumerConfig>(builder.Configuration.GetSection(nameof(KafkaConsumerConfig)));
builder.Services.Configure<CirclesServiceConfig>(builder.Configuration.GetSection(nameof(CirclesServiceConfig)));
builder.Services.Configure<RemindersServiceConfig>(builder.Configuration.GetSection(nameof(RemindersServiceConfig)));
builder.Services.Configure<MongoDbNotificationConfig>(builder.Configuration.GetSection(nameof(MongoDbNotificationConfig)));

builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<InternalHttpClient<CircleDto>>();
builder.Services.AddScoped<InternalHttpClient<List<ReminderDto>>>();

builder.Services.AddScoped<IMessageHandler<AppointmentChangePublicEvent>, AppointmentChangeEventHandler>();
builder.Services.AddScoped<IMessageHandler<JoinCircleRequestPublicEvent>, JoinRequestEventHandler>();
builder.Services.AddScoped<IMessageHandler<TaskChangePublicEvent>, TaskChangeEventHandler>();
builder.Services.AddScoped<IMessageHandler<DeleteNotificationCommand>, DeleteNotificationCommandHandler>();
builder.Services.AddScoped<IMessageHandler<SendNotificationsCommand>, SendNotificationsCommandHandler>();
builder.Services.AddScoped<IMessageHandler<SendReminderCommand>, SendReminderCommandHandler>();

builder.Services.AddScoped<IMessageDispatcher, EventDispatcher>();
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddSingleton<IConnectedUsersService, ConnectedUsersService>();
builder.Services.AddSingleton<SocketConnectionManager>();
builder.Services.AddHostedService<EventHostedService>();
builder.Services.AddHostedService<ReminderHostedService>();

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

app.UseWebSockets();
app.MapControllers();

app.Run();
