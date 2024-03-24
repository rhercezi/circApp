using Core.Configs;
using Core.MessageHandling;
using Core.Repositories;
using Core.Utilities;
using User.Command.Application.Dispatcher;
using User.Command.Application.Handlers.CommandHandlers;
using User.Command.Application.Repositories;
using User.Command.Domain.Config;
using User.Command.Domain.Events;
using User.Command.Domain.Repositories;
using User.Command.Domin.Stores;
using User.Common.DAOs;
using User.Common.PasswordService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<KafkaProducerConfig>(builder.Configuration.GetSection(nameof(KafkaProducerConfig)));
builder.Services.Configure<MailConfig>(builder.Configuration.GetSection(nameof(MailConfig)));
builder.Services.Configure<MongoDbConfigIDLink>(builder.Configuration.GetSection(nameof(MongoDbConfigIDLink)));
builder.Services.AddSingleton<EmailSenderService>();
builder.Services.AddScoped<IdLinkRepository>();
builder.Services.AddScoped<EventStoreRepository>();
builder.Services.AddScoped<EventProducer>();
builder.Services.AddScoped<EventStore>();
builder.Services.AddScoped<PasswordHashService>();
builder.Services.AddSingleton<IHandlerService, HandlerService>();
builder.Services.AddScoped<CreateUserCommandHandler>();
builder.Services.AddScoped<DeleteUserCommandHandler>();
builder.Services.AddScoped<EditUserCommandHandler>();
builder.Services.AddScoped<ResetPasswordCommandHandler>();
builder.Services.AddScoped<UpdatePasswordCommandHandler>();
builder.Services.AddScoped<VerifyEmailCommandHandler>();
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
