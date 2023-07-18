using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Pool <K, T, P> : MonoBehaviour
    where K : Enum
    where T : Content<K> 
    where P : PoolContainer<T>, new()
{
    [SerializeField]
    private List<Info> objectInfo;
    [Serializable]
    private struct Info
    {
        public K Type;
        public T Prefab;
        public int StartCount;
    }

    private Dictionary<K, P> poolTiles = new();
    public static Pool<K, T, P> Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        InitPool();
    }

    private void InitPool()
    {
        var empty = new GameObject();
        foreach (var obj in objectInfo)
        {
            var conteiner = Instantiate(empty, transform, false);
            conteiner.name = obj.Type.ToString();
            poolTiles[obj.Type] = new P();
            poolTiles[obj.Type].Container = conteiner.transform;
            for (int i = 0; i < obj.StartCount; i++)
            {
                var create = InstantiateObject(obj.Type, conteiner.transform);
                poolTiles[obj.Type].Objects.Enqueue(create);
            }
        }
        Destroy(empty);
    }
    private T InstantiateObject(K type, Transform parent)
    {
        var create = Instantiate(objectInfo.Find(x => Enum.Equals(x.Type, type)).Prefab, parent);
        create.gameObject.SetActive(false);
        return create;
    }
    public T GetContent(K type)
    {
        var obj = poolTiles[type].Objects.Count > 0 ?
            poolTiles[type].Objects.Dequeue() : InstantiateObject(type, poolTiles[type].Container);
        obj.gameObject.SetActive(true);
        return obj;
    }
    public void DestroyContent(T obj)
    {
        poolTiles[obj.Type].Objects.Enqueue(obj);
        obj.transform.localPosition = Vector3.zero;
        obj.gameObject.SetActive(false);
    }
}

