
namespace DataAccess.Interface
{
    public interface IHealthRepository
    {
        Task<bool> CanConnectToDatabaseAsync();
    }
}