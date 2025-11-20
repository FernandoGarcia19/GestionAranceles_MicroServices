var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

builder.Services.AddHttpClient("categoryApi", c => {
    c.BaseAddress = new Uri("http://localhost:<port>");
});
builder.Services.AddHttpClient("establishmentApi", e => {
    e.BaseAddress = new Uri("http://localhost:<port>");
});
builder.Services.AddHttpClient("personInChargeApi", p => {
    p.BaseAddress = new Uri("http://localhost:<port>");
});
builder.Services.AddHttpClient("userApi", u => {
    u.BaseAddress = new Uri("http://localhost:<port>");
});

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