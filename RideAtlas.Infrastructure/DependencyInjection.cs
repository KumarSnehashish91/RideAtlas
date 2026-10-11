using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Infrastructure.AiPlanning;
using RideAtlas.Infrastructure.Auth;
using RideAtlas.Infrastructure.Persistence;
using RideAtlas.Infrastructure.Persistence.Repositories;
using RideAtlas.Infrastructure.Weather;

namespace RideAtlas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RideAtlasDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IRiderAccountRepository, RiderAccountRepository>();
        services.AddScoped<IBikeRepository, BikeRepository>();
        services.AddScoped<ITripRepository, TripRepository>();
        services.AddScoped<IRideRepository, RideRepository>();
        services.AddScoped<IFuelLogRepository, FuelLogRepository>();
        services.AddScoped<IServiceEntryRepository, ServiceEntryRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IPackingItemRepository, PackingItemRepository>();
        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IReminderRuleRepository, ReminderRuleRepository>();
        services.AddScoped<IExplorerRideRepository, ExplorerRideRepository>();
        services.AddScoped<IRiderProfileRepository, RiderProfileRepository>();
        services.AddScoped<IRidingBuddyRepository, RidingBuddyRepository>();
        services.AddScoped<ICommunityPostRepository, CommunityPostRepository>();
        services.AddScoped<ICommunityCommentRepository, CommunityCommentRepository>();
        services.AddScoped<IPostLikeRepository, PostLikeRepository>();
        services.AddScoped<IRideEventRepository, RideEventRepository>();
        services.AddScoped<IEventInviteeRepository, EventInviteeRepository>();
        services.AddScoped<IAppSettingsRepository, AppSettingsRepository>();

        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IAiPlanningService, TemplateAiPlanningService>();

        services.AddHttpClient<IWeatherService, OpenMeteoWeatherService>(client =>
            client.BaseAddress = new Uri("https://api.open-meteo.com"));

        return services;
    }
}
