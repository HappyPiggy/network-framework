using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
/// <summary>
/// 收到消息的封装
/// </summary>
public class ReceivePacket
{
    public int cmd;//协议号 2位
    public object protoObj;//协议结构
    public MethodInfo callback;//收到消息后的回调

    public void Execute()
    {
        if (callback != null)
        {
            callback.Invoke(null,new object[] { protoObj});
        }
    }

}