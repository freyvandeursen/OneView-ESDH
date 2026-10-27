using DataAccess;
using DataAccess.Context;
using DataAccess.Interface;
using DataAccess.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Test_Unit
{
    public static class TestDependencySetup
    {
        public static ServiceProvider BuildServiceProvider()
        {
            var connectionString = Environment.GetEnvironmentVariable("ONEVIEW_TEST_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Set ONEVIEW_TEST_CONNECTION_STRING to the connection string for the disposable OneViewTest database before running repository tests."
                );
            }

            var connectionSettings = new SqlConnectionStringBuilder(connectionString);
            if (!string.Equals(connectionSettings.InitialCatalog, "OneViewTest", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Repository tests reset database objects and may only target the disposable OneViewTest database. Set Database=OneViewTest in ONEVIEW_TEST_CONNECTION_STRING."
                );
            }

            var services = new ServiceCollection();

            //Database connection
            services.AddDbContext<Database>(options =>
                options.UseSqlServer(connectionString));

            //Create logging
            services.AddLogging();

            //DataAccess services
            services.AddLogging();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHealthRepository, HealthRepository>();
            services.AddScoped<IFileServerRepository, FileServerRepository>();
            services.AddScoped<IContextRepository, ContextRepository>();
            services.AddScoped<ILabelRepository, LabelRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();

            //Build service provider
            var provider = services.BuildServiceProvider();

            //Deploy database updates
            DependencyInjection.ApplyDatabaseUpdates(provider);

            return provider;
        }
    }
}
