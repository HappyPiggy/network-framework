using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
/// <summary>
/// 处理实际发送和接收数据的socket
/// </summary>
public class TcpClient
{
    private Socket socket = null;
    private Thread sendThread = null;
    private Thread recvThread=null;

    private ByteStream recvBuf;//接收到数据的缓存
    private ByteStream recvStream;//每一次接收到数据的临时流


    public PacketPool packetPool;
    public MessagePacker messagePacker;
    private Action onLink; //连接后的回调

    private long isConnected; //多线程访问

    public bool IsConnected
    {
        get
        {
            if (socket == null || !socket.Connected || Interlocked.Read(ref isConnected) == 0)
                return false;
            return true;
        }
    }

    public TcpClient()
    {
        packetPool = new PacketPool();
        messagePacker = new MessagePacker();
        isConnected = 0;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="addr">ip:port</param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public bool Connect(string addr,Action callback=null)
    {
        if (socket != null && socket.Connected)
            return true;

        onLink = callback;
        var pars = addr.Split(':');
        if (pars.Length != 2)
            return false;

        string ip = pars[0];
        int port = int.Parse(pars[1]);
        IPEndPoint address = new IPEndPoint(IPAddress.Parse(ip),port);
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

        socket.BeginConnect(address,new AsyncCallback(ConnectCallback),null);
        return true;
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        socket.EndConnect(ar);
        packetPool.ResetPool();
        Debug.Log("tcp success");

        isConnected = 1;
        recvBuf = new ByteStream(socket.ReceiveBufferSize);
        recvStream = new ByteStream(socket.ReceiveBufferSize);
        
        //只有当 前台线程全部结束，应用程序才能被卸载（才能被关闭）
        recvThread = new Thread(new ThreadStart(OnReceiveDataThread))
        {
            IsBackground = false,
            Priority = System.Threading.ThreadPriority.Normal
        };
        recvThread.Start();

        sendThread = new Thread(new ThreadStart(OnSendDataThread))
        {
            IsBackground = false,
            Priority = System.Threading.ThreadPriority.Normal
        };
        sendThread.Start();

        if (onLink != null)
            onLink();
    }

    private void OnReceiveDataThread()
    {
        while (true)
        {
            if (Interlocked.Read(ref isConnected) == 1)
            {
                ReceiveData();
            }
            else
            {
                return;
            }

        }
    }

    /// <summary>
    /// 事件驱动
    /// 客户端添加事件后才会执行发送
    /// 否则2min发送一次
    /// </summary>
    private void OnSendDataThread()
    {
        AutoResetEvent sendEvnet = packetPool.sendEvent;
        while (true)
        {
            if(Interlocked.Read(ref isConnected) == 1)
            {
                try
                {
                    sendEvnet.WaitOne(TimeSpan.FromMinutes(2));
                    SendData();
                }catch(Exception ex)
                {
                    Debug.LogError("消息发送失败 "+ex.Message);
                    continue;
                }
               
            }
            else
            {
                return;
            }
         
        }
    }

    private void ReceiveData()
    {
        if (socket == null || !socket.Connected)
        {
            Close();
            return;
        }
        else
        {
            recvStream.Clear();
            int i = socket.Receive(recvStream.buf);

            if (i < 0)
            {
                Close();
                return;
            }

            recvStream.used = i;
            recvBuf.Append(recvStream);
            //当缓存中剩余未读数据至少是2字节时继续读(2字节为包长度大小)
            while(recvBuf.UnreadBytes>2 && socket != null)
            {
                int msgLen = recvBuf.ReadInt16();
                
                //如果剩余未读是完整数据包则读取 否则接收到完整数据包再读
                if (msgLen > recvBuf.UnreadBytes)
                {
                    int cmd = recvBuf.ReadInt16();
                    ReceivePacket p = messagePacker.Read(cmd,recvBuf, msgLen-2);//减去cmd的2位
                    if(p!=null)
                         packetPool.AddReceivePacket(p);

                    if (recvBuf.UnreadBytes == 0)
                    {
                        recvBuf.Clear();
                        return;
                    }
                }
                else
                {
                    recvBuf.readPos -= 2;
                    return;
                }
            }
        }
    }

    private void SendData()
    {
        if(socket==null || !socket.Connected)
        {
            Close();
            return;
        }
        else
        {
            SendPacket p;
            while ((p = packetPool.GetSendPacket()) != null)
            {
                byte[] msg = messagePacker.Write(p);
                if (msg != null)
                {
                    socket.Send(msg);
                }
            }
        }
    }

    public void Close()
    {
        if (Interlocked.CompareExchange(ref isConnected, 0, 1) == 0)
            return;
        if (socket != null)
            socket.Close();

        onLink = null;
        socket = null;
    }

    public void Reset()
    {
        Close();
        if (sendThread != null && sendThread.IsAlive)
            packetPool.sendEvent.Set();
    }

}