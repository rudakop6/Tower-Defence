﻿using System.Collections.Generic;
using UnityEngine;

public abstract class PoolConteiner<T> where T : MonoBehaviour
{
    public Transform Conteiner { get; private set; }
    public Queue<T> Objects;
    public PoolConteiner(Transform conteiner)
    {
        Conteiner = conteiner;
        Objects = new Queue<T>();
    }
}

