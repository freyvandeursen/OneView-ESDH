
namespace DataAccess.Interface {

    public interface IFileServerRepository {

        Task SyncAllContextsAsync();

        Task<string> GeneratePreSignedUrlAsync(string fileKey);

    }
}
