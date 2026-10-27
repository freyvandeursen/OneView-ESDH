using DataAccess.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Logging;


public class ContextSyncJob : BackgroundService {

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContextSyncJob> _logger;

    public ContextSyncJob(IServiceProvider serviceProvider, ILogger<ContextSyncJob> logger) {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {

        while (!stoppingToken.IsCancellationRequested) {

            try {

                _logger.LogInformation("Running periodic context sync at {time}", DateTime.UtcNow);

                using var scope = _serviceProvider.CreateScope();
                var fileServerRepository = scope.ServiceProvider.GetRequiredService<IFileServerRepository>();

                await fileServerRepository.SyncAllContextsAsync();
            }
            catch (Exception ex) {

                _logger.LogError(ex, "Error occurred during context sync.");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}