using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using usercmd;
/// <summary>
/// 登录窗口
/// </summary>
public class UILoginWindow:UIWindow
{

    private InputField accountText;
    private InputField passwordText;

    private Button loginBtn;

    protected override void OnAwake()
    {
        base.OnAwake();

        btnClose = Find<Button>("cancel");
        accountText = Find<InputField>("account/InputField");
        passwordText = Find<InputField>("password/InputField");
        loginBtn = Find<Button>("login");

        AddUIClickListener(loginBtn, OnLoginBtnClick);
    }


    private void OnLoginBtnClick()
    {
        //用户登录操作
        Loom.QueueOnMainThread(()=> {
            NetworkUpdater.Instance.HttpReqLogin(OnLoginRet, "a", accountText.text.ToString(), "p", passwordText.text.ToString());
        });
       
    }

    /// <summary>
    /// 登录成功后返回消息
    /// </summary>
    /// <param name="data"></param>
    /// <param name="error"></param>
    private void OnLoginRet(RetLoginMsg data, RetErrorMsg error)
    {
        if (error != null || data == null)
        {
            string dec = string.Format("{0}\n（错误码:{1}）",error.ErrorReason,error.RetCode);
            UIManager.Instance.ShowTips(dec);
            return;
        }

        bool res=HttpClient.SetSession(data.SessionKey, data.Id);
        if (res)
        {
            UIManager.Instance.ShowTips("登录成功");
            UIManager.Instance.CloseWindow("UILoginWindow");

            OnLoginSuccess(data);
        }
        else
        {
            UIManager.Instance.ShowTips("session结构不对");
        }
    }

    /// <summary>
    /// 登录成功后一系列处理
    /// </summary>
    /// <param name="msg"></param>
    private void OnLoginSuccess(RetLoginMsg msg)
    {
        RecordAccountMsg(msg);
        ConnectedServer(msg);

    }

    /// <summary>
    /// 账号密码信息存在本地
    /// </summary>
    private void RecordAccountMsg(RetLoginMsg msg)
    {
        //todo 
    }

    private void ConnectedServer(RetLoginMsg msg)
    {
        RetTeamAddr addr = new RetTeamAddr();
        addr.Key = msg.Key;
        addr.Address = msg.Address;
        NetworkUpdater.Instance.OnConnectedServer(addr);
    }


}