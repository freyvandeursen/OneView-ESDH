using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;

namespace OneView_API.Controllers {
    [Route("fileserver/files")]
    [ApiController]
    public class FileServerController : ControllerBase {

        private readonly IFileServerService _fileServerSerice;

        public FileServerController(IFileServerService fileServerSerice) {
            _fileServerSerice = fileServerSerice;
        }

        [HttpPost("presigned-url")]
        public async Task<ActionResult<string>> GeneratePreSignedUrlAsync([FromQuery] string fileKey) {
            try {
                var url = await _fileServerSerice.GeneratePreSignedUrlAsync(fileKey);
                return Ok(url);
            }
            catch (Exception) {
                return StatusCode(500, "Error generating presigned url");
            }
        }







    }
}