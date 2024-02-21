using Core.Configs;
using Core.MessageHandling;
using Microsoft.EntityFrameworkCore;
using User.Common.PasswordService;
using User.Query.Application.Dispatchers;
using User.Query.Application.EventConsuming;
using User.Query.Domain.Repositories;
using User.Query.Domain.DatabaseContext;
using Core.DTOs;
using User.Query.Application.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<KafkaConsumerConfig>(builder.Configuration.GetSection(nameof(KafkaConsumerConfig)));
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextFactory<UserDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("UserDbConnectionString"))
);
builder.Services.AddScoped<PasswordHashService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<GetUserByEmailQueryHandler>();
builder.Services.AddScoped<GetUserByIdQueryHandler>();
builder.Services.AddScoped<GetUserByUsernameQueryHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<EmailVerifiedEventHandler>();
builder.Services.AddScoped<PasswordUpdatedEventHandler>();
builder.Services.AddScoped<UserCreatedEventHandler>();
builder.Services.AddScoped<UserDeletedEventHandler>();
builder.Services.AddScoped<UserUpdatedEventHandler>();
builder.Services.AddSingleton<IHandlerService, HandlerService>();
builder.Services.AddScoped<IQueryDispatcher<UserDto>, AuthDispatcher>();
builder.Services.AddScoped<IQueryDispatcher<UserDto>, QueryDispatcher>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<EventConsumer>();
builder.Services.AddHostedService<EventHostedService>();

builder.Services.AddCors(options =>
    {
        options.AddPolicy("MyPolicy", builder =>
        {
            builder.WithOrigins("http://127.0.0.1:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyPolicy");

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
