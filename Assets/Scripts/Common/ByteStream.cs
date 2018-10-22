using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// byte[]储存 辅助类
/// </summary>
public class ByteStream:Stream
{
    private static int defaultSize = 64;//默认buf空间
    private static int packSize = 64;//buf不够后每次增长空间

    public byte[] buf;
    public int pos; //当前buf最右index
    public int used;//当前buf使用过的最右index
    public int capacity;
    public int offset;

    public override bool CanRead
    {
        get
        {
            return used > pos;
        }
    }

    public override bool CanSeek
    {
        get
        {
            return true;
        }
    }

    public override bool CanWrite
    {
        get
        {
            return used < capacity;
        }
    }

    public override long Length
    {
        get
        {
            return used;
        }
    }

    public override long Position
    {
        get
        {
            return pos;
        }
        set
        {
            pos = offset + (int)value;
        }
    }
    public ByteStream(int size)
    {
        ResetBytes(size);
    }
    public ByteStream()
    {
        ResetBytes(defaultSize);
    }

    private void ResetBytes(int size)
    {
        if (size > 0)
        {
            buf = new byte[size];
        }
        pos = 0;
        used = 0;
        capacity = size;
    }

    /// <summary>
    /// 写数据到buf
    /// </summary>
    /// <param name="p"></param>
    public void Write(byte[] p)
    {
        if (p == null || p.Length == 0) return;
        int len = p.Length;
        Grow(len);
        Buffer.BlockCopy(p,0,buf,pos,len);
        Move(len);
    }
    /// <summary>
    /// uint转byte写入buf
    /// </summary>
    /// <param name="n"></param>
    public void Write(ulong n)
    {
        Grow(8);
        buf[pos + 0] = (byte)(n & 0xff);
        buf[pos + 1] = (byte)(n >> 8 & 0xff);
        buf[pos + 2] = (byte)(n >> 16 & 0xff);
        buf[pos + 3] = (byte)(n >> 24 & 0xff);
        buf[pos + 4] = (byte)(n >> 32 & 0xff);
        buf[pos + 5] = (byte)(n >> 40 & 0xff);
        buf[pos + 6] = (byte)(n >> 48 & 0xff);
        buf[pos + 7] = (byte)(n >> 56 & 0xff);
        Move(8);
    }

    public byte[] GetUsedBytes()
    {
        byte[] bytes = new byte[used];
        Buffer.BlockCopy(buf, 0, bytes, 0, used);
        return bytes;
    }



    private void Move(int size)
    {
        pos += size;
        if (pos > used)
            used = pos;
    }

    /// <summary>
    /// 为buf分配空间
    /// </summary>
    /// <param name="size"></param>
    public void Grow(int size)
    {
        int left = capacity - pos;
        
        //剩余空间小于预计分配的空间 则要扩充cap
        if (left < size)
        {
            int need = size - left;
            capacity += Mathf.CeilToInt((float)need / packSize) * packSize;
            byte[] newBuf = new byte[capacity];

            if (used > 0)
                Array.Copy(buf,newBuf,used);
            buf = newBuf;
        }
    }



    public static byte[] StreamToBytes(Stream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Seek(0, SeekOrigin.Begin);
        stream.Read(bytes, 0, bytes.Length);
        return bytes;
    }

    public override void Flush()
    {
       
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
       
    }
}
