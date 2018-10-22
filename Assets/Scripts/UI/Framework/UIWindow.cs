using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIWindow : UIPanel
{
    public override UITypeDef UIType { get { return UITypeDef.Window; } }

    protected Button btnClose;

    /// <summary>
    /// 当UI可用时调用
    /// </summary>
    protected override void OnEnable()
    {
        AddUIClickListener(btnClose, OnBtnClose);
    }

    /// <summary>
    /// 当UI不可用时调用
    /// </summary>
    protected override void OnDisable()
    {
        RemoveUIClickListeners(btnClose);
    }

    private void OnBtnClose()
    {
        Close();
    }
}