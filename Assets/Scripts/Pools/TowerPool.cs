using System.Collections.Generic;
using UnityEngine;

public class TowerPool : Pool<TowerType, TowerContent, TowerConteiner>
{
    protected override void InitPool()
    {
        var empty = new GameObject();
        foreach (var obj in objectInfo)
        {
            var conteiner = Instantiate(empty, transform, false);
            conteiner.name = obj.Type.ToString();
            poolTiles[obj.Type] = new TowerConteiner(conteiner.transform);
            for (int i = 0; i < obj.StartCount; i++)
            {
                var create = InstantiateObject(obj.Type, conteiner.transform);
                poolTiles[obj.Type].Objects.Enqueue(create);
            }
        }
        Destroy(empty);
    }
}

