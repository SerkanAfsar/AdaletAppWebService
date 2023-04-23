using AdaletApp.WEBAPI.Abstract;

namespace AdaletApp.WEBAPI.Concrete
{
    public class FileService : IFileService
    {
        public void DeleteFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", path);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);

                }
            }
        }
    }
}
