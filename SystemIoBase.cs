using System;
using System.Collections.Generic;
using System.IO;

namespace Sunaba.Core
{

    public class SystemIoBase : IoInterface
    {
        public virtual string GetFileUrl(string path)
        {
            return PathUrl + "/" + path;
        }

        public override string LoadText(string assetPath)
        {
            if (!assetPath.Contains(PathUrl))
                assetPath = PathUrl + assetPath;
            
            string path = GetFilePath(assetPath);

            if (!File.Exists(path))
            {
                return null;
            }

            if (path == null || !File.Exists(path))
            {
                throw new FileNotFoundException(assetPath + " not found");
                return null;
            }

            var txt = File.ReadAllText(path);
            return txt;
        }

        public override void SaveText(string assetPath, string text)
        {
            if (!assetPath.Contains(PathUrl))
                assetPath = PathUrl + assetPath;
            
            string path = GetFilePath(assetPath);
            if (path == null)
            {
                throw new Exception("Unable to save" + assetPath);
                return;
            }

            File.WriteAllText(path, text);
        }


        public override byte[] LoadBytes(string assetPath)
        {
            if (!assetPath.Contains(PathUrl))
                assetPath = PathUrl + assetPath;
            
            string path = GetFilePath(assetPath);
            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllBytes(path);
        }

        public override void SaveBytes(string assetPath, byte[] bytes)
        {
            if (!assetPath.Contains(PathUrl))
                assetPath = PathUrl + assetPath;
            
            string path = GetFilePath(assetPath);
            if (path == null)
            {
                throw new Exception("Unable to save" + assetPath);
                return;
            }

            File.WriteAllBytes(path, bytes);
        }

        public override List<String> GetFileList(string path = "", string extension = "", bool recursive = true)
        {
            
            path = GetFilePath(path);

            if (!Directory.Exists(path))
            {
                return new List<string>();
            }

            List<string> assets = new List<string>();

            if (path == "")
            {
                path = PathUrl;
            }
            else if (!path.EndsWith("/"))
            {
                path += "/";
            }

            var pathArry = path.Split('/');

            foreach (string file in Directory.GetFiles(path))
            {
                if (extension != "")
                {
                    if (file.EndsWith(extension))
                    {
                        assets.Add(GetFileUrl(file));
                    }
                }
                else
                {
                    assets.Add(GetFileUrl(file));
                }
            }

            foreach (string directory in Directory.GetDirectories(path))
            {
                var dirArry = directory.Split('/');
                if (dirArry[dirArry.Length - 2] != pathArry[pathArry.Length - 2])
                {
                    continue;
                }
                
                if (extension == "/")
                    if (!directory.EndsWith(extension))
                        assets.Add(GetFileUrl(directory) + "/");
                    else 
                        assets.Add(GetFileUrl(directory));

                if (recursive)
                {
                    List<string> dirAssets = GetFileList(directory, extension, recursive);
                    foreach (string asset in dirAssets)
                    {
                        assets.Add(asset);
                    }
                }
            }

            return assets;
        }

        public override int CreateDirectory(string path)
        {
            if (!path.Contains(PathUrl))
                throw new Exception("Illegal Path: Path must start with " + PathUrl);
            
            path = GetFilePath(path);
            if (path == null)
            {
                return 1;
            }
            else if (Directory.Exists(path))
            {
                return 2;
            }
            else
            {
                Directory.CreateDirectory(path);
                return 0;
            }
        }

        public override void DeleteDirectory(string path)
        {
            if (!path.Contains(PathUrl))
                throw new Exception("Illegal Path: Path must start with " + PathUrl);
            
            path = GetFilePath(path);
            if (path == null)
            {
                return;
            }            

            Directory.Delete(path, true);
        }

        public override void DeleteFile(string path)
        {
            if (!path.Contains(PathUrl))
                throw new Exception("Illegal Path: Path must start with " + PathUrl);
            
            path = GetFilePath(path);
            if (path == null)
            {
                return;
            }

            File.Delete(path);
        }

        public override bool DirectoryExists(string path)
        {
            if (!path.Contains(PathUrl))
                throw new Exception("Illegal Path: Path must start with " + PathUrl);
            
            path = GetFilePath(path);
            return Directory.Exists(path);
        }
        
        public override Stream GetStream(string path, StreamMode mode)
        {
            if (!path.Contains(PathUrl))
                throw new Exception("Illegal Path: Path must start with " + PathUrl);
            
            path = GetFilePath(path);
            if (path == null)
            {
                return null;
            }

            if (mode == StreamMode.Read)
            {
                return File.OpenRead(path);
            }
            else if (mode == StreamMode.Write)
            {
                return File.OpenWrite(path);
            }
            else if (mode == StreamMode.Append)
            {
                return File.Open(path, FileMode.Append);
            }
            else if (mode == StreamMode.ReadPlus)
            {
                return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            }
            else if (mode == StreamMode.WritePlus)
            {
                return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            }
            else if (mode == StreamMode.AppendPlus)
            {
                return File.Open(path, FileMode.Append, FileAccess.ReadWrite);
            }
            else
            {
                return null;
            }
        }
    }
}