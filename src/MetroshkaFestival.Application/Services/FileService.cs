using System;
using System.IO;
using System.Threading.Tasks;
using Interfaces.Application;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data;
using Microsoft.EntityFrameworkCore;
using File = MetroshkaFestival.Data.Entities.File;

namespace MetroshkaFestival.Application.Services
{
    public class FileService : IService
    {
        private const string StorageRoot = "wwwroot/files";
        private readonly DataContext _dataContext;

        public FileService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private async Task<File> GetFileAsync(int id)
        {
            return await _dataContext.Files.SingleOrDefaultAsync(x => x.Id == id);
        }

        private async Task AddAsync(File file)
        {
            file.CreationDate = DateTime.UtcNow;
            await _dataContext.Files.AddAsync(file);
        }

        public async Task<File> UploadFileAsync(UploadFileModel fileModel, string contentType)
        {
            var directoryPath = Path.Combine(StorageRoot, fileModel.StorageName);
            Directory.CreateDirectory(directoryPath);

            var fileExtension = Path.GetExtension(fileModel.FileName);
            var fileName = $"{GenerateFileName(directoryPath)}{fileExtension}";
            var filePath = Path.Combine(directoryPath, fileName);
            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileModel.Stream.Position = 0;
                await fileModel.Stream.CopyToAsync(fileStream);
            }

            var file = new File
            {
                OriginalFileName = fileModel.FileName,
                Path = Path.Combine(fileModel.StorageName, fileName).Replace('\\', '/'),
                Length = (int) fileModel.Length,
                ContentType = contentType
            };

            await AddAsync(file);
            return file;
        }

        private static string GenerateFileName(string directoryPath)
        {
            while (true)
            {
                var newFileName = Guid.NewGuid().ToString();
                var filePath = Path.Combine(directoryPath, newFileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return newFileName;
                }
            }
        }

        public FileStream GetStream(File file)
        {
            var path = Path.Combine(StorageRoot, file.Path);
            return new FileStream(path, FileMode.Open);
        }

        public string GetFileName(File file)
        {
            return Path.GetFileName(file.Path);
        }

        public async Task DeleteAsync(int id)
        {
            var file = await GetFileAsync(id);
            var fullPath = Path.Combine(StorageRoot, file.Path);
            System.IO.File.Delete(fullPath);

            _dataContext.Files.Remove(file);
        }
    }
}