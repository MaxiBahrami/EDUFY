using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.InfrastructureLayer.Data;
using InsightAcademy.InfrastructureLayer.Data.Seed;
using InsightAcademy.InfrastructureLayer.Implementation;
using InsightAcademy.InfrastructureLayer.IRepository;
using InsightAcademy.InfrastructureLayer.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InsightAcademy.InfrastructureLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<AppDbContext>(
                                              option => option.UseSqlServer(connectionString,
                                              b =>
                                              {
                                                  b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                                                  b.CommandTimeout(180);
                                              })
                                             .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking), ServiceLifetime.Transient);

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                                                                            .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            services.AddSingleton<IHostedService, Seeder>();
            services.AddScoped<ITutor, TutorRepository>();
            services.AddScoped<IUser, UserRepository>();
            services.AddScoped<IWishList, WishListRepository>();

            services.AddScoped<ICategory, CategoryRepository>();
            services.AddScoped<ISubject, SubjectRepository>();
            //builder.Services.AddScoped<IMessage, ChatRepository>();
            services.AddScoped<ILike, LikeRepository>();
            services.AddScoped<IReview, ReviewRepository>();
            services.AddScoped<IChat, ChatRepository>();
            services.AddScoped<IStudent, StudentRepository>();
            services.AddScoped<IDashBoard, DashBoardRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IGeo, GeoRepository>();
            services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();

            return services;
        }
    }
}