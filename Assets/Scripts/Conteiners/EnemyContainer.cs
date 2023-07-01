using UnityEngine;

public class EnemyContainer : PoolContainer<EnemyContent>
{
    public EnemyContainer(Transform conteiner) : base(conteiner) { }
}
