using System.IO.Compression;

namespace Sunaba.Core;

public class ZipStream : Stream
{
    ZipArchive zipArchive;
    
    StreamMode mode;

    private String path;

    private ZipArchiveEntry Entry;

    private Stream Stream;
    
    public ZipStream(ZipArchive zipArchive, string path, StreamMode mode)
    {
        this.zipArchive = zipArchive;
        this.path = path;
        this.mode = mode;
        
        Entry = zipArchive.GetEntry(path);
        Stream = Entry.Open();
    }


    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (mode == StreamMode.Read || mode == StreamMode.ReadPlus || mode == StreamMode.WritePlus || mode == StreamMode.AppendPlus)
        {
            return Stream.Read(buffer, offset, count);
        }
        else
        {
            throw new Exception("Stream is not readable");
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (mode == StreamMode.Append || mode == StreamMode.ReadPlus || mode == StreamMode.WritePlus || mode == StreamMode.AppendPlus)
        {
            return Stream.Seek(offset, origin);
        }
        else
        {
            throw new Exception("Stream is not seekable");
        }
    }

    public override void SetLength(long value)
    {
        ZipArchiveEntry entry = zipArchive.GetEntry(path);
        Stream stream = entry.Open();
        stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (mode == StreamMode.Write || mode == StreamMode.WritePlus || mode == StreamMode.AppendPlus || mode == StreamMode.ReadPlus)
        {
            Stream.Write(buffer, offset, count);
        }
        else
        {
            throw new Exception("Stream is not writable");
        }
    }

    public override bool CanRead
    {
        get
        {
            if (mode == StreamMode.Read || mode == StreamMode.ReadPlus || mode == StreamMode.WritePlus || mode == StreamMode.AppendPlus)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public override bool CanSeek
    {
        get
        {
            if (mode == StreamMode.Append || mode == StreamMode.ReadPlus || mode == StreamMode.WritePlus || mode == StreamMode.AppendPlus)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public override bool CanWrite
    {
        get
        {
            if (mode == StreamMode.Write || mode == StreamMode.WritePlus || mode == StreamMode.AppendPlus || mode == StreamMode.ReadPlus)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public override long Length
    {
        get
        {
            return Entry.Length;
        }
    }

    public override long Position
    {
        get
        {
            return Stream.Position;
        }
        set
        {
            Stream.Position = value;
        }
    }
}