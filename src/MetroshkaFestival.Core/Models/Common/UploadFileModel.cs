using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MetroshkaFestival.Core.Models.Common
{
    public class UploadFileModel
    {
        public Stream Stream { get; set; }
        public string FileName { get; set; }
        public long Length { get; set; }
        public string StorageName { get; set; }

        public static async Task<UploadFileModel> Create(IFormFile formFile, string storageName)
        {
            var file = new UploadFileModel
            {
                FileName = formFile.FileName,
                Length = formFile.Length,
                Stream = new MemoryStream(),
                StorageName = storageName
            };
            await formFile.CopyToAsync(file.Stream);

            return file;
        }
    }
}