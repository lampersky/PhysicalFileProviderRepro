using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace WebApplication1
{
    public class MyFileProvider : PhysicalFileProvider, IMyFileProvider
    {
        //some problematic file names
        private readonly string[] Paths = new string[] {
            "firstFile.txt",
            "FirstLevelDirectory\\secondFile.txt",
            "FirstLevelDirectory\\SecondLevelDirectory\\thirdFile.txt",
            "FirstLevelDirectory\\SecondLevelDirectory\\ThirdLevelDirectory\\fourthFile.txt",
            "FirstLevelDirectory\\SecondLevelDirectory\\ThirdLevelDirectory\\1611678346913_20_INDIA - 6C - Login page - NETWORKING LOUNGE (f5e6955f-d106-4ebb-b46a-a83e9cd0845d).txt",
        };

        public MyFileProvider(string root) : base(root) { }

        /// <summary>
        /// method taken from OC (DefaultMediaFileStoreCacheFileProvider)
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> PurgeAsync()
        {
            var filesErrors = await PurgeFilesAsync("");
            var directoriesErrors = await PurgeDirectoriesAsync("");
            //var createErrors = await Recreate();

            return filesErrors
                .Concat(directoriesErrors)
                .ToDictionary(x => x.Key, x => x.Value); // || directoriesErrors; // || createErrors.Keys.Any();
        }

        public async Task<Dictionary<string, string>> PurgeFilesAsync(string path = "")
        {
            var errors = new Dictionary<string, string>();

            var folders = GetDirectoryContents(path);
            foreach (var fileInfo in folders)
            {
                if (fileInfo.IsDirectory)
                {
                    var subPath = Path.GetRelativePath(Root, fileInfo.PhysicalPath);
                    try
                    {
                        var newErrors = await PurgeFilesAsync(subPath);
                        errors = errors.Concat(newErrors).ToDictionary(x => x.Key, x => x.Value);
                    }
                    catch (Exception e)
                    {
                        errors.Add(subPath, e.Message);
                    }
                }
                else
                {
                    try
                    {
                        File.Delete(fileInfo.PhysicalPath);
                    }
                    catch (Exception e)
                    {
                        errors.Add(fileInfo.PhysicalPath, e.Message);
                    }
                }
            }

            return errors;
        }

        public async Task<Dictionary<string, string>> PurgeDirectoriesAsync(string path = "")
        {
            var errors = new Dictionary<string, string>();

            var folders = GetDirectoryContents(path);
            foreach (var fileInfo in folders)
            {
                if (fileInfo.IsDirectory)
                {
                    var subPath = Path.GetRelativePath(Root, fileInfo.PhysicalPath);
                    try
                    {
                        var newErrors = await PurgeDirectoriesAsync(subPath);
                        errors = errors.Concat(newErrors).ToDictionary(x => x.Key, x => x.Value);

                        File.SetAttributes(fileInfo.PhysicalPath, FileAttributes.Normal);
                        Directory.Delete("\\\\?\\" + fileInfo.PhysicalPath);
                    }
                    catch (Exception e)
                    {
                        errors.Add(subPath, e.Message);
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// this methods creates directory tree and file
        /// </summary>
        /// <param name="path"></param>
        /// <returns>null for success, error massage if fails</returns>
        static private string CreateFile(string path)
        {
            var dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (Exception e)
                {
                    return "Create Directory failed: " + e.Message;
                }
            }
            try
            {
                using var fs = File.Create(path);
                byte[] content = new UTF8Encoding(true).GetBytes("example file");
                fs.Write(content, 0, content.Length);
                return null;
            }
            catch (Exception e)
            {
                return "Create File failed: " + e.Message;
            }
        }

        /// <summary>
        /// this methods creates all problematic files
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, string>> Recreate()
        {
            var errors = new Dictionary<string, string>();
            foreach (var path in Paths)
            {
                var result = CreateFile(Root + path);
                if (result != null)
                {
                    errors.Add(path, result);
                }
            }

            return Task.FromResult(errors);
        }

        /// <summary>
        /// tries to read files and return their contents or exception message (if occurs)
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, string>> ReadThemAll()
        {
            var contents = new Dictionary<string, string>();
            foreach (var path in Paths)
            {
                string content;
                try
                {
                    content = File.ReadAllText(Root + path);
                }
                catch (Exception e)
                {
                    content = e.Message;
                }
                contents.Add(path, content);

            }

            return Task.FromResult(contents);
        }
    }
}
