using Aranceles_UI.Security;
using Aranceles_UI.Services.Implementations;
using Aranceles_UI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// HttpClient to Auth microservice (A)
builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri("https://auth-service-url"); // move to config
});

// For accessing HttpContext in services if needed later
builder.Services.AddHttpContextAccessor();
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
builder.Services.AddHttpClient("paymentApi", p => {
    p.BaseAddress = new Uri("http://localhost:5092");
});
builder.Services.AddHttpClient("reportApi", r => {
    r.BaseAddress = new Uri("http://localhost:5007");
});
builder.Services.AddHttpClient("userApi", u => {
    u.BaseAddress = new Uri("http://localhost:5249");
}).ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

builder.Services.AddScoped<IdProtector>();

// Register Facade Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEstablishmentService, EstablishmentService>();
builder.Services.AddScoped<IPersonInChargeService, PersonInChargeService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReportService, ReportService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();