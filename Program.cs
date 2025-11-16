using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PDV.Data;
using PDV.Models.Interfaces;
using PDV.Services;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PDVContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PDVContext") ?? throw new InvalidOperationException("Connection string 'SenacLojasContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IFechamentoService, FechamentoService>();

var supportedCultures = new[] { new CultureInfo("pt-BR") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseDeveloperExceptionPage();

app.UseStaticFiles();

app.UseRequestLocalization();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();