using Circles.Domain.Config;
using Circles.Domain.Repositories;
using Circles.Query.Application.Dispatchers;
using Circles.Query.Application.Handlers;
using Core.DTOs;
using Core.MessageHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbCirclesConfig>(builder.Configuration.GetSection(nameof(MongoDbCirclesConfig)));
builder.Services.Configure<MongoDbCircleUserMapConfig>(builder.Configuration.GetSection(nameof(MongoDbCircleUserMapConfig)));
builder.Services.Configure<MongoDbRequestsConfig>(builder.Configuration.GetSection(nameof(MongoDbRequestsConfig)));
builder.Services.Configure<MongoDbUsersConfig>(builder.Configuration.GetSection(nameof(MongoDbUsersConfig)));

builder.Services.AddScoped<CirclesRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserCircleRepository>();
builder.Services.AddScoped<JoinRequestRepository>();

builder.Services.AddScoped<GetCirclesByUserIdQueryHandler>();
builder.Services.AddScoped<GetUsersByCircleIdQueryHandler>();
builder.Services.AddScoped<SearchQueryHandler>();

builder.Services.AddSingleton<IHandlerService, HandlerService>();

builder.Services.AddScoped<IQueryDispatcher<AppUserDto>, UserQueryDispatcher>();
builder.Services.AddScoped<IQueryDispatcher<CircleDto>, CircleQueryDispatcher>();
builder.Services.AddScoped<IQueryDispatcher<AppUsersDto>, UserSearchDispatcher>();

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
