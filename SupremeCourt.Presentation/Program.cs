using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SupremeCourt.Application;
using SupremeCourt.Infrastructure;
using SupremeCourt.Presentation;
using SupremeCourt.Presentation.Hubs;
using SupremeCourt.Presentation.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Na�ten� connection stringu
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Na�ten� konfigurace JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Registrace autentizace a autorizace pomoc� JWT
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

builder.Services.AddAuthorization();

// Registrace Swaggeru
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SupremeCourt API", Version = "v1" });

    // P�id�n� mo�nosti zadat JWT token do Swaggeru
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Vlo�te JWT token s 'Bearer ' p�edponou",
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
            new string[] { }
        }
    });
});

// Registrace slu�eb z jednotliv�ch vrstev
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(connectionString);
builder.Services.AddPresentationServices();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<BlacklistTokenMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run();
