using UnityEngine;

public class AppMain : MonoBehaviour {

    //private static  AppMain _instance;
    //public static AppMain Instance
    //{
    //    get
    //    {
    //        return _instance;
    //    }
    //}

    public static GameObject uiRoot;
    public static GameObject GM; //游戏总管理器

    //private void Awake()
    //{
    //    _instance = this;
    //}



    void Start () {

        //初始化游戏总管理
        GM = new GameObject("GM");

        //初始化ui
        uiRoot = GameObject.Find("UIRoot");
        uiRoot.AddComponent<UIRoot>();

        UIManager.Instance.Init("UI/");
        UIManager.Instance.OpenPage("UILoginPage");

        //初始化网络组件
        GM.AddComponent<NetworkUpdater>();
      
    }

}
