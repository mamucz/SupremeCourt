using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using SupremeCourt.Application;
using SupremeCourt.Infrastructure;
using SupremeCourt.Presentation;
using SupremeCourt.Presentation.Middleware;
using System.Text;

using SupremeCourt.Infrastructure.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain;

// ✅ Bootstrap logger pro logování během startu
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
// ✅ Serilog z konfigurace
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// 🔐 JWT + připojení na DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        // ✅ Toto umožní čtení access_token z query stringu (pro SignalR!)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // Tady zadej všechny své SignalR endpointy
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/waitingRoomHub") ||
                     path.StartsWithSegments("/waitingRoomListHub") ||
                     path.StartsWithSegments("/hubLoggerFilter") ||
                     path.StartsWithSegments("/gameHub")))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

// 🧾 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SupremeCourt API", Version = "v1" });
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SupremeCourt.Presentation.xml"));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Vložte JWT token s 'Bearer ' předponou",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 🧱 Služby
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(connectionString);
builder.Services.AddPresentationServices();
//builder.Services.AddHostedService<WaitingRoomMonitor>();
//AI hráči
builder.Services.AddScoped<IAIPlayerRegistrar, AiPlayerRegistrar>();
// 🌐 CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "http://frontend:80")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// ✅ Po sestavení aplikačních služeb zaregistruj callbacky
var sessionManager = app.Services.GetRequiredService<IWaitingRoomSessionManager>();
var eventHandler = app.Services.GetRequiredService<IWaitingRoomEventHandler>();
sessionManager.RegisterCallbacks(
    onTick: eventHandler.HandleCountdownTickAsync,
    onExpired: eventHandler.HandleRoomExpiredAsync
);

// 🗃️ Migrace databáze
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    db.Database.Migrate();
    var registrar = scope.ServiceProvider.GetRequiredService<IAIPlayerRegistrar>();
    await registrar.RegisterAllAiPlayersAsync();
}

// 📈 Serilog request logging s filtrem pro /health
app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (httpContext.Request.Path.StartsWithSegments("/api/health") || httpContext.Request.Path.StartsWithSegments("/health"))
            return LogEventLevel.Debug;

        return LogEventLevel.Information;
    };
});

// 🌍 Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors();
app.UseMiddleware<BlacklistTokenMiddleware>();
app.UseAuthentication();
app.UseAuthorization();


// 📡 Endpoints
app.MapControllers();
app.MapHub<GameHub>("/gameHub").RequireAuthorization();
app.MapHub<WaitingRoomListHub>("/waitingRoomListHub").RequireAuthorization();
app.MapHub<WaitingRoomHub>("/waitingRoomHub").RequireAuthorization();
app.MapGet("/health", () => "OK");

app.Run();
