using MicroServicioUser.App.Services;
using MicroServicioUser.Dom.Interfaces;
using MicroServicoUser.Inf.Repository;
using MicroServicoUser.Inf.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Mysql
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//Inyeccion de capas
builder.Services.AddScoped<IRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MicroServicoUser.Inf.Persistence.MySqlConnectionDB>();

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
