using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private Transform _ground;
    [SerializeField]
    private Tile _tilePrefab;

    private Tile[] _tiles;
    private Vector2Int _size;
    private Queue<Tile> _searchFrontier { get; } = new();
    private List<Tile> _spawnPoints { get; } = new();
    private List<Tile> _destinationPoints { get; } = new();

    public int SpawnPointsCount => _spawnPoints.Count;
    public void Initialize(Vector2Int size)
    {
        _size = size;
        _ground.localScale = new Vector3(_size.x, _size.y, 1f);

        var offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
        _tiles = new Tile[size.x * size.y];
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                Tile tile = _tiles[i] = Instantiate(_tilePrefab, transform, false);
                tile.name = string.Format("Tile {0}", i);
                tile.Identificator = i;

                tile.transform.localPosition = new Vector3(x - offset.x, Constants.OffsetTileY, y - offset.y);

                if (x > 0)
                    Tile.CreateEastWestLinks(tile, _tiles[i - 1]);
                if (y > 0)
                    Tile.CreateNorthSouthLinks(tile, _tiles[i - size.x]);
            }
        }
        CreateBuilding(_tiles[_tiles.Length / 2], TileContentType.Destination);
        CreateBuilding(_tiles[0], TileContentType.SpawnPoint);
    }

    public void CreateBuilding(Tile tile, TileContentType type)
    {
        if (tile.Content == null || tile.Content.ContentType == ContentType.Projection)
        {
            switch (type)
            {
                case TileContentType.Wall:
                    tile.Content = BuildingPool.Instance.GetContent(TileContentType.Wall);
                    CalculatePaths();
                    if (!CheckWall())
                    {
                        tile.Content = null;
                    }
                    break;
                case TileContentType.SpawnPoint:
                    if (tile.HasPath)
                    {
                        tile.Content = BuildingPool.Instance.GetContent(TileContentType.SpawnPoint);
                        _spawnPoints.Add(tile);
                    }
                    break;
                case TileContentType.Destination:
                    tile.Content = BuildingPool.Instance.GetContent(TileContentType.Destination);
                    _destinationPoints.Add(tile);
                    break;

            }
        }
        else
        {
            switch (tile.Content.Type)
            {
                case TileContentType.Wall:
                    tile.Content = null;
                    break;
                case TileContentType.SpawnPoint:
                    if (_spawnPoints.Count > 1)
                    {
                        tile.Content = null;
                        _spawnPoints.Remove(tile);
                        CalculatePaths();
                    }
                    return;
                case TileContentType.Destination:
                    if (_destinationPoints.Count > 1)
                    {
                        tile.Content = null;
                        _destinationPoints.Remove(tile);
                        CalculatePaths();
                    }
                    break;
            }
        }
        CalculatePaths();
    }

    public void ToggleTower(WallContent wallContent)
    {
        if (wallContent.TowerContent != null)
        {
            wallContent.TowerContent = null;
            return;
        }

        wallContent.TowerContent = TowerPool.Instance.GetContent(TowerType.Laser);
    }

    public Tile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int x = (int)(hit.point.x + _size.x * 0.5f);
            int y = (int)(hit.point.z + _size.y * 0.5f);
            if (x.CheckRange(0, _size.x, Inclusive.Minimum) && y.CheckRange(0, _size.y, Inclusive.Minimum))
            {
                return _tiles[x + y * _size.x];
            }
        }
        return null;
    }
    public Tile GetSpawnPoint(int index)
    {
        return _spawnPoints[index];
    }
    public WallContent GetWall(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //int x = (int)(hit.point.x + _size.x * 0.5f);
            //int y = (int)(hit.point.z + _size.y * 0.5f);
            //if (x.CheckRange(0, _size.x, Inclusive.Minimum) && y.CheckRange(0, _size.y, Inclusive.Minimum))
            //{
            //Debug.Log(hit.collider.gameObject.name);
            //hit.col
            return hit.collider.gameObject.GetComponentInParent<WallContent>();
            //}
        }
        return null;
    }
    private void CalculatePaths()
    {
        FindPaths();

        foreach (Tile tile in _spawnPoints)
        {
            _searchFrontier.Enqueue(tile);
        }
        while (_searchFrontier.Any())
        {
            Tile tile = _searchFrontier.Dequeue();
            if (tile != null && tile.Content?.Type != TileContentType.Destination)
            {
                foreach (var item in tile.NextTilesOnPath)
                {
                    _searchFrontier.Enqueue(item.Value);
                    if (item.Value.Content?.Type != TileContentType.Destination)
                    {
                        item.Value.ShowArrow();
                    }
                }
            }
        }
    }


    private bool FindPaths()
    {
        if (!CheckPath())
            return false;

        while (_searchFrontier.Any())
        {
            Tile tile = _searchFrontier.Dequeue();
            if (tile != null)
            {
                _searchFrontier.Enqueue(tile.GrowPathNorth());
                _searchFrontier.Enqueue(tile.GrowPathEast());
                _searchFrontier.Enqueue(tile.GrowPathSouth());
                _searchFrontier.Enqueue(tile.GrowPathWest());
            }
        }
        foreach (var tile in _tiles)
        {
            tile.FindQuicklyPaths();
        }

        return true;
    }

    private bool CheckPath()
    {
        foreach (var tile in _tiles)
        {
            if (tile.Content?.Type == TileContentType.Destination)
            {
                tile.BecomeDestination();
                _searchFrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }
        return _searchFrontier.Any();
    }
    private bool CheckWall()
    {
        foreach (Tile tile in _spawnPoints)
        {
            _searchFrontier.Enqueue(tile);
        }
        while (_searchFrontier.Any())
        {
            Tile tile = _searchFrontier.Dequeue();
            if (!tile.NextTilesOnPath.Any())
                return false;
        }
        return true;
    }
}


