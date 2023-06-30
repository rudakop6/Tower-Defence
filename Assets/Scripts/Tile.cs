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
    private GameBoard _board;
    private Renderer _backlightMaterial;
    private MeshFilter _meshFilter;

    private Tile _north, _south, _west, _east;
    //public int CounterNext = 0;
    public Dictionary<WorldSids, Tile> NextTilesOnPath { get; private set; } = new();
    private int _distance;
    public bool HasPath => _distance != int.MaxValue;
    private bool _visibleArrow = false;
    public bool VisibleArrow
    {
        get => _visibleArrow;
        set
        {
            _visibleArrow = value;
            if (_visibleArrow)
                ShowArrow();
            else
                ArrowContent = ArrowPool.Instance.GetContent(ArrowType.Empty);
        }
    }
    private TileContent _content;
    public TileContent Content
    {
        get => _content;
        set
        {
            if (_content != null)
                _content.Recycle();

            _content = value;
            if (_content is WallContent)
                (_content as WallContent).TowerContent = TowerPool.Instance.GetContent(TowerType.Empty);

            if (_content.Type != TileContentType.Empty)
            {
                _content.transform.localPosition = Helper.GetPositionChildren(transform, _content);
            }          
        }
    }

    private ArrowContent _arrowContent;
    public ArrowContent ArrowContent
    {
        get => _arrowContent;
        set
        {
            if (_arrowContent != null)
                _arrowContent.Recycle();

            _arrowContent = value;
            Vector3 position = transform.localPosition;
            _arrowContent.transform.localPosition = new Vector3(position.x, position.y + Constants.OffsetArrowY, position.z) + Constants.transformTileOffset;
        }
    }

    private void Awake()
    {
        //_board = transform.parent.gameObject.GetComponent<GameBoard>();
        _backlightMaterial = _backlight.GetComponent<Renderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _backlightMaterial.material.SetColor("_Color", Color.red);
        _backlightMaterial.gameObject.SetActive(false);

    }

    private void OnMouseEnter()
    {
        _backlightMaterial.gameObject.SetActive(true);
        //_backlightMaterial.material.SetColor("_Color", Color.green);
    }
    private void OnMouseExit()
    {
        _backlightMaterial.gameObject.SetActive(false);
        //_backlightMaterial.material.SetColor("_Color", Color.red);
    }

    //private void OnMouseUp()
    //{
    //    _board.ToggleWall(this);
    //}

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
        if (_distance > 0 && Content.Type != TileContentType.Wall)
        {
            Dictionary<WorldSids, Tile> neighbors = new();
            int minDistance;

            if (CheckNeighbor(_north))
                neighbors.Add(WorldSids.North, _north);
            if (CheckNeighbor(_east))
                neighbors.Add(WorldSids.East, _east);
            if (CheckNeighbor(_south))
                neighbors.Add(WorldSids.South, _south);
            if (CheckNeighbor(_west))
                neighbors.Add(WorldSids.West, _west);

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
        //CounterNext = NextTilesOnPath.Count;

        bool CheckNeighbor(Tile neighbor)
        {
            return neighbor != null && neighbor.HasPath &&
                neighbor.Content.Type != TileContentType.Wall &&
                neighbor.Content.Type != TileContentType.SpawnPoint;
        }
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

        return neighbor.Content.Type == TileContentType.Wall ||
            neighbor.Content.Type == TileContentType.SpawnPoint ? null : neighbor;
    }

    private void ShowArrow()
    {
        Quaternion Rotate0 = Quaternion.Euler(90, 0, 0);
        Quaternion Rotate90 = Quaternion.Euler(90, 90, 0);
        Quaternion Rotate180 = Quaternion.Euler(90, 180, 0);
        Quaternion Rotate270 = Quaternion.Euler(90, 270, 0);
        switch (NextTilesOnPath.Count)
        {
            case 0:
                ArrowContent = ArrowPool.Instance.GetContent(ArrowType.Empty);
                ArrowContent.transform.localRotation = Rotate0;
                break;
            case 1:
                ArrowContent = ArrowPool.Instance.GetContent(ArrowType.North);
                switch (NextTilesOnPath.First().Key)
                {
                    case WorldSids.North:
                        ArrowContent.transform.localRotation = Rotate0;
                        break;
                    case WorldSids.East:
                        ArrowContent.transform.localRotation = Rotate90;
                        break;
                    case WorldSids.South:
                        ArrowContent.transform.localRotation = Rotate180;
                        break;
                    case WorldSids.West:
                        ArrowContent.transform.localRotation = Rotate270;
                        break;
                }
                break;
            case 2:
                if (NextTilesOnPath.ContainsKey(WorldSids.North))
                {
                    if (NextTilesOnPath.ContainsKey(WorldSids.East))
                    {
                        ArrowContent = ArrowPool.Instance.GetContent(ArrowType.NorthEast);
                        ArrowContent.transform.localRotation = Rotate0;
                    }
                    else if (NextTilesOnPath.ContainsKey(WorldSids.South))
                    {
                        ArrowContent = ArrowPool.Instance.GetContent(ArrowType.WestEast);
                        ArrowContent.transform.localRotation = Rotate90;
                    }
                    else if (NextTilesOnPath.ContainsKey(WorldSids.West))
                    {
                        ArrowContent = ArrowPool.Instance.GetContent(ArrowType.NorthEast);
                        ArrowContent.transform.localRotation = Rotate270;
                    }
                }
                else if (NextTilesOnPath.ContainsKey(WorldSids.East))
                {
                    if (NextTilesOnPath.ContainsKey(WorldSids.South))
                    {
                        ArrowContent = ArrowPool.Instance.GetContent(ArrowType.NorthEast);
                        ArrowContent.transform.localRotation = Rotate90;
                    }
                    else if (NextTilesOnPath.ContainsKey(WorldSids.West))
                    {
                        ArrowContent = ArrowPool.Instance.GetContent(ArrowType.WestEast);
                        ArrowContent.transform.localRotation = Rotate0;
                    }
                }
                else if (NextTilesOnPath.ContainsKey(WorldSids.South) && NextTilesOnPath.ContainsKey(WorldSids.West))
                {
                    ArrowContent = ArrowPool.Instance.GetContent(ArrowType.NorthEast);
                    ArrowContent.transform.localRotation = Rotate180;
                }
                break;
            case 3:
                ArrowContent = ArrowPool.Instance.GetContent(ArrowType.WestNorthEast);
                if (!NextTilesOnPath.ContainsKey(WorldSids.North))
                    ArrowContent.transform.localRotation = Rotate180;
                else if (!NextTilesOnPath.ContainsKey(WorldSids.East))
                    ArrowContent.transform.localRotation = Rotate270;
                else if (!NextTilesOnPath.ContainsKey(WorldSids.South))
                    ArrowContent.transform.localRotation = Rotate0;
                else if (!NextTilesOnPath.ContainsKey(WorldSids.West))
                    ArrowContent.transform.localRotation = Rotate90;
                break;
            case 4:
                ArrowContent = ArrowPool.Instance.GetContent(ArrowType.AllDirection);
                ArrowContent.transform.localRotation = Rotate0;
                break;
        }
    }
}
