﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1
{
    public interface IMyFileProvider
    {
        Task<bool> PurgeAsync();
        Task<Dictionary<string, string>> Recreate();
        Task<Dictionary<string, string>> ReadThemAll();
    }
}
