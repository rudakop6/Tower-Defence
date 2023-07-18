using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Tile : MonoBehaviour
{
    [SerializeField]
    private GameObject _backlight;
    private Renderer _backlightMaterial;

    private Tile _north, _south, _west, _east;
    public Dictionary<WorldSides, Tile> NextTilesOnPath { get; private set; } = new();
    private int _distance;
    public bool HasPath => _distance != int.MaxValue;
    private TileContent _content;
    public int Identificator { get; set; }

    public bool Lock = false;
    public TileContent Content
    {
        get => _content;
        set
        {
            _content?.Recycle();
            _content = value;
            if (_content == null)
                return;

            if (Content.ContentType == ContentType.Building)
                Arrow = null;

            _content.transform.localPosition = Helper.GetPositionChildren(transform, _content);
        }
    }

    private ArrowContent _arrow;
    public ArrowContent Arrow
    {
        get => _arrow;
        set
        {
            _arrow?.Recycle();
            _arrow = value;
        }
    }

    private void Awake()
    {
        _backlightMaterial = _backlight.GetComponent<Renderer>();
        _backlightMaterial.material.SetColor("_Color", Color.red);
        _backlightMaterial.gameObject.SetActive(false);
    }

    public static void CreateNorthSouthLinks(Tile north, Tile south)
    {
        north._south = south;
        south._north = north;
    }
    public static void CreateEastWestLinks(Tile east, Tile west)
    {
        east._west = west;
        west._east = east;
    }

    public void ClearPath()
    {
        _distance = int.MaxValue;
        NextTilesOnPath.Clear();
    }
    public void BecomeDestination()
    {
        _distance = 0;
        NextTilesOnPath.Clear();
    }

    public void FindQuicklyPaths()
    {
        Arrow = null;
        if (_distance > 0 && Content?.Type != TileContentType.Wall)
        {
            Dictionary<WorldSides, Tile> neighbors = new();
            int minDistance;

            if (CheckNeighbor(_north))
                neighbors.Add(WorldSides.North, _north);
            if (CheckNeighbor(_east))
                neighbors.Add(WorldSides.East, _east);
            if (CheckNeighbor(_south))
                neighbors.Add(WorldSides.South, _south);
            if (CheckNeighbor(_west))
                neighbors.Add(WorldSides.West, _west);

            if (neighbors.Any())
            {
                minDistance = neighbors.Values.Min(c => c._distance);
                foreach (var item in neighbors)
                {
                    if (item.Value._distance == minDistance)
                        NextTilesOnPath.Add(item.Key, item.Value);
                }
            }
        }

        bool CheckNeighbor(Tile neighbor)
        {
            return neighbor != null && neighbor.HasPath &&
                neighbor.Content?.Type != TileContentType.Wall &&
                neighbor.Content?.Type != TileContentType.SpawnPoint;
        }
    }

    public void ShowArrow()
    {
        Quaternion Rotate0 = Quaternion.Euler(90, 0, 0);
        Quaternion Rotate90 = Quaternion.Euler(90, 90, 0);
        Quaternion Rotate180 = Quaternion.Euler(90, 180, 0);
        Quaternion Rotate270 = Quaternion.Euler(90, 270, 0);

        switch (NextTilesOnPath.Count)
        {
            case 0:
                Arrow = null;
                return;
            case 1:
                Arrow = ArrowPool.Instance.GetContent(ArrowType.North);
                switch (NextTilesOnPath.First().Key)
                {
                    case WorldSides.North:
                        Arrow.transform.localRotation = Rotate0;
                        break;
                    case WorldSides.East:
                        Arrow.transform.localRotation = Rotate90;
                        break;
                    case WorldSides.South:
                        Arrow.transform.localRotation = Rotate180;
                        break;
                    case WorldSides.West:
                        Arrow.transform.localRotation = Rotate270;
                        break;
                }
                break;
            case 2:
                if (NextTilesOnPath.ContainsKey(WorldSides.North))
                {
                    if (NextTilesOnPath.ContainsKey(WorldSides.East))
                    {
                        Arrow = ArrowPool.Instance.GetContent(ArrowType.NorthEast);
                        Arrow.transform.localRotation = Rotate0;
                    }
                    else if (NextTilesOnPath.ContainsKey(WorldSides.South))
                    {
                        Arrow = ArrowPool.Instance.GetContent(ArrowType.WestEast);
                        Arrow.transform.localRotation = Rotate90;
                    }
                    else if (NextTilesOnPath.ContainsKey(WorldSides.West))
                    {
                        Arrow = ArrowPool.Instance.GetContent(ArrowType.NorthEast);
                        Arrow.transform.localRotation = Rotate270;
                    }
                }
                else if (NextTilesOnPath.ContainsKey(WorldSides.East))
                {
                    if (NextTilesOnPath.ContainsKey(WorldSides.South))
                    {
                        Arrow = ArrowPool.Instance.GetContent(ArrowType.NorthEast);
                        Arrow.transform.localRotation = Rotate90;
                    }
                    else if (NextTilesOnPath.ContainsKey(WorldSides.West))
                    {
                        Arrow = ArrowPool.Instance.GetContent(ArrowType.WestEast);
                        Arrow.transform.localRotation = Rotate0;
                    }
                }
                else if (NextTilesOnPath.ContainsKey(WorldSides.South) && NextTilesOnPath.ContainsKey(WorldSides.West))
                {
                    Arrow = ArrowPool.Instance.GetContent(ArrowType.NorthEast);
                    Arrow.transform.localRotation = Rotate180;
                }
                break;
            case 3:
                Arrow = ArrowPool.Instance.GetContent(ArrowType.WestNorthEast);
                if (!NextTilesOnPath.ContainsKey(WorldSides.North))
                    Arrow.transform.localRotation = Rotate180;
                else if (!NextTilesOnPath.ContainsKey(WorldSides.East))
                    Arrow.transform.localRotation = Rotate270;
                else if (!NextTilesOnPath.ContainsKey(WorldSides.South))
                    Arrow.transform.localRotation = Rotate0;
                else if (!NextTilesOnPath.ContainsKey(WorldSides.West))
                    Arrow.transform.localRotation = Rotate90;
                break;
            case 4:
                Arrow = ArrowPool.Instance.GetContent(ArrowType.AllDirection);
                Arrow.transform.localRotation = Rotate0;
                break;
        }
        Vector3 position = transform.localPosition;
        Arrow.transform.localPosition = new Vector3(
            position.x,
            position.y + Constants.OffsetArrowY,
            position.z) + Constants.TransformTileOffset;
    }

    public Tile GrowPathNorth() => GrowPathTo(_north);
    public Tile GrowPathSouth() => GrowPathTo(_south);
    public Tile GrowPathEast() => GrowPathTo(_east);
    public Tile GrowPathWest() => GrowPathTo(_west);
    private Tile GrowPathTo(Tile neighbor)
    {
        if (!HasPath || neighbor == null || neighbor.HasPath)
            return null;

        neighbor._distance = _distance + 1;

        return neighbor.Content?.Type == TileContentType.Wall ||
            neighbor.Content?.Type == TileContentType.SpawnPoint ? null : neighbor;
    }

    
}
