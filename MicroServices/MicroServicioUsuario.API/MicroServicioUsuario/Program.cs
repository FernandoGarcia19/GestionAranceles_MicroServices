using MicroServicioUser.App.Services;
using MicroServicioUser.Dom.Interfaces;
using MicroServicoUser.Inf.Persistence;
using MicroServicoUser.Inf.Repository;
using MicroServicioUser.App.Services;
using MicroServicioUser.Dom.Entities;
using MicroServicoUser.Inf.EmailAdapters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Mysql
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<MicroServicoUser.Inf.Persistence.Database.MySqlConnectionManager>();

//Inyeccion de capas
builder.Services.AddScoped<IRepository, UserRepository>();
builder.Services.AddScoped<IRepositoryService<User>, UserService>();

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

builder.Services.AddScoped<IEmailService, SmtpEmailAdapter>(sp => adapter);
builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<RegistrationService>();

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
