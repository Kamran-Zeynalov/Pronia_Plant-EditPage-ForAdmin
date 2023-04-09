namespace P230_Pronia.Utilities.Extensions
{
    public static class FileUpload
    {
        public static async Task<string> CreateImage(this IFormFile file,string imagesFolderPath,string folder)
        {
            var destinationPath = Path.Combine(imagesFolderPath, folder);
            Random r = new();
            int random = r.Next(0, 1000);
            var fileName = string.Concat(random, file.FileName);
            var path = Path.Combine(destinationPath, fileName);

            using (FileStream stream = new(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;

        }

        public static bool IsValidLength(this IFormFile file,double size)
        {
            return (double)file.Length / 1024 / 1024 <= size;
        }

        public static bool IsValidFile(this IFormFile file,string type)
        {
            return file.ContentType.Contains(type);
        }

        public static bool DeleteImage(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
