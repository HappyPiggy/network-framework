using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using usercmd;
/// <summary>
/// 接收ui层信息
/// 处理客户端发出的网络请求
/// </summary>
public class NetworkUpdater:MonoSingleton<NetworkUpdater>
{

    public static string AccountServer= "http://192.168.98.86:8080";
    public static string GameServer = "";


    protected override void Awake()
    {
        base.Awake();
    }


    public static string LoginURL
    {
        get
        {
            return AccountServer + "/login";
        }
    }



    #region  http请求
    /// <summary>
    /// 登录请求
    /// </summary>
    /// <param name="callback">请求成功后返回的结构</param>
    /// <param name="RetErrorMsg">请求失败后返回的结构</param>
    public void HttpReqLogin(Action<RetLoginMsg,RetErrorMsg> callback, params object[] args)
    {
        HttpManager.HttpRequest(LoginURL,(int)MsgType.Login,callback, args);
    }

    #endregion


    #region tcp
    /// <summary>
    /// 登录成功后连接网关服
    /// </summary>
    /// <param name="data"></param>
    public void OnConnectedServer(RetTeamAddr data)
    {
        if (data != null)
        {
            StopAllCoroutines();
            StartCoroutine(CheckConnect(data.Address,()=> {
                Debug.Log(" server connect success");
                TcpSendTest(data.Key);
            }));

        }
    }


    private IEnumerator CheckConnect(string addr,Action success,Action fail=null)
    {
        yield return new WaitForFixedUpdate();
        TcpManager.Instance.Disconnect();
        TcpManager.Instance.ResetSocket();
        yield return new WaitForFixedUpdate();

        if (TcpManager.Instance.Connect(addr))
        {
            success();
        }
        else
        {
            fail();
        }

    }

    /// <summary>
    /// 测试tcp消息
    /// </summary>
    /// <param name="data"></param>
    private void TcpSendTest(string data)
    {
        data = "test proto " + data;
        SendPacket p = new SendPacket(999, data);
        TcpManager.Instance.Send(p);
    }
    #endregion

}