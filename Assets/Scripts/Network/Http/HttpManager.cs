using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// 统一管理httpclient的请求创建
/// </summary>
public  class HttpManager:MonoBehaviour
{
    private static HttpManager _instance;
    public static HttpManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("HttpManager");
                _instance = go.AddComponent<HttpManager>();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 新建一个httpclient发起请求
    /// </summary>
    /// <typeparam name="T">返回的结构类型</typeparam>
    /// <typeparam name="RetErrorMsg">返回的错误结构类型</typeparam>
    /// <param name="url">请求url</param>
    /// <param name="cmd">协议号</param>
    /// <param name="callback">结束回调</param>
    /// <param name="args">请求所带的参数</param>
    public static void HttpRequest<T, RetErrorMsg>(string url,int cmd,Action<T, RetErrorMsg> callback,params object[] args)
    {
        HttpClient.New(url).Request(cmd,callback,args);
    }
}