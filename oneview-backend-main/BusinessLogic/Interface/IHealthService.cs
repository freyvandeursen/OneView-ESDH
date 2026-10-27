namespace BusinessLogic.Interface {

    public interface IHealthService {
        Task<bool> CheckDatabaseConnectionAsync();

    }

}