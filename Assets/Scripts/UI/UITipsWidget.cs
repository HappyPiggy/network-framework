using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public class UITipsWidget:UIWidget
{
    private Text tipsText;
    private float hideTime = 1.5f; //提示框隐藏时间

    protected override void OnAwake()
    {
        base.OnAwake();

        tipsText = Find<Text>("content");
    }

    protected override void OnOpen(object arg = null)
    {
        base.OnOpen(arg);

        tipsText.text = arg.ToString();

        TimeManager.Start((float t) =>
        {
            UIManager.Instance.CloseWidget("UITipsWidget");
        }, hideTime);
    }

}
