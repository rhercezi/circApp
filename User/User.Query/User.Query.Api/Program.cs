using Core.Configs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.EntityFrameworkCore;
using User.Common.PasswordService;
using User.Query.Application.Dispatchers;
using User.Query.Application.DTOs;
using User.Query.Application.EventConsuming;
using User.Query.Domain.Repositories;
using User.Query.Domain.DatabaseContext;
using User.Query.Application.Utils.Configs;
using User.Query.Application.Utils.Services;
using User.Common.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<KafkaConsumerConfig>(builder.Configuration.GetSection(nameof(KafkaConsumerConfig)));
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));
builder.Services.Configure<MailConfig>(builder.Configuration.GetSection(nameof(MailConfig)));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextFactory<UserDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("UserDbConnectionString"))
);
builder.Services.AddSingleton<EmailSenderService>();
builder.Services.AddScoped<PasswordHashService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddSingleton<IHandlerService, HandlerService>();
builder.Services.AddScoped<IQueryDispatcher<TokenDto>, AuthDispatcher>();
builder.Services.AddScoped<IQueryDispatcher<UserDto>, QueryDispatcher>();
builder.Services.AddScoped<EventConsumer>();
builder.Services.AddSingleton<TypeResolutionService>();
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

app.Run();
