using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 发送消息的封装
/// </summary>
public class SendPacket
{
    public int cmd;//协议号 2位
    public object protoObj;//协议结构
    public int uid;//玩家id 8位
    public bool hasUid = false;


    public SendPacket(int cmd, object protoObj)
    {
        this.cmd = cmd;
        this.protoObj = protoObj;
        hasUid = false;
    }

    public SendPacket(int cmd,object protoObj,int uid)
    {
        hasUid = true;

        this.cmd = cmd;
        this.protoObj = protoObj;
        this.uid = uid;
    }
}