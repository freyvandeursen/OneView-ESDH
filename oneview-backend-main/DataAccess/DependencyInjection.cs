using Amazon.Runtime;
using Amazon.S3;
using DataAccess.Context;
using DataAccess.Interface;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
        {
            // Respositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHealthRepository, HealthRepository>();
            services.AddScoped<IFileServerRepository, FileServerRepository>();
            services.AddScoped<IContextRepository, ContextRepository>();
            services.AddScoped<ILabelRepository, LabelRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();

            return services;
        }

        public static IServiceCollection SetupDatabase(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<Database>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
                options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            });
            return services;
        }

        public static IServiceCollection SetupRedis(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = configuration["REDIS:ConnectionString"]!;
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(connectionString)
            );
            return services;
        }

        public static IServiceCollection SetupAWS(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var accessKey = configuration["AWS:AccessKey"];
            var secretKey = configuration["AWS:SecretKey"];
            var serviceUrl = configuration["AWS:ServiceURL"];

            var credentials = new BasicAWSCredentials(accessKey, secretKey);

            var config = new AmazonS3Config { ServiceURL = serviceUrl };

            var s3Client = new AmazonS3Client(credentials, config);

            services.AddSingleton<IAmazonS3>(s3Client);

            return services;
        }

        public static void ApplyDatabaseUpdates(IServiceProvider servies)
        {
            using var scope = servies.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            try
            {
                var dbContext = serviceProvider.GetRequiredService<Database>();
                var path = Path.Combine(AppContext.BaseDirectory, "Context", "change.sql");
                ChangeSqlExecutor.ApplyPendingChanges(dbContext, path);
                Console.WriteLine("Database updates applied successfully.");
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while applying database updates.");
                throw;
            }
        }
    }
}
