using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class ServerMessage
{
    public Delegate callback;
    public Type msgType;
}


/// <summary>
/// 用于客户端注册返回消息号与本地回调
/// </summary>
public class MessageData
{
    public delegate void ServerMsgHandler<T>(T arg);
    public Dictionary<int, ServerMessage> serverMessages = new Dictionary<int, ServerMessage>();
  
    public void RegisterHandle<T>(int cmd, ServerMsgHandler<T> handler)
    {
        if (serverMessages.ContainsKey(cmd))
            serverMessages.Remove(cmd);

        ServerMessage serverMessage = new ServerMessage();
        serverMessage.callback = handler;
        serverMessage.msgType = typeof(T);
        serverMessages.Add(cmd,serverMessage);
    }

    public ServerMessage GetServerMessage(int cmd)
    {
        if (serverMessages.ContainsKey(cmd))
            return serverMessages[cmd];
        return null;
    }
}