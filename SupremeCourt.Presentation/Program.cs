using SupremeCourt.Application;
using SupremeCourt.Infrastructure;
using SupremeCourt.Presentation;
using SupremeCourt.Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Na�ten� connection stringu
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registrace slu�eb z jednotliv�ch vrstev
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(connectionString);
builder.Services.AddPresentationServices();

// P�id�n� Swaggeru
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();

// Mapov�n� controller� a SignalR hubu
app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run();