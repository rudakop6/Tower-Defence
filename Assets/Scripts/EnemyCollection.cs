using System;
using System.Collections.Generic;

[Serializable]
public class EnemyCollection
{
    private List<EnemyContent> _enemies = new List<EnemyContent>();
    public void Add(EnemyContent enemy)
    {
        _enemies.Add(enemy);
    }

    public void EnemiesUpdate()
    {
        for(int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i].EnemyUpdate())
                continue;

            int lastIndex = _enemies.Count - 1;
            _enemies[i] = _enemies[lastIndex];
            _enemies.RemoveAt(lastIndex);
            i -= 1;
        }
    }
}

