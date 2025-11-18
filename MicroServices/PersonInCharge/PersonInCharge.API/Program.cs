using PersonInCharge.App.Service;
using PersonInCharge.Dom.Interface;
using PersonInCharge.Inf.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<PersonInCharge.Inf.Persistence.MySqlConnectionDB>();
builder.Services.AddScoped<IRepository, PersonInChargeRepository>();
builder.Services.AddScoped<PersonInChargeService>();
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
