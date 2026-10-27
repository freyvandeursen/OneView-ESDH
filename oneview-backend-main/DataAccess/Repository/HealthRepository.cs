using DataAccess.Context;
using DataAccess.Interface;

namespace DataAccess.Repository
{
    internal class HealthRepository : IHealthRepository
    {
        private readonly Database _db;

        public HealthRepository(Database db)
        {
            _db = db;
        }

        public async Task<bool> CanConnectToDatabaseAsync()
        {
            try
            {
                {
                    return await _db.Database.CanConnectAsync();
                }
            }
            catch
            {
                return false;
            }
        }
    }
}