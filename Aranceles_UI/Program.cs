using Aranceles_UI.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient("categoryApi", c => {
    c.BaseAddress = new Uri("http://localhost:5093");
});
builder.Services.AddHttpClient("establishmentApi", e => {
    e.BaseAddress = new Uri("http://localhost:5284");
});
builder.Services.AddHttpClient("personInChargeApi", p => {
    p.BaseAddress = new Uri("http://localhost:5171");
});
builder.Services.AddHttpClient("userApi", u => {
    u.BaseAddress = new Uri("http://localhost:5249");
}).ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

builder.Services.AddScoped<IdProtector>();



var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();