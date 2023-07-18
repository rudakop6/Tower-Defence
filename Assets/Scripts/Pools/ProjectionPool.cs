using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionPool : MonoBehaviour
{
    [Serializable]
    protected struct Info
    {
        public TileContentType Type;
        public TileContent Prefab;
    }
    [SerializeField]
    private List<Info> objectInfo;

    public static ProjectionPool Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        InitPool();
    }
    private Dictionary<TileContentType, TileContent> poolProjection = new();
    private void InitPool()
    {
        var empty = new GameObject();
        foreach (var obj in objectInfo)
        {
            var container = Instantiate(empty, transform, false);
            container.name = obj.Type.ToString();
            poolProjection[obj.Type] = InstantiateObject(obj.Type, container.transform);
        }
        Destroy(empty);
    }
    private TileContent InstantiateObject(TileContentType type, Transform parent)
    {
        var create = Instantiate(objectInfo.Find(x => Enum.Equals(x.Type, type)).Prefab, parent);
        create.gameObject.SetActive(false);
        return create;
    }
    public TileContent GetContent(TileContentType type)
    {
        var obj = poolProjection[type];
        obj.gameObject.SetActive(true);
        return obj;
    }
    public void DestroyContent(TileContent obj)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.gameObject.SetActive(false);
    }
}

