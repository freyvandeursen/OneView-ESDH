using BusinessLogic.Interface;
using DataAccess.Interface;

namespace BusinessLogic.Service
{
    public class HealthService : IHealthService
    {

        private readonly IHealthRepository _healthRepository;

        public HealthService(IHealthRepository healthRepository)
        {
            _healthRepository = healthRepository;
        }

        public async Task<bool> CheckDatabaseConnectionAsync()
        {
            return await _healthRepository.CanConnectToDatabaseAsync();
        }
    }
}