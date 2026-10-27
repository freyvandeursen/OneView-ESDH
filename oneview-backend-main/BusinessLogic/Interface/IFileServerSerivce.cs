namespace BusinessLogic.Interface {
    public interface IFileServerService {
        Task<string> GeneratePreSignedUrlAsync(string fileKey);
    }

}