using BusinessLogic.Interface;
using DataAccess.Interface;

namespace BusinessLogic.Service {
    public class FileServerService : IFileServerService {
        private readonly IFileServerRepository _fileServerRepository;

        public FileServerService(IFileServerRepository fileServerRepository) {
            _fileServerRepository = fileServerRepository;
        }

        public async Task<string> GeneratePreSignedUrlAsync(string fileKey) {
            return await _fileServerRepository.GeneratePreSignedUrlAsync(fileKey);
        }






    }
}