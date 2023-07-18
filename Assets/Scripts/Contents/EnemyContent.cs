using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EnemyContent : Content<EnemyType>
{
    private Tile _tileFrom, _tileTo;
    private Vector3 _positionFrom, _positionTo;
    private float _progress;

    public override void Recycle()
    {
        EnemyPool.Instance.DestroyContent(this);
    }
    public void SpawnOn(Tile tile)
    {        
        //Vector3 tilePosition = tile.transform.localPosition;
        //transform.localPosition = new Vector3(tilePosition.x, tilePosition.y + 0.75f, tilePosition.z);
        transform.localPosition = tile.transform.localPosition;
        _tileFrom = tile;
        _tileTo = GetTileTo(tile);
        _positionFrom = _tileFrom.transform.localPosition;
        _positionTo = _tileTo.transform.localPosition;
        _progress = 0f;
    }

    public bool EnemyUpdate()
    {
        _progress += Time.deltaTime;

        if(_progress >= 0.5f)
        {
            _tileFrom.Lock = false;
            _tileTo.Lock = true;
            if (_tileTo.Content?.ContentType == ContentType.Projection)
                _tileTo.Content = null;
        }

        while (_progress >= 1)
        {
            _tileFrom = _tileTo;
            _tileTo = GetTileTo(_tileTo);
            
            if (_tileTo == null)
            {
                Recycle();
                return false;
            }

            _positionFrom = _positionTo;
            _positionTo = _tileTo.transform.localPosition;
            _progress -= 1f;
        }

        transform.localPosition = Vector3.LerpUnclamped(_positionFrom, _positionTo, _progress);
        return true;
    }

    private Tile GetTileTo(Tile tile)
    {
        if(!tile.NextTilesOnPath.Any())
            return null;
        
        List<Tile> list = tile.NextTilesOnPath.Values.ToList();
        return list[Random.Range(0, list.Count)];
    }
}

