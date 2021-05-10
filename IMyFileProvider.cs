using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public interface IMyFileProvider
    {
        Task<bool> PurgeAsync();
        IDirectoryContents MyGetDirectoryContents(string subpath);
        IFileInfo MyGetFileInfo(string subpath);
        Task<Dictionary<string, string>> Recreate();
        Task<Dictionary<string, string>> ReadThemAll();
    }
}
