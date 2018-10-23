using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using usercmd;
/// <summary>
/// 处理http请求相关
/// </summary>
public class HttpClient
{


    private DateTime lastSendTime = DateTime.Now;

    private byte[] recvData; //http请求返回data
    private Type recvObjectType;//data的type
    private object recvObject;//接收到byte反序列化后的obj

    private static byte[] sessionKey; //登录成功后返回的session和uid的组合
    private static byte[] sessionEnKey; //登录成功后返回的加密key
    public static byte[] defaultKey = new byte[]{ 211, 9, 192, 126, 155, 227, 52, 59, 155, 82, 72, 203, 165, 22, 212, 200 };//默认aes加密的key 


    private Type errorType;
    private object errorObject;


    private HttpManager httpManager;
    private string domainUrl;
    private Coroutine coroutine;

    private Delegate callback; //http请求的回调
    private Action<ErrorType> errCallBack;



    /// <summary>
    /// 新建httpclient
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static HttpClient New(string url)
    {
        HttpClient client = new HttpClient
        {
            httpManager = HttpManager.Instance,
            domainUrl = url
        };
        return client;
    }

    /// <summary>
    /// 发起http请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="RetErrorMsg"></typeparam>
    /// <param name="cmd"></param>
    /// <param name="callback"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public bool Request<T, RetErrorMsg>(int cmd,Action<T, RetErrorMsg> callback,params object[] args)
    {
        //todo
        //某种情况登录失败
        //return false

        recvObjectType = typeof(T);
        errorType = typeof(RetErrorMsg);
        this.callback = callback;
        HttpProcess(cmd,args,cmd!=1); //除了登录都要session

        return true;
    }

    /// <summary>
    /// 封装参数到reqHttpArgData
    /// </summary>
    /// <param name="cmd">协议号</param>
    /// <param name="args">具体内容</param>
    /// <param name="isNeedSession"></param>
    /// <returns></returns>
    private bool HttpProcess(int cmd,object[] args,bool isNeedSession)
    {
        if (args.Length % 2 != 0)
        {
            OnResult(ErrorType.ArgError);
            return false;
        }


        int argLen = args.Length / 2;
        try
        {
            ReqHttpArgData data = new ReqHttpArgData
            {
                Cmd = (uint)cmd
            };
            for (int i = 0; i < argLen; i++)
            {
                ReqHttpArgData.KeyVal arg = new ReqHttpArgData.KeyVal
                {
                    Key = args[i * 2].ToString(),
                    val = args[i * 2 + 1].ToString()
                };
                data.Args.Add(arg);
            }

            if (coroutine != null)
                httpManager.StopCoroutine(coroutine);
            coroutine = httpManager.StartCoroutine(RequestData(domainUrl,data,isNeedSession));
        }
        catch (Exception ex)
        {
            Debug.LogError("cmd :"+cmd+" err:"+ex.Message);
        }
        return true;
    }

    /// <summary>
    /// 发起http请求
    /// 发送数据，接收返回数据
    /// </summary>
    /// <param name="url"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerator RequestData(string url,ReqHttpArgData data,bool isNeedSession)
    {
        if ((DateTime.Now - lastSendTime).TotalSeconds < 0.05f)
        {
            yield return new WaitForEndOfFrame();
        }
        lastSendTime = DateTime.Now;

        MemoryStream stream = new MemoryStream();
        try
        {
            Serializer.Serialize(stream, data);
        }
        catch (Exception )
        {
            Debug.LogError("protobuf序列化失败");
        }

        //加密 
        //登录时数据尾部添加默认uid
        //登录成功后返回 session和uid
        //再次http时数据尾部添加 session和uid
        ByteStream writer = null;
        try
        {
            if (isNeedSession)
            {
                if(sessionKey==null || sessionEnKey == null)
                {
                    OnResult(ErrorType.ConnectError);
                    yield break;
                }

                byte[] msg = EncryptUtils.AesEncrypt(stream.ToArray(), sessionEnKey);
                writer= new ByteStream(msg.Length+24);//session 16 +uid 8 =24
                writer.Write(msg);
                writer.Write(sessionKey);
            }
            else
            {
                byte[] msg = EncryptUtils.AesEncrypt(stream.ToArray(), defaultKey);
                writer = new ByteStream(msg.Length + 8);//uid 8 
                writer.Write(msg);
                writer.Write((ulong)0);
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("加密失败 "+ex.Message);
        }

        WWW ret = new WWW(url, writer.GetUsedBytes());
        yield return ret;

        if (ret.error == null && ret.bytes != null)
        {
            recvData = ret.bytes;
            ParseData();
        }
        else
        {
            errorObject = ret.error;
            OnResult(ErrorType.ConnectError);

        }
    }

    /// <summary>
    /// 解析请求返回的数据
    /// </summary>
    private void ParseData()
    {
        //2byte消息号+protobuf数据
        int cmd= recvData[0]| (recvData[1]<<8);

        byte[] protoData = new byte[recvData.Length-2];

        try
        {
            Buffer.BlockCopy(recvData, 2, protoData, 0,recvData.Length-2);
        }
        catch (Exception ex)
        {
            Debug.LogError("解析数据出错 "+ex.Message);
            OnResult(ErrorType.ParseProtoBufError);
        }


        MemoryStream stream = new MemoryStream(protoData);
        stream.SetLength(protoData.Length);

        //错误协议号
        if (cmd == 25)
        {
            try
            {
                errorObject = RuntimeTypeModel.Default.Deserialize(stream, null, errorType);
            }
            catch (Exception)
            {
                Debug.LogError("protobuf反序列化失败");
                OnResult(ErrorType.ParseProtoBufError);
                return;
            }

            OnResult(ErrorType.CallbackError);
        }
        else//其他协议号
        {
            try
            {
                recvObject = RuntimeTypeModel.Default.Deserialize(stream, null, recvObjectType);
            }
            catch (Exception)
            {
                Debug.LogError("protobuf反序列化失败");
                OnResult(ErrorType.ParseProtoBufError);
                return;
            }

            OnResult(ErrorType.None);
        }
        
    }

    /// <summary>
    /// 登录成功需要保存session和uid 
    /// 之后的http请求用
    /// </summary>
    /// <param name="key"></param>
    /// <param name="uid"></param>
    /// <returns></returns>
    public static bool SetSession(byte[] key,ulong uid)
    {
        if (key.Length != 32)
            return false;

        byte[] session = new byte[16];
        byte[] enKey = new byte[16];

        Buffer.BlockCopy(key,0,session,0,16);
        Buffer.BlockCopy(key,16, enKey, 0,16);

        var buf = new ByteStream(24);
        buf.Write(session);
        buf.Write(uid);
        sessionKey = buf.GetUsedBytes();
        sessionEnKey = enKey;
        return true;
    }


    /// <summary>
    /// 处理http请求的返回结果
    /// </summary>
    /// <param name="type"></param>
    private void OnResult(ErrorType type)
    {
        switch (type)
        {
            case ErrorType.None:
                if (callback != null)
                    callback.Method.Invoke(callback.Target, new object[] { recvObject ,null});
                break;
            case ErrorType.CallbackError:
                if (callback != null)
                    callback.Method.Invoke(callback.Target, new object[] { null, errorObject });
                break;
            case ErrorType.ConnectError:
                if (callback != null)
                    callback.Method.Invoke(callback.Target, new object[] { null,
                    new RetErrorMsg() {
                        ErrorReason=errorObject.ToString() 
                    } }
                );
                break;
            default:
                Debug.Log("传入参数或解析数据出错");
                break;
        }
    }
}