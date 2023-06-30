using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pool <K, T, P> : MonoBehaviour
    where K : Enum
    where T : Content<K>    
    where P : PoolConteiner<T>
{
    public static Pool<K, T, P> Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        InitPool();
    }
    protected abstract void InitPool();

    [Serializable]
    protected struct Info
    {
        public K Type;
        public T Prefab;
        public int StartCount;
    }
    [SerializeField]
    protected List<Info> objectInfo;
    protected Dictionary<K, P> poolTiles = new();
    protected T InstantiateObject(K type, Transform parent)
    {
        var create = Instantiate(objectInfo.Find(x => Enum.Equals(x.Type, type)).Prefab, parent);
        create.gameObject.SetActive(false);
        return create;
    }
    public T GetContent(K type)
    {
        var obj = poolTiles[type].Objects.Count > 0 ?
            poolTiles[type].Objects.Dequeue() : InstantiateObject(type, poolTiles[type].Conteiner);
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

