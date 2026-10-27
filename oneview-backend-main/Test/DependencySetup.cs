using Reqnroll.Microsoft.Extensions.DependencyInjection;
using BusinessLogic.Interface;
using DataAccess.Repository;
using DataAccess.Interface;
using BusinessLogic.Service;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Test {
    [Binding]
    public class DependencySetup {
        [ScenarioDependencies]
        public static IServiceCollection createServices() {
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "oneview-test-signing-key-at-least-32-bytes"
                })
                .Build());

            //db
            services.AddDbContext<Database>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            //services/repos
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();

            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IFileServerService, FileServerService>();
            services.AddScoped<IFileServerRepository, FileServerRepository>();


            return services;
        }
    }
}
