using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 计时器管理
/// 负责所有timer的实际计时工作
/// </summary>
public class TimeManager : MonoBehaviour
{
    private static TimeManager _instance;

    public static TimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                if (AppMain.GM == null)
                    return null;
                _instance = AppMain.GM.GetComponent<TimeManager>();
                if (_instance == null)
                    _instance = AppMain.GM.AddComponent<TimeManager>();
            }
            return _instance;
        }
    }

    public List<Timer> timerList = new List<Timer>();
    private float lastStartUpTime = 0;

    private void Start()
    {
        lastStartUpTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        for (int i = 0; i < timerList.Count; i++)
        {
            var t = timerList[i];
            //if (t.function.Target==null ||t.function.Target.Equals(null))
            //{
            //    timerList.RemoveAt(i);
            //    continue;
            //}

            t.curDelay += t.isUnsacale ? Time.realtimeSinceStartup - lastStartUpTime : Time.deltaTime;

            if (t.curDelay >= t.delay)
            {
                // Debug.Log("t.function.Target" + t.function.Target);

                t.function.Invoke(t.curDelay);
                t.curCallTime++;

                if (t.curCallTime >= t.callTime)
                {
                    t.Stop(false, i);
                    i--; //stop后从list中移除了一个timer
                }
                else
                {
                    t.curDelay = 0;//如果要call多次，重置计时器
                }
            }
        }
        lastStartUpTime = Time.realtimeSinceStartup;
    }


    public static Timer Start(Action<float> function, float delay, int callTime = 1)
    {
        var timer = new Timer();
        timer.Start(function, delay, callTime);
        return timer;
    }

}

/// <summary>
/// 计时器
/// 自身负责加入和删除timemanager的列表
/// </summary>
public class Timer
{
    public Action<float> function;
    public float delay;
    public int callTime;

    public float curDelay;
    public int curCallTime;
    public bool isUnsacale = false; //是否无视timescale

    public bool IsRunning
    {
        get;
        private set;
    }

    public Timer()
    {
        Reset();
        IsRunning = false;
    }

    /// <summary>
    /// timer的初始化
    /// </summary>
    /// <param name="function"></param>
    /// <param name="delay"></param>
    /// <param name="callTime"></param>
    public void Start(Action<float> function, float delay, int callTime = 1)
    {
        this.function = function;
        this.delay = delay;
        this.callTime = callTime;

        Reset();
        IsRunning = true;

        //重新加入timermanager的队尾
        var manager = TimeManager.Instance;
        for (int i = 0; i < manager.timerList.Count; i++)
        {
            if (this == manager.timerList[i])
            {
                manager.timerList.RemoveAt(i);
                break;
            }
        }
        manager.timerList.Add(this);
    }

    /// <summary>
    /// 停止timer
    /// </summary>
    /// <param name="isInvoke">是否在call一次func</param>
    /// <param name="index">具体停止哪个func</param>
    public void Stop(bool isInvoke, int index = -1)
    {
        Reset();
        IsRunning = false;

        var manager = TimeManager.Instance;
        if (index >= 0)
        {
            manager.timerList.RemoveAt(index);
        }
        else
        {
            for (int i = 0; i < manager.timerList.Count; i++)
            {
                if (this == manager.timerList[i])
                {
                    manager.timerList.RemoveAt(i);
                    break;
                }
            }
        }

        if (isInvoke)
            function.Invoke(curDelay);
    }





    private void Reset()
    {
        curDelay = 0;
        curCallTime = 0;
    }
}