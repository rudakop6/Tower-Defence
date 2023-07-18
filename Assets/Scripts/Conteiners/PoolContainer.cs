using System.Collections.Generic;
using UnityEngine;

public abstract class PoolContainer<T> where T : MonoBehaviour
{
    public Transform Container { get; set; }
    public Queue<T> Objects;
    public PoolContainer()
    {
        Objects = new Queue<T>();
    }
}

