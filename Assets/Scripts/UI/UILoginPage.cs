using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 登录背景界面
/// </summary>
public class UILoginPage : UIPage {

    private Button openLoginBtn;
    protected override void OnAwake()
    {
        base.OnAwake();
        Layer = UILayerDef.Page;

        openLoginBtn = Find<Button>("start");
        AddUIClickListener(openLoginBtn, OnOpenLoginBtnClick);
    }

    private void OnOpenLoginBtnClick()
    {
        UIManager.Instance.OpenWindow("UILoginWindow");
    }
}
