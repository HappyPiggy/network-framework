using UnityEngine;

public class AppMain : MonoBehaviour {

    private GameObject uiRoot;

	void Start () {
        //初始化ui
        uiRoot = GameObject.Find("UIRoot");
        uiRoot.AddComponent<UIRoot>();

        UIManager.Instance.Init("UI/");
        UIManager.Instance.OpenPage("UILoginPage");

        //初始化网络组件
        gameObject.AddComponent<NetworkUpdater>();
      
    }

}
