using System.Text;
using MicroServicioUser.App.Services;
using MicroServicioUser.Dom.Interfaces;
using MicroServicoUser.Inf.Persistence;
using MicroServicoUser.Inf.Repository;
using MicroServicioUser.App.Services;
using MicroServicioUser.Dom.Entities;
using MicroServicoUser.Inf.EmailAdapters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Mysql
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<MicroServicoUser.Inf.Persistence.Database.MySqlConnectionManager>();

//Inyeccion de capas
builder.Services.AddScoped<IRepository, UserRepository>();
builder.Services.AddScoped<IRepositoryService<User>, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<ILogin, Login>();
builder.Services.AddScoped<IRegistration, Registration>();

var _configuration = builder.Configuration;
var smtpHost = _configuration["Email:SmtpHost"];
var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
var smtpUser = _configuration["Email:SmtpUser"];
var smtpPass = _configuration["Email:SmtpPassword"];
var fromEmail = _configuration["Email:FromEmail"];
var fromName = _configuration["Email:FromName"] ?? "Sistema de Pagos";
var adapter = new SmtpEmailAdapter(
    new SmtpSettings
    {
        Host = smtpHost,
        Port = smtpPort,
        User = smtpUser,
        Password = smtpPass,
        FromEmail = fromEmail,
        FromName = fromName
    }
);

builder.Services.AddHttpClient("userApi", u => {
    u.BaseAddress = new Uri("http://localhost:5249");
}).ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

builder.Services.AddScoped<IEmailService, SmtpEmailAdapter>(sp => adapter);
builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<RegistrationService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]);

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

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
