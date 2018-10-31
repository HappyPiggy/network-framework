using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T m_instance = null;

    public static T Instance
    {
        get { return m_instance; }
    }


    //设置成virtual 子类可以重写  protect可以被继承到子类
    protected virtual void Awake()
    {
        m_instance = this as T;
    }
}