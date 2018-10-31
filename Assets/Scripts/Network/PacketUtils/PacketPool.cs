using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

/// <summary>
/// 收发消息队列
/// </summary>
public class PacketPool
{
    public AutoResetEvent sendEvent = new AutoResetEvent(false); //默认阻塞
    public Queue<ReceivePacket> recvPool = new Queue<ReceivePacket>();
    public Queue<SendPacket> sendPool = new Queue<SendPacket>();

    public void AddReceivePacket(ReceivePacket p)
    {
        lock (recvPool)
        {
            recvPool.Enqueue(p);
        }
    }

    public void AddSendPacket(SendPacket p)
    {
        lock (sendPool)
        {
            sendPool.Enqueue(p);
        }
        sendEvent.Set();
    }

    public ReceivePacket GetReceivePacket()
    {
        lock (recvPool)
        {
            if (recvPool.Count == 0)
                return null;
            return recvPool.Dequeue();
        }
    }

    public SendPacket GetSendPacket()
    {
        lock (sendPool)
        {
            if (sendPool.Count == 0)
                return null;
            return sendPool.Dequeue();
        }
    }

    public void ResetPool()
    {
        lock (recvPool)
            recvPool.Clear();
        lock (sendPool)
            sendPool.Clear();

        lock(this)
        {
            sendEvent.Close();
            sendEvent = new AutoResetEvent(false);
        }
    }
}