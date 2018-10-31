using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// 网络服务中间类
/// 连接 登录
/// </summary>
public class TcpManager:MonoSingleton<TcpManager>
{
    private TcpClient tcpClient;

   // private Queue<ReceivePacket> recvPacketPool = new Queue<ReceivePacket>();
    public bool isLogin = false;
    private ReceivePacket packet;


    public bool IsConnected
    {
        get
        {
            if (tcpClient == null)
                return false;
            return tcpClient.IsConnected;
        }
    }

    public TcpManager()
    {
    }

    private void Update()
    {
        //一帧执行一次回调
        if(tcpClient!=null && tcpClient.packetPool.recvPool.Count > 0)
        {
            if ((packet = tcpClient.packetPool.GetReceivePacket()) != null)
            {
                packet.Execute();
            }
        }
    }


    public bool Connect(string addr)
    {
        isLogin = false;

        //recvPacketPool.Clear();
        tcpClient = new TcpClient();
        return tcpClient.Connect(addr);
    }

    public void Disconnect()
    {
       // recvPacketPool.Clear();
        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
        }
    }

    public bool Send(SendPacket p)
    {
        if (tcpClient == null || !tcpClient.IsConnected)
            return false;
        tcpClient.packetPool.AddSendPacket(p);
        return true;
    }

    public void Destroy()
    {
        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
        }
    }

    public void ResetSocket()
    {
        if (tcpClient != null)
            tcpClient.Reset();
    }
}