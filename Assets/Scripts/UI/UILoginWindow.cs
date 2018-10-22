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
        Layer = UILayerDef.NormalWindow;

        btnClose = Find<Button>("cancel");
        accountText = Find<InputField>("account/InputField");
        passwordText = Find<InputField>("password/InputField");
        loginBtn = Find<Button>("login");

        AddUIClickListener(loginBtn, OnLoginBtnClick);
    }


    private void OnLoginBtnClick()
    {
        //用户登录操作
        NetworkUpdater.Instance.HttpReqLogin(OnLoginRet,"account", accountText.ToString(), "password",passwordText.ToString());
    }

    private void OnLoginRet(RetLoginMsg data, RetErrorMsg error)
    {
        if (error != null || data == null)
        {
            Debug.Log("login收到消息出错 错误号:" + error.RetCode);
            Debug.Log("错误描述 :" + error.JsonParam);
            return;
        }

        Debug.Log(data.Account);
        Debug.Log(data.Id);
        HttpClient.SetSession(data.SessionKey, data.Id);
    }


}