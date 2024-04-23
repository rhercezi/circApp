using Core.Configs;
using Core.MessageHandling;
using Tasks.Command.Application.Dispatchers;
using Tasks.Command.Application.Events;
using Tasks.Command.Application.Handlers;
using Tasks.Domain.Config;
using Tasks.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbTaskConfig>(builder.Configuration.GetSection(nameof(MongoDbTaskConfig)));
builder.Services.Configure<KafkaProducerConfig>(builder.Configuration.GetSection(nameof(KafkaProducerConfig)));

builder.Services.AddScoped<AppTaskRepository>();
builder.Services.AddScoped<TasksEventProducer>();

builder.Services.AddScoped<CreateTaskCommandHandler>();
builder.Services.AddScoped<DeleteTaskCommandHandler>();
builder.Services.AddScoped<UpdateTaskCommandHandler>();
builder.Services.AddScoped<CompleteTaskCommandHandler>();

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
