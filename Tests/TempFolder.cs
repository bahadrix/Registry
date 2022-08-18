using System;
using System.IO;

namespace Tests;

public class TempFolder : IDisposable
{
    public TempFolder(string prefix = "Test")
    {
        var folderName = $"{prefix}_{Guid.NewGuid()}";
        Folder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), folderName));
    }

    public DirectoryInfo Folder { get; }

    public void Dispose()
    {
        Directory.Delete(Folder.FullName, true);
    }
}