using System.Text;
using MicroServiceCategory.Infrastructure.Messaging;
using Payment.App.Service;
using Payment.Dom.Interface;
using Payment.Inf.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Payment.API.Messaging;
using Payment.Inf.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<Payment.Inf.Persistence.MySqlConnectionDB>();
builder.Services.AddScoped<IRepository, PaymentRepository>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

            ClockSkew = TimeSpan.Zero 
        };
    });

builder.Services.AddSingleton<IEventPublisher, RabbitPaymentPublisher>();
builder.Services.AddHostedService<RabbitPaymentConsumer>();
builder.Services.AddSingleton<RabbitPaymentInitialPublisher>();

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
