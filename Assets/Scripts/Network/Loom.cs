using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
/// <summary>
/// unity多线程辅助
/// </summary>
public class Loom : MonoBehaviour
{
    public static int maxThreads = 8;
    private static int numThreads;

    private static Loom _current;
    private int _count;

    private Queue<Action> _actions = new Queue<Action>();

    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();//外部随时可以加入的执行列表
    private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();//每一帧执行的

    public static Loom Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    void Awake()
    {
        _current = this;
        initialized = true;
    }

    static bool initialized;

    static void Initialize()
    {
        if (!initialized)
        {

            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject("Loom");
            DontDestroyOnLoad(g);
            _current = g.AddComponent<Loom>();
            ThreadPool.SetMaxThreads(20,20);
            ThreadPool.SetMinThreads(10,10);
        }

    }


    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }


    /// <summary>
    /// 执行要在主线程运行的
    /// </summary>
    /// <param name="action"></param>
    /// <param name="time">延时执行</param>
    public static void QueueOnMainThread(Action action, float time=0)
    {
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Enqueue(action);
            }
        }
    }


    /// <summary>
    /// 线程池中取出工作线程运行计算
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Thread RunAsync(Action a)
    {
        Initialize();
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
        }

    }


    void OnDisable()
    {
        if (_current == this)
        {

            _current = null;
        }
    }



    void Update()
    {
        if (_actions.Count > 0)
        {
            lock (_actions)
            {
                int count = _actions.Count;
                while (count-- > 0)
                    _actions.Dequeue()();
            }
        }



        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }
        foreach (var delayed in _currentDelayed)
        {
            delayed.action();
        }



    }
}
