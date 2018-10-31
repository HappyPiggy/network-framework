using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// 处理收发的二进制消息与proto之间的转换
/// </summary>
public class MessagePacker
{
    public MessageData serverMessageData;

    public MessagePacker()
    {
        serverMessageData = new MessageData();
    }

    /// <summary>
    /// 用protobuf将结构转二进制
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public  byte[] Write(SendPacket p)
    {
        MemoryStream stream = new MemoryStream();
        try
        {
            Serializer.Serialize(stream, p.protoObj);
        }
        catch (Exception)
        {
            Debug.LogError("protobuf序列化失败");
        }


        //msg len占2位 uid占8位
        // len+cmd+[uid]+body
        byte[] msgbody = stream.ToArray();
        int msgLen = 0; 
        if (p.hasUid)
        {
            msgLen = msgbody.Length + 2 + 8;// body+cmd+uid
            ByteStream writer = new ByteStream(msgLen+2);

            writer.Write((ushort)msgLen);
            writer.Write((ushort)p.cmd);
            writer.Write((ulong)p.uid);
            writer.Write(msgbody);
            return writer.GetUsedBytes();
        }
        else
        {
            msgLen = msgbody.Length + 2 ;// body+cmd
            ByteStream writer = new ByteStream(msgLen + 2);

            writer.Write((ushort)msgLen);
            writer.Write((ushort)p.cmd);
            writer.Write(msgbody);
            return writer.GetUsedBytes();
        }
    }

    /// <summary>
    /// 读取二进制转换为结构
    /// </summary>
    /// <returns></returns>
    public  ReceivePacket Read(int cmd,ByteStream recvStream,int len)
    {
        ServerMessage msg = serverMessageData.GetServerMessage(cmd);
        if (msg == null)
            return null;
        ReceivePacket p = new ReceivePacket
        {
            cmd = cmd,
            callback = msg.callback.Method,
        };
        
        byte[] msgbody = recvStream.ReadByte(len);
        if(msgbody==null || msgbody.Length == 0)
        {
            p.protoObj = Activator.CreateInstance(msg.msgType);
            return p;
        }

        MemoryStream stream = new MemoryStream(msgbody);
        stream.SetLength(msgbody.Length);
        try
        {
            p.protoObj = RuntimeTypeModel.Default.Deserialize(stream, null, msg.msgType);
        }
        catch (Exception)
        {
            Debug.LogError("protobuf反序列化失败");
        }

        return p;
    }
}