using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ui基础类型
/// 全屏
/// </summary>
public abstract class UIPage : UIPanel
{
    public override UITypeDef UIType { get { return UITypeDef.Page; } }

    /// <summary>
    /// 返回按钮，大部分Page都会有返回按钮
    /// </summary>
    [SerializeField]
    private Button btnGoBack;


    /// <summary>
    /// 当UIPage被激活时调用
    /// </summary>
    protected override void OnEnable()
    {
        AddUIClickListener(btnGoBack, OnBtnGoBack);
    }

    /// <summary>
    /// 当UI不可用时调用
    /// </summary>
    protected override void OnDisable()
    {
        RemoveUIClickListeners(btnGoBack);
    }

    /// <summary>
    /// 当点击“返回”时调用
    /// 但是并不是每一个Page都有返回按钮
    /// </summary>
    private void OnBtnGoBack()
    {
        UIManager.Instance.GoBackPage();
    }
    
}