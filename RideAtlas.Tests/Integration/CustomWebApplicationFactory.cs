using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RideAtlas.Infrastructure.Persistence;

namespace RideAtlas.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // EF Core 8+ composes AddDbContext calls via IDbContextOptionsConfiguration<T>;
            // removing only DbContextOptions<T> leaves the SqlServer configuration delegate
            // registered alongside InMemory's, which EF rejects as two providers.
            services.RemoveAll<DbContextOptions<RideAtlasDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<RideAtlasDbContext>>();
            services.AddDbContext<RideAtlasDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
