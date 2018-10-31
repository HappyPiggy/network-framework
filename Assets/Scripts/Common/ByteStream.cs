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
    public int readPos; //当前已读的buf中的索引
    public int used;//当前buf使用过的最右index
    public int capacity;
    public int offset;

    /// <summary>
    /// 注意返回的是int不是byte
    /// </summary>
    /// <returns></returns>
    public override int ReadByte()
    {
        readPos++;
        return buf[readPos - 1];
    }

    public byte[] ReadByte(int count)
    {
        if (count <= 0 || readPos+count>=capacity)
            return null;
        byte[] newBuf = new byte[count];
        Buffer.BlockCopy(buf,readPos,newBuf,0,count);
        readPos += count;
        return newBuf;
    }

    public int ReadInt16()
    {
        int a = ReadByte();
        int b = ReadByte();
        return a + (b << 8);//如果是byte操作 则a[0]|a[1]<<8
    }

    public override bool CanRead
    {
        get
        {
            return used > readPos;
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
            return readPos;
        }
        set
        {
            readPos = offset + (int)value;
        }
    }

    public int UnreadBytes
    {
        get
        {
            return used - readPos;
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
        readPos = 0;
        used = 0;
        capacity = size;
    }

    public void Append(ByteStream recvBuf)
    {
        Grow(recvBuf.used);
        int left = capacity - used;
        if (left < recvBuf.used)
        {
            capacity = used + recvBuf.used;
            byte[] newBuf = new byte[capacity];
            if (used > 0)
            {
                Array.Copy(buf,newBuf,used);
            }
            Array.Copy(recvBuf.buf,0,newBuf,used,recvBuf.used);
            buf = newBuf;
        }
        else
        {
            Array.Copy(recvBuf.buf,0,buf,used,recvBuf.used);
        }

        used += recvBuf.used;
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
        Buffer.BlockCopy(p,0,buf,readPos,len);
        Move(len);
    }

    /// <summary>
    /// uint转byte写入buf
    /// </summary>
    /// <param name="n"></param>
    public void Write(ulong n)
    {
        Grow(8);
        buf[readPos + 0] = (byte)(n & 0xff);
        buf[readPos + 1] = (byte)(n >> 8 & 0xff);//0xff=....0000 11111111  转无符
        buf[readPos + 2] = (byte)(n >> 16 & 0xff);
        buf[readPos + 3] = (byte)(n >> 24 & 0xff);
        buf[readPos + 4] = (byte)(n >> 32 & 0xff);
        buf[readPos + 5] = (byte)(n >> 40 & 0xff);
        buf[readPos + 6] = (byte)(n >> 48 & 0xff);
        buf[readPos + 7] = (byte)(n >> 56 & 0xff);
        Move(8);

       // BitConverter.GetBytes
    }


    public void Write(ushort n)
    {
        Grow(2);
        buf[readPos + 0] = (byte)(n & 0xff);
        buf[readPos + 1] = (byte)(n >> 8 & 0xff);
        Move(2);
    }

    public byte[] GetUsedBytes()
    {
        byte[] bytes = new byte[used];
        Buffer.BlockCopy(buf, 0, bytes, 0, used);
        return bytes;
    }


    /// <summary>
    /// 读了buf不一定写了buf
    /// 写了buf当做已读buf，所以写buf的时候
    /// readPos也要移动
    /// </summary>
    /// <param name="size"></param>
    private void Move(int size)
    {
        readPos += size;
        if (readPos > used)
            used = readPos;
    }

    /// <summary>
    /// 为buf分配空间
    /// </summary>
    /// <param name="size"></param>
    public void Grow(int size)
    {
        int left = capacity - readPos;
        
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

    public void Clear()
    {
        readPos = 0;
        used = 0;
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
