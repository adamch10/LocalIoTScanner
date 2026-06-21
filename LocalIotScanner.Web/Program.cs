using LocalIotScanner.Core.Interfaces;
using LocalIotScanner.Core.Models;
using LocalIotScanner.Core.Services;
using LocalIotScanner.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<ScannerDbContext>(options =>
    options.UseSqlite("Data Source=iot_scanner.db"));

builder.Services.AddTransient<IHostDiscovery, HostDiscovery>();
builder.Services.AddTransient<IPortScanner, PortScanner>();
builder.Services.AddTransient<IVulnerabilityChecker, VulnerabilityChecker>();
builder.Services.AddScoped<LocalIotScanner.Web.Services.TranslationService>();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IAuditOrchestratorService, AuditOrchestratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ScannerDbContext>();
    dbContext.Database.Migrate();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
