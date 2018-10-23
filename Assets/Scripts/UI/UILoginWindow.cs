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
        NetworkUpdater.Instance.HttpReqLogin(OnLoginRet,"a", accountText.text.ToString(), "p",passwordText.text.ToString());
    }

    private void OnLoginRet(RetLoginMsg data, RetErrorMsg error)
    {
        if (error != null || data == null)
        {
            string dec = string.Format("{0}\n（错误码:{1}）",error.ErrorReason,error.RetCode);
            UIManager.Instance.OpenWidget("UITipsWidget", dec);
            return;
        }

        bool res=HttpClient.SetSession(data.SessionKey, data.Id);
        if (res)
        {
            UIManager.Instance.OpenWidget("UITipsWidget","登录成功");
            UIManager.Instance.CloseWindow("UILoginWindow");
        }
        else
        {
            UIManager.Instance.OpenWidget("UITipsWidget", "session结构不对");
        }
    }


}