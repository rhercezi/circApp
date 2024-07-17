using Core.Configs;
using Core.MessageHandling;
using Microsoft.EntityFrameworkCore;
using User.Common.PasswordService;
using User.Common.Events;
using User.Query.Application.Dispatchers;
using User.Query.Application.EventConsuming;
using User.Query.Application.Handlers;
using User.Query.Domain.Repositories;
using User.Query.Domain.DatabaseContext;
using Core.DTOs;
using Core.Events;
using Core.Utilities;
using User.Query.Api.Config;
using User.Query.Application.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<KafkaConsumerConfig>(builder.Configuration.GetSection(nameof(KafkaConsumerConfig)));
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));
builder.Services.Configure<CookieConfig>(builder.Configuration.GetSection(nameof(CookieConfig)));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextFactory<UserDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("UserDbConnectionString"))
);

builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<PasswordHashService>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RefreshTokenRepository>();

builder.Services.AddScoped<IMessageHandler<GetUserByEmailQuery>, GetUserByEmailQueryHandler>();
builder.Services.AddScoped<IMessageHandler<GetUserByIdQuery>, GetUserByIdQueryHandler>();
builder.Services.AddScoped<IMessageHandler<GetUserByUsernameQuery>, GetUserByUsernameQueryHandler>();
builder.Services.AddScoped<IMessageHandler<LoginQuery>, LoginHandler>();
builder.Services.AddScoped<IMessageHandler<RefreshTokenQuery>, RefershTokenHandler>();
builder.Services.AddScoped<IMessageHandler<EmailVerifiedEvent>, EmailVerifiedEventHandler>();
builder.Services.AddScoped<IMessageHandler<PasswordUpdatedEvent>, PasswordUpdatedEventHandler>();
builder.Services.AddScoped<IMessageHandler<UserCreatedEvent>, UserCreatedEventHandler>();
builder.Services.AddScoped<IMessageHandler<UserDeletedEvent>, UserDeletedEventHandler>();
builder.Services.AddScoped<IMessageHandler<UserEditedEvent>, UserUpdatedEventHandler>();

builder.Services.AddScoped<IMessageDispatcher, QueryDispatcher>();
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddHostedService<EventHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<UserDbContext>();
    context.Database.Migrate();
}
catch (Exception e)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(e.StackTrace, $"Migration error: {e.Message}");
}

app.Run();
