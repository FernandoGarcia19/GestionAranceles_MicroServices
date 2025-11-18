using Establishment.App.Service;
using Establishment.Dom.Interface;
using Establishment.Inf.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<Establishment.Inf.Persistence.MySqlConnectionDB>();
builder.Services.AddScoped<IRepository, EstablishmentRepository>();
builder.Services.AddScoped<EstablishmentService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();