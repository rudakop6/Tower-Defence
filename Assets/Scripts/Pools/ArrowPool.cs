using System;
using System.Collections.Generic;
using UnityEngine;
public class ArrowPool : Pool<ArrowType, ArrowContent, ArrowConteiner>
{
    protected override void InitPool()
    {
        var empty = new GameObject();
        foreach (var obj in objectInfo)
        {
            var conteiner = Instantiate(empty, transform, false);
            conteiner.name = obj.Type.ToString();
            poolTiles[obj.Type] = new ArrowConteiner(conteiner.transform);            
            for (int i = 0; i < obj.StartCount; i++)
            {
                var create = InstantiateObject(obj.Type, conteiner.transform);
                poolTiles[obj.Type].Objects.Enqueue(create);
            }
        }
        Destroy(empty);
    }
}

