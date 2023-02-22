using Entertainmentcareers.net.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Entertainmentcareers.net.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public FileController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task<ActionResult<List<FileUpload>>> UploadFile(List<IFormFile> files)
        {
            List<FileUpload> uploadResults = new List<FileUpload>();
            foreach (var file in files)
            {
                var uploadResult = new FileUpload();
                var untrustedFileName = file.FileName;

                string trustedFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(file.FileName));
                var path = Path.Combine(_env.ContentRootPath, "FileUploads", trustedFileName);

                await using FileStream fileStream = new(path, FileMode.Create);
                await file.CopyToAsync(fileStream);

                uploadResult.OriginalFileName = untrustedFileName;
                uploadResult.StoredFileName = trustedFileName;
                uploadResult.FileContentType = file.ContentType;
                uploadResult.FilePath = path;
                uploadResults.Add(uploadResult);

            }
            return Ok(uploadResults);
        }
    }
}
