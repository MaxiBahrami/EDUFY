using InsightAcademy.ApplicationLayer.IService;
using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.InfrastructureLayer.Implementation;
using InsightAcademy.UI.Helper;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

using System.Reflection;

namespace InsightAcademy.ApplicationLayerApplication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddRazorPages();
            services.AddSignalR();

            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<ITutorService, TutorServices>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWishListService, WishListService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IDashBoardService, DashBoardService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IGeoService, GeoService>();
            services.AddScoped<IAvailabilityService, AvailabilityService>();

            services.AddScoped<FileUploader>();
            //use for sinalR large file size upload
            services.Configure<HubOptions>(options =>
             {
                 options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB or use null
             });
            return services;
        }
    }
}