using Core.DTOs;
using Core.MessageHandling;
using Core.Utilities;
using Tasks.Domain.Config;
using Tasks.Domain.Repositories;
using Tasks.Query.Application.Config;
using Tasks.Query.Application.Dispatchers;
using Tasks.Query.Application.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<CirclesServiceConfig>(builder.Configuration.GetSection(nameof(CirclesServiceConfig)));
builder.Services.Configure<MongoDbTaskConfig>(builder.Configuration.GetSection(nameof(MongoDbTaskConfig)));

builder.Services.AddScoped<AppTaskRepository>();
builder.Services.AddScoped<InternalHttpClient<AppUserDto>>();

builder.Services.AddScoped<GetTasksForUserQueryHandler>();
builder.Services.AddScoped<GetTasksForCircleQueryHandler>();

builder.Services.AddSingleton<IHandlerService, HandlerService>();
builder.Services.AddScoped<IQueryDispatcher<TasksDto>, TasksQueryDispatcher>();

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
