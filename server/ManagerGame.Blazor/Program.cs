using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using ManagerGame.Core;
using ManagerGame.Domain;
using ManagerGame.Blazor.Hubs;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add repository factory
builder.Services.AddScoped<ManagerGame.Blazor.RepositoryFactory>();

// Add repositories using factory
builder.Services.AddScoped<IRepository<Manager>>(sp =>
    sp.GetRequiredService<ManagerGame.Blazor.RepositoryFactory>().CreateRepository<Manager>());
builder.Services.AddScoped<IRepository<Team>>(sp =>
    sp.GetRequiredService<ManagerGame.Blazor.RepositoryFactory>().CreateRepository<Team>());
builder.Services.AddScoped<IRepository<Player>>(sp =>
    sp.GetRequiredService<ManagerGame.Blazor.RepositoryFactory>().CreateRepository<Player>());
builder.Services.AddScoped<IRepository<League>>(sp =>
    sp.GetRequiredService<ManagerGame.Blazor.RepositoryFactory>().CreateRepository<League>());
builder.Services.AddScoped<IRepository<Draft>>(sp =>
    sp.GetRequiredService<ManagerGame.Blazor.RepositoryFactory>().CreateRepository<Draft>());
builder.Services.AddScoped<IRepository<MatchResult>>(sp =>
    sp.GetRequiredService<ManagerGame.Blazor.RepositoryFactory>().CreateRepository<MatchResult>());
builder.Services.AddScoped<IRepository<MatchEvent>>(sp =>
    sp.GetRequiredService<ManagerGame.Blazor.RepositoryFactory>().CreateRepository<MatchEvent>());
builder.Services.AddScoped<IRepository<StartingEleven>>(sp =>
    sp.GetRequiredService<ManagerGame.Blazor.RepositoryFactory>().CreateRepository<StartingEleven>());
builder.Services.AddScoped<IRepository<LeagueSettings>>(sp =>
    sp.GetRequiredService<ManagerGame.Blazor.RepositoryFactory>().CreateRepository<LeagueSettings>());

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Configure SignalR endpoints
app.MapHub<DraftHub>("/drafthub");

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
