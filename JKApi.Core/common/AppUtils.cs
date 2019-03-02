using System.IO;
using System.Text;
using System.Web;

namespace JKApi.Core
{
    public static class AppUtils
    {
        private static readonly NLogger NLogger = NLogger.Instance;

        public static string GetOrCreateDirectory(string path)
        {
            var hasDirectory = Directory.Exists(path);
            if (!hasDirectory)
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static void SaveFile(string path, HttpPostedFile file, string fileUniqueId)
        {
            var hasDirectory = Directory.Exists(path);
            if (!hasDirectory)
            {
                NLogger.Debug($"Create new directory: {path}");
                Directory.CreateDirectory(path);
            }
            file.SaveAs(Path.Combine(path, fileUniqueId));
        }

        public static void DeleteFile(string path)
        {
            var hasFile = File.Exists(path);
            if (hasFile)
            {
                NLogger.Debug($"Delete file: {path}");
                File.Delete(path);
            }
        }
    }
}
