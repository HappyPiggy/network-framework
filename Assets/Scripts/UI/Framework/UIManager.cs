using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// ui管理器
/// </summary>
public class UIManager : Singleton<UIManager>
{
    class UIPageTrack
    {
        public string name;
        public object arg;
        public Type type;
    }

    public static string MainScene = "Main";
    public static string MainPage = "UIMainPage";

    private Stack<UIPageTrack> pageTrackStack; //已开页的栈(只有页需要)
    private UIPageTrack currentPage;

    private List<UIPanel> loadedPanelList;

    private Action<string> onSceneLoadedOnly;


    public UIManager()
    {
        pageTrackStack = new Stack<UIPageTrack>();
        loadedPanelList = new List<UIPanel>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uiResRoot">ui加载目录</param>
    public void Init(string uiResRoot)
    {
        UIRes.UIResRoot = uiResRoot;

        pageTrackStack.Clear();
        loadedPanelList.Clear();

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (onSceneLoadedOnly != null) onSceneLoadedOnly(scene.name);
        };
    }

    /// <summary>
    /// 打开ui界面通用接口
    /// 不能被外部调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="arg"></param>
    /// <param name="implType"></param>
    /// <returns></returns>
    private T Open<T>(string name, object arg = null, Type implType = null) where T : UIPanel
    {
        T ui = UIRoot.Find<T>(GetShortUIName(name));
        if (ui == null)
        {
            ui = Load<T>(name, implType);
        }
        if (ui != null)
        {
            if (loadedPanelList.IndexOf(ui) < 0)
            {
                loadedPanelList.Add(ui);
            }

            ui.Open(arg);
            UIRoot.Sort();
        }
        else
        {
            Debug.LogError("UI不存在："+name);
        }
        return ui;
    }


    private T Load<T>(string name, Type implType) where T : UIPanel
    {
        T ui = default(T);
        GameObject original = UIRes.LoadPrefab(name);
        if (original != null)
        {
            GameObject go = GameObject.Instantiate(original);
            ui = go.GetComponent<T>();
            if (ui == null)
            {
                try
                {
                    ui = go.AddComponent(implType) as T;
                }
                catch (Exception e)
                {
                    Debug.LogError("无法自动添加抽象的UIPanel");
                }

            }

            if (ui != null)
            {
                go.name = GetShortUIName(name);
                UIRoot.AddChild(ui);
            }
            else
            {
                Debug.LogError("Prefab没有增加对应组件: " + name);
            }
        }
        else
        {
            Debug.LogError("Res Not Found: " + name);
        }

        return ui;
    }



    private string GetShortUIName(string name)
    {
        int i = name.LastIndexOf("/");
        if (i < 0) i = name.LastIndexOf("\\");
        if (i < 0) return name;
        return name.Substring(i + 1);
    }


    public UIPanel GetUI(string name)
    {
        string shortname = GetShortUIName(name);
        for (int i = 0; i < loadedPanelList.Count; i++)
        {
            if (loadedPanelList[i].name == shortname || loadedPanelList[i].name == name)
            {
                return loadedPanelList[i];
            }
        }
        return null;
    }

    private void CloseAllLoadedPanel()
    {
        for (int i = loadedPanelList.Count - 1; i >= 0; i--)
        {
            var panel = loadedPanelList[i];
            if (panel == null)
            {
                loadedPanelList.RemoveAt(i);
            }
            else if (panel.IsOpen)
            {
                panel.Close();
            }

        }
    }

    //==========================================================================================
    #region Page管理

    /// <summary>
    /// 打开page接口
    /// </summary>
    /// <param name="name">预制体名字</param>
    /// <param name="arg"></param>
    public void OpenPage( string name,object arg = null)
    {
        if (currentPage != null)
        {
            if (currentPage.name != name)
            {
                pageTrackStack.Push(currentPage);
            }

        }

        OpenPageWorker(name, arg, Type.GetType(name));
    }

    public void GoBackPage()
    {
        if (pageTrackStack.Count > 0)
        {
            var track = pageTrackStack.Pop();
            OpenPageWorker(track.name, track.arg, track.type);
        }
        else
        {
            EnterMainPage();
        }
    }

    private void OpenPageWorker(string page, object arg, Type type)
    {
        currentPage = new UIPageTrack
        {
            name = page,
            arg = arg,
            type = type
        };

        CloseAllLoadedPanel();

        Open<UIPage>(page, arg, type);
    }


    public void EnterMainPage()
    {
        pageTrackStack.Clear();
        OpenPageInScene(MainScene, MainPage, null, null);
    }

    /// <summary>
    /// 可夸场景打开ui界面
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="page"></param>
    /// <param name="arg"></param>
    /// <param name="type"></param>
    private void OpenPageInScene(string scene, string page, object arg, Type type)
    {

        string oldScene = SceneManager.GetActiveScene().name;
        if (oldScene == scene)
        {
            OpenPageWorker(page, arg, type);
        }
        else
        {
            LoadScene(scene, () =>
            {
                OpenPageWorker(page, arg, type);
            });
        }
    }


    public void LoadScene(string scene, Action onLoadComplete)
    {
        onSceneLoadedOnly = (sceneName) =>
        {
            if (sceneName == scene)
            {
                onSceneLoadedOnly = null;
                if (onLoadComplete != null) onLoadComplete();
               // CloseLoading(SceneLoading);
            }
        };

       // OpenLoading(SceneLoading);
        SceneManager.LoadScene(scene);
    }
    #endregion

    //=======================================================================



    //=======================================================================

    #region UIWindow管理
    /// <summary>
    /// 打开window接口
    /// </summary>
    /// <param name="name"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public UIWindow OpenWindow(string name,object arg = null)
    {
        UIWindow ui = Open<UIWindow>(name, arg, Type.GetType(name));
        return ui;
    }


    public void CloseWindow(string name, object arg = null)
    {
        UIWindow ui = GetUI(name) as UIWindow;
        if (ui != null)
        {
            ui.Close(arg);
        }
    }



    #endregion

    //=======================================================================




    //=======================================================================

    #region UIWidget管理

    public UIWidget OpenWidget(string name, object arg = null)
    {
        UIWidget ui = Open<UIWidget>(name, arg,Type.GetType(name));
        return ui;
    }

    public void CloseWidget(string name, object arg = null)
    {
        UIWidget ui = GetUI(name) as UIWidget;
        if (ui != null)
        {
            ui.Close(arg);
        }
    }

    #endregion


    //==========================================================================================
}