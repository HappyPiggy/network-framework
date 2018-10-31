using UnityEngine;

public class AppMain : MonoBehaviour {

    public static GameObject uiRoot;
    public static GameObject GM; //游戏总管理器


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
         GM.AddComponent<TcpManager>();

        Test();
    }


    void Test()
    {
        //byte[] x = { 1,1 };
        ////int a = 1;
        ////int b = 1;
        ////print(a+(b<<8));
        //print(x[0]|x[1]<<8);
    }

}
