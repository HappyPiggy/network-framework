using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using usercmd;
/// <summary>
/// 接收处理 客户端发出的网络请求
/// </summary>
public class NetworkUpdater:MonoBehaviour
{
    public static NetworkUpdater Instance { get; private set; }

    public static string AccountServer= "http://192.168.98.86:8080";
    public static string GameServer = "";

    private void Awake()
    {
        Instance = this;
    }


    public static string LoginURL
    {
        get
        {
            return AccountServer + "/login";
        }
    }

    /// <summary>
    /// 登录请求
    /// </summary>
    /// <param name="callback">请求成功后返回的结构</param>
    /// <param name="RetErrorMsg">请求失败后返回的结构</param>
    public void HttpReqLogin(Action<RetLoginMsg,RetErrorMsg> callback, params object[] args)
    {
        HttpManager.HttpRequest(LoginURL,(int)MsgType.Login,callback, args);
    }

}