using Appointments.Domain.Configs;
using Appointments.Domain.Repositories;
using Appointments.Query.Application.Config;
using Appointments.Query.Application.Dispatchers;
using Appointments.Query.Application.DTOs;
using Appointments.Query.Application.Handlers;
using Core.DTOs;
using Core.MessageHandling;
using Core.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbAppointmentsConfig>(builder.Configuration.GetSection(nameof(MongoDbAppointmentsConfig)));
builder.Services.Configure<MongoDbAppointmentDetailsConfig>(builder.Configuration.GetSection(nameof(MongoDbAppointmentDetailsConfig)));
builder.Services.Configure<MongoDbCAMapConfig>(builder.Configuration.GetSection(nameof(MongoDbCAMapConfig)));
builder.Services.Configure<MongoDbCircleUserMapConfig>(builder.Configuration.GetSection(nameof(MongoDbCircleUserMapConfig)));
builder.Services.Configure<CirclesServiceConfig>(builder.Configuration.GetSection(nameof(CirclesServiceConfig)));

builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<AppointmentDetailsRepository>();
builder.Services.AddScoped<CAMapRepository>();
builder.Services.AddScoped<UserCircleRepository>();
builder.Services.AddScoped<InternalHttpClient<AppUserDto>>();

builder.Services.AddScoped<GetAppointmentsByCircleIdQueryHandler>();
builder.Services.AddScoped<GetAppointmentsByUserIdQueryHandler>();

builder.Services.AddSingleton<IHandlerService, HandlerService>();
builder.Services.AddScoped<IQueryDispatcher<AppointmentsDto>, AppointmentsQueryDispatcher>();

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
