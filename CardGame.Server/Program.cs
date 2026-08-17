using CardGame.Infrastructure.Database;
using CardGame.Infrastructure.Services;
using CardGame.Server.Middleware;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add SQLite DbContext 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=cardgame.db";

builder.Services.AddDbContext<CardGameDbContext>(options =>
    options.UseSqlite(connectionString));

string swaggerBasePath = "api";


// Add services to the container.
builder.Services.AddTransient<IDeckService, DeckService>();
builder.Services.AddTransient<IScoringEngine, ScoringEngineService>();
builder.Services.AddScoped<IGameService, GameService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use Problem Details middleware for better error handling
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        // Ensure the database is created and apply any pending migrations
        var dbContext = scope.ServiceProvider.GetRequiredService<CardGameDbContext>();
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
    }

    app.MapOpenApi();
    app.UseSwagger(config =>config.RouteTemplate = swaggerBasePath+"/swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint($"/{swaggerBasePath}/swagger/v1/swagger.json", $"Card Game API");
        option.RoutePrefix = $"{swaggerBasePath}/swagger";
    }); 

}

app.UseHttpsRedirection();

app.UseAuthorization();

//Use a global error handling middleware to catch unhandled exceptions and return a standardized error response
app.UseMiddleware<GlobalErrorHandler>();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
