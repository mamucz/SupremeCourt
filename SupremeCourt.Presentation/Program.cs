using SupremeCourt.Application;
using SupremeCourt.Infrastructure;
using SupremeCourt.Presentation;
using SupremeCourt.Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Naètení connection stringu
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registrace služeb z jednotlivých vrstev
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(connectionString);
builder.Services.AddPresentationServices();

// Pøidání Swaggeru
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();

// Mapování controllerù a SignalR hubu
app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run();