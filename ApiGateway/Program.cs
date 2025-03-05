using System.Text;
using Core.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SIGNING_KEY"]))
        };
    });

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.Development.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
}
else
{
    builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
}

builder.Services.AddCors(options =>
    {
        options.AddPolicy("MyPolicy", builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.Use(async (context, next) =>
    {
        if (context.Request.Cookies.TryGetValue("AccessToken", out var token))
        {
            context.Request.Headers.Add("Authorization", $"Bearer {token}");
            Console.WriteLine($"Token added to headers {token}");
        }

        await next.Invoke();
    });

app.UseCors("MyPolicy");

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();

await app.UseOcelot();
app.Run();
