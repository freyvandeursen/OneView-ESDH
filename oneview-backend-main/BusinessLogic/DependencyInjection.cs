using BusinessLogic.Interface;
using BusinessLogic.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic {

    public static class DependencyInjection {

        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services, IConfiguration configuration) {
            services.AddScoped<IFileServerService, FileServerService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IHealthService, HealthService>();
            services.AddScoped<IContextService, ContextService>();
            services.AddScoped<ILabelService, LabelService>();
            services.AddScoped<IContactService, ContactService>();

            if (configuration.GetValue("FileSync:Enabled", true)) {
                services.AddHostedService<ContextSyncJob>();
            }
            return services;
        }


    }

}
