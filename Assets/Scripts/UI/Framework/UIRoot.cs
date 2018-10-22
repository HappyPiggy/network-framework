using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIRoot : MonoBehaviour
{
    private static Camera uiCamera;
    private static CanvasScaler canvasScaler;
    private static Transform cameraRoot; //所有ui的根

    void Awake()
    {
        //让UIRoot一直存在于所有场景中
        DontDestroyOnLoad(gameObject);
        uiCamera = transform.Find("Camera").GetComponent<Camera>();
        cameraRoot = uiCamera.transform;
        canvasScaler = GetComponent<CanvasScaler>();
    }

    public CanvasScaler GetUIScaler()
    {
        return canvasScaler;
    }

    public Camera GetUICamera()
    {
        return uiCamera;
    }

    /// <summary>
    /// 从UIRoot下通过名字&类型寻找一个组件对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T Find<T>(string name) where T : MonoBehaviour
    {
        GameObject obj = Find(name);
        if (obj != null)
        {
            return obj.GetComponent<T>();
        }
        return null;
    }



    public static GameObject Find(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }


        Transform obj = null;
        GameObject root = cameraRoot.gameObject;
        if (root != null)
        {
            Transform t = root.transform;
            for (int i = 0; i < t.childCount; i++)
            {
                Transform c = t.GetChild(i);
                if (c.name == name)
                {
                    obj = c;
                    break;
                }
            }
        }

        if (obj != null)
        {
            return obj.gameObject;
        }
        return null;
    }

    private static GameObject FindUIRoot()
    {
        GameObject root = GameObject.Find("UIRoot");
        if (root != null && root.GetComponent<UIRoot>() != null)
        {
            return root;
        }
        Debug.LogError("UIRoot Is Not Exist!!!");
        return null;
    }


    /// <summary>
    /// 当一个UIPage/UIWindow/UIWidget添加到UIRoot下面
    /// </summary>
    /// <param name="child"></param>
    public static void AddChild(UIPanel child)
    {
        GameObject root = FindUIRoot();
        if (root == null || child == null)
        {
            return;
        }
        child.transform.SetParent(cameraRoot, false);

    }


    public static void Sort()
    {
        GameObject root = FindUIRoot();
        if (root == null)
        {
            return;
        }

        List<UIPanel> list = new List<UIPanel>();
        root.GetComponentsInChildren<UIPanel>(true, list);
        list.Sort((a, b) =>
        {
            return a.Layer - b.Layer;
        });

        for (int i = 0; i < list.Count; i++)
        {
            list[i].transform.SetSiblingIndex(i);
        }
    }

}