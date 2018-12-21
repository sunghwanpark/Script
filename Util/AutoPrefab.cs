using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public static partial class Extension
{
    private static Dictionary<Type, GameObject> typePrefabResource = new Dictionary<Type, GameObject>();

    public static GameObject GetTypePrefab(Type type, string prefabName)
    {
        GameObject prefab = null;
        if (!typePrefabResource.ContainsKey(type))
        {
            prefab = Resources.Load(prefabName) as GameObject;
            typePrefabResource.Add(type, prefab);
        }
        else
            prefab = typePrefabResource[type];

        return prefab;
    }

    public static GameObject GetTypePrefab(Type type, string bundleName, string prefabName)
    {
        GameObject prefab = null;
        if (!typePrefabResource.ContainsKey(type))
        {
            //prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, prefabName);
            typePrefabResource.Add(type, prefab);
        }
        else
            prefab = typePrefabResource[type];

        return prefab;
    }

    public static T Instantiate<T>(this MonoBehaviourBase caller, Transform parent = null) where T : MonoBehaviourBase
    {
        return Instantiate<T>(caller, parent.gameObject, null);
    }

    public static T Instantiate<T>(this MonoBehaviourBase caller, GameObject parent = null) where T : MonoBehaviourBase
    {
        return Instantiate<T>(caller, parent, null);
    }

    public static T Instantiate<T>(this MonoBehaviourBase caller, GameObject parent, params object[] args) where T : MonoBehaviourBase
    {
        Type type = typeof(T);
        var attr = type.GetCustomAttribute<AutoPrefab>();
        if (attr == null)
            return null;

        T inst = null;
        GameObject prefab = null;
        if (parent == null)
            parent = GameObject.Find("UI Root");

        if (!string.IsNullOrEmpty(attr._assetbundleName))
        {
            // 에셋번들에서 로드
            prefab = GetTypePrefab(type, attr._assetbundleName, attr._prefabName);
        }
        else
        {
            prefab = GetTypePrefab(type, attr._prefabName);
        }

        GameObject newObject = null;
        newObject = NGUITools.AddChild(parent, prefab);

        inst = newObject.GetComponent<T>();
        if (inst == null)
            inst = newObject.AddComponent<T>();

        inst.Init(args);
        return inst;
    }
}

/// <summary>
/// use [AutoPrefab("Prefab Path"))]
/// </summary>
public class AutoPrefab : Attribute
{
    public readonly string _prefabName;
    public readonly string _assetbundleName;

    public AutoPrefab(string prefabName)
    {
        _prefabName = prefabName;
        _assetbundleName = string.Empty;
    }

    public AutoPrefab(string prefabName, string bundleName)
    {
        _prefabName = prefabName;
        _assetbundleName = bundleName;
    }
}