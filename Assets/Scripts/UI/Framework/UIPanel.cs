using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 所有ui界面基类
/// </summary>
public abstract class UIPanel : MonoBehaviour
{
    public virtual UITypeDef UIType { get { return UITypeDef.Unkown; } }

    private int layer = UILayerDef.Unkown;
    public int Layer { get { return layer; } set { layer = value; } }

    public Delegate onClose;

    public bool IsOpen { get { return gameObject.activeSelf; } }

    private void Awake()
    {
        OnAwake();
    }

    private void Update()
    {
        OnUpdate();
    }


    public void Open(object arg = null)
    {

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        OnOpen(arg);
    }

    public void Close(object arg = null)
    {

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        OnClose(arg);
        if(onClose!=null)
        onClose.Method.Invoke(onClose.Target,new object[] { arg});
    }

    /// <summary>
    /// 方便寻找Panel上的UI控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    public T Find<T>(string controlName) where T : MonoBehaviour
    {
        Transform target = transform.Find(controlName);
        if (target != null)
        {
            return target.GetComponent<T>();
        }
        else
        {
            Debug.LogError("未找到UI控件：" +controlName);
            return default(T);
        }
    }



    /// <summary>
    /// 为UIPanel内的脚本提供便捷的UI事件监听接口
    /// </summary>
    /// <param name="target"></param>
    /// <param name="listener"></param>
    public void AddUIClickListener(UIBehaviour target, Action listener)
    {
        if (target != null)
        {
            UIEventTrigger.Get(target).onClick -= listener;
            UIEventTrigger.Get(target).onClick += listener;
        }
    }


    /// <summary>
    /// 移除UI控件的所有监听器
    /// </summary>
    /// <param name="target"></param>
    public void RemoveUIClickListeners(UIBehaviour target)
    {
        if (target != null)
        {
            if (UIEventTrigger.HasExistOn(target.transform))
            {
                UIEventTrigger.Get(target).onClick = null;
            }
        }
    }


    /// <summary>
    /// 移除UI控件的监听器
    /// </summary>
    /// <param name="controlName"></param>
    /// <param name="listener"></param>
    public void RemoveUIClickListener(string controlName, Action<string> listener)
    {
        Transform target = transform.Find(controlName);
        if (target != null)
        {
            if (UIEventTrigger.HasExistOn(target))
            {
                UIEventTrigger.Get(target).onClickWithName -= listener;
            }
        }
        else
        {
            Debug.LogError("未找到UI控件："+ controlName);
        }
    }

    protected virtual void OnAwake()
    {
    }

    protected virtual void OnUpdate()
    {

    }

    protected virtual void OnOpen(object arg = null)
    {

    }

    protected virtual void OnClose(object arg = null)
    {

    }



    protected virtual void OnDestroy()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

}