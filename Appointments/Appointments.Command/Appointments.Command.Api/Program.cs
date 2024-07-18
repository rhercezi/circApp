using Appointments.Command.Application.Commands;
using Appointments.Command.Application.Dispatchers;
using Appointments.Command.Application.EventProducer;
using Appointments.Command.Application.Handlers;
using Appointments.Domain.Configs;
using Appointments.Domain.Repositories;
using Core.Configs;
using Core.MessageHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbAppointmentsConfig>(builder.Configuration.GetSection(nameof(MongoDbAppointmentsConfig)));
builder.Services.Configure<MongoDbAppointmentDetailsConfig>(builder.Configuration.GetSection(nameof(MongoDbAppointmentDetailsConfig)));
builder.Services.Configure<MongoDbCAMapConfig>(builder.Configuration.GetSection(nameof(MongoDbCAMapConfig)));
builder.Services.Configure<KafkaProducerConfig>(builder.Configuration.GetSection(nameof(KafkaProducerConfig)));

builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<AppointmentDetailsRepository>();
builder.Services.AddScoped<CAMapRepository>();
builder.Services.AddScoped<AppointmentEventProducer>();

builder.Services.AddScoped<IMessageHandler<AddAppointmentDetailsCommand>, AddAppointmentDetailsCommandHandler>();
builder.Services.AddScoped<IMessageHandler<CreateAppointmentCommand>, CreateAppointmentCommandHandler>();
builder.Services.AddScoped<IMessageHandler<DeleteAppointmentCommand>, DeleteAppointmentCommandHandler>();
builder.Services.AddScoped<IMessageHandler<DeleteAppointmentDetailCommand>, DeleteAppointmentDetailCommandHandler>();
builder.Services.AddScoped<IMessageHandler<UpdateAppointmentCommand>, UpdateAppointmentCommandHandler>();
builder.Services.AddScoped<IMessageHandler<UpdateAppointmentDetailCommand>, UpdateAppointmentDetailCommandHandler>();

builder.Services.AddScoped<IMessageDispatcher, CommandDispatcher>();

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
