using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace WebApplication1
{
    public class MyFileProvider : PhysicalFileProvider, IMyFileProvider
    {
        public MyFileProvider(string root) : base(root)
        {

        }

        public MyFileProvider(string root, ExclusionFilters filters) : base(root, filters)
        {

        }

        public IDirectoryContents MyGetDirectoryContents(string subpath = null)
        {
            return GetDirectoryContents(subpath ?? String.Empty);
        }

        public IFileInfo MyGetFileInfo(string subpath)
        {
            return GetFileInfo(subpath);
        }

        public Task<bool> PurgeAsync()
        {
            var hasErrors = false;
            var folders = GetDirectoryContents(String.Empty);
            foreach (var fileInfo in folders)
            {
                if (fileInfo.IsDirectory)
                {
                    try
                    {
                        Directory.Delete(fileInfo.PhysicalPath, true);
                    }
                    catch (IOException ex)
                    {
                        hasErrors = true;
                    }
                }
                else
                {
                    try
                    {
                        File.Delete(fileInfo.PhysicalPath);
                    }
                    catch (IOException ex)
                    {
                        hasErrors = true;
                    }
                }
            }

            return Task.FromResult(hasErrors);
        }

        static private (bool, string) CreateFile(string path)
        {
            var dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            try
            {
                using var fs = File.Create(path);
                byte[] info = new UTF8Encoding(true).GetBytes("example file");
                fs.Write(info, 0, info.Length);
                return (false, "success");
            }
            catch (IOException ioe)
            {
                return (true, ioe.Message);
            }
        }

        private string[] Paths = new string[] {
                "FirstLevelDirectory\\SecondLevelDirecotry\\ThirdLevelDirectory\\firstFile.txt",
                "FirstLevelDirectory\\SecondLevelDirecotry\\ThirdLevelDirectory\\1611678346913_20_INDIA - 6C - Login page - NETWORKING LOUNGE (f5e6955f-d106-4ebb-b46a-a83e9cd0845d).txt",
                //"111\\222\\333\\secondFile.txt",
                //"111\\222\\333\\444\\555\\666\\777\\888\\999\\000\\anotherFile.txt",
                //"111\\1611678346913_20_INDIA - 6C - Login page - NETWORKING LOUNGE (f5e6955f-d106-4ebb-b46a-a83e9cd0845d).txt",
                //"testDirecotry\\file.txt"
            };

        public Task<Dictionary<string, string>> Recreate()
        {
            var hasErrors = false;
            var errors = new Dictionary<string, string>();

            foreach (var path in Paths)
            {
                var result = CreateFile(Root + path);
                hasErrors = hasErrors || result.Item1;
                if (result.Item1) {
                    errors.Add(path, result.Item2);
                }
            }
            return Task.FromResult(errors);
        }

        public Task<Dictionary<string, string>> ReadThemAll()
        {
            var contents = new Dictionary<string, string>();
            foreach (var path in Paths)
            {
                var content = "";
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