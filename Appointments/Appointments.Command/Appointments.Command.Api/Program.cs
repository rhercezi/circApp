using Appointments.Command.Application.Dispatchers;
using Appointments.Command.Application.Handlers;
using Appointments.Domain.Configs;
using Appointments.Domain.Repositories;
using Core.MessageHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbAppointmentsConfig>(builder.Configuration.GetSection(nameof(MongoDbAppointmentsConfig)));
builder.Services.Configure<MongoDbAppointmentDetailsConfig>(builder.Configuration.GetSection(nameof(MongoDbAppointmentDetailsConfig)));
builder.Services.Configure<MongoDbCAMapConfig>(builder.Configuration.GetSection(nameof(MongoDbCAMapConfig)));

builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<AppointmentDetailsRepository>();
builder.Services.AddScoped<CAMapRepository>();

builder.Services.AddScoped<AddAppointmentDetailsCommandHandler>();
builder.Services.AddScoped<CreateAppointmentCommandHandler>();
builder.Services.AddScoped<DeleteAppointmentCommandHandler>();
builder.Services.AddScoped<DeleteAppointmentDetailCommandHandler>();
builder.Services.AddScoped<UpdateAppointmentCommandHandler>();
builder.Services.AddScoped<UpdateAppointmentDetailCommandHandler>();

builder.Services.AddSingleton<IHandlerService, HandlerService>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();

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
