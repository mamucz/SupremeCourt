using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SupremeCourt.Presentation;

namespace SupremeCourt.Infrastructure.Tests2
{
    public class TestApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Pokud potřebujeme mockovat služby, můžeme zde
            });

            builder.UseEnvironment("Development"); // ✅ Ujistí se, že běží ve správném prostředí
        }
    }
}