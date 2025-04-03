using System.IO.Compression;

namespace Sunaba.Core;

public class IoInterfaceZip : IoInterface
{
    ZipArchive zipArchive;
    
    public IoInterfaceZip(string path, string pathUrl)
    {
        PathUrl = pathUrl;
        if (path.EndsWith(".spkg") || path.EndsWith(".zip"))
        {
            zipArchive = ZipFile.OpenRead(path);
        }
        else
        {
            throw new Exception("Invalid Zip File");
        }
    }

    public IoInterfaceZip(byte[] buffer, string pathUrl)
    {
        PathUrl = pathUrl;
        MemoryStream ms = new MemoryStream(buffer);
        zipArchive = new ZipArchive(ms);
    }
    
    public override string GetFilePath(string path)
    {
        if (path.StartsWith(PathUrl))
        {
            path = path.Replace(PathUrl, "");
            if (path == null)
            {
                throw new Exception("Path Conversion Error");
            }
        }
        else if (path.StartsWith("./"))
        {
            path = path.Replace("./", "");
            if (path == null)
            {
                throw new Exception("Path Conversion Error");
            }
        }
        if (path.Contains("\\"))
        {
            path = path.Replace("\\/", "/");
            path = path.Replace("\\", "/");
        }
        if (path.Contains("/"))
        {
            path = path.Replace("/", "\\");
        }
        path = path.Replace("\\", "/");
        return path;
    }

    public string SanitizePath(string path)
    {
        path = path.Replace("\\/", "\\");
        path = path.Replace("\\", "/");
        return path;
    }
    
    public override string LoadText(string assetPath)
    {
        string path = GetFilePath(assetPath);
        ZipArchiveEntry entry = zipArchive.GetEntry(path);
        if (entry == null)
        {
            return null;
        }
        using (StreamReader reader = new StreamReader(entry.Open()))
        {
            return reader.ReadToEnd();
        }
    }

    public override byte[] LoadBytes(string assetPath)
    {
        string path = GetFilePath(assetPath);
        ZipArchiveEntry entry = zipArchive.GetEntry(path);
        if (entry == null)
        {
            return null;
        }
        using (MemoryStream ms = new MemoryStream())
        {
            entry.Open().CopyTo(ms);
            return ms.ToArray();
        }
    }

    public override List<string> GetFileList(string path, string extension = "", bool recursive = true)
    {
        path = GetFilePath(path);
        List<string> assets = new List<string>();
        foreach (var entry in zipArchive.Entries)
        {
            if (extension != "")
            {
                if (entry.FullName.StartsWith(path) && entry.FullName.EndsWith(extension))
                {
                    assets.Add(PathUrl + SanitizePath(entry.FullName));
                }
            }
            else
            {
                if (entry.FullName.StartsWith(path))
                {
                    assets.Add(PathUrl + SanitizePath(entry.FullName));
                }
            }
        }
        return assets;
    }

    public override Stream GetStream(string path, StreamMode mode)
    {
        path = GetFilePath(path);

        return new ZipStream(zipArchive, path, mode);
    }
}