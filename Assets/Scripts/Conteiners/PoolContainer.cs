using System.Collections.Generic;
using UnityEngine;

public abstract class PoolContainer<T> where T : MonoBehaviour
{
    public Transform Conteiner { get; private set; }
    public Queue<T> Objects;
    public PoolContainer(Transform conteiner)
    {
        Conteiner = conteiner;
        Objects = new Queue<T>();
    }
}

