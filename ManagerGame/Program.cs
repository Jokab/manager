using ManagerGame;
using ManagerGame.Commands;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("user", policy =>
        policy
            .RequireRole("user")
            .RequireClaim("scope", "api"));

builder.Services.AddCors();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<CreateTeamCommandHandler>();
builder.Services.AddTransient<CreateManagerCommandHandler>();
builder.Services.AddTransient<LoginCommandHandler>();


builder.Services.AddNpgsql<ApplicationDbContext>(builder.Configuration.GetConnectionString("Db"));

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapApi();
app.Run();

// Make the implicit Program class public so test projects can access it
namespace ManagerGame
{
    public partial class Program { }
}