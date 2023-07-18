using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Board settings")]
    [SerializeField]
    private Vector2Int _boardSize;
    [SerializeField]
    private GameBoard _board;
    [Space]
    [Header("Camera settings")]
    [SerializeField]
    private CameraType _cameraType;
    [SerializeField]
    private Camera _isometricCamera;
    [SerializeField]
    private Camera _firstPersonCamera;
    private Camera _camera;
    [SerializeField]
    private Character _character;
    [SerializeField]
    private float _spawnSpeed;
    private float _spawnProgress;
    private EnemyCollection _enemies = new EnemyCollection();
    private Ray _touchRay => _camera.ScreenPointToRay(Input.mousePosition);

    private TileContentType _buildingType = TileContentType.Wall;
    private TileContentType buildingType
    {
        get { return _buildingType; }
        set
        {
            _buildingType = value;
            if (selectedTile != null)
            {
                CreateProjection(selectedTile);
            }
        }
    }

    private void Start()
    {
        switch (_cameraType)
        {
            case CameraType.Isometric:
                _camera = _isometricCamera;
                _character.gameObject.SetActive(false);
                _isometricCamera.gameObject.SetActive(true);
                break;
            case CameraType.FirstPerson:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _camera = _firstPersonCamera;
                _character.gameObject.SetActive(true);
                _isometricCamera.gameObject.SetActive(false);
                break;
        }

        //Application.targetFrameRate = 120;    
        _board.Initialize(_boardSize);
    }

    private Tile _selectedTile;
    private Tile selectedTile
    {
        get { return _selectedTile; }
        set
        {
            if (value == null || value.Lock == true)
                return;

            if (selectedTile == null || value.Identificator != selectedTile.Identificator)
            {
                if(selectedTile != null && selectedTile.Content != null && selectedTile.Content.ContentType == ContentType.Projection)
                    selectedTile.Content = null;

                _selectedTile = value;
                if (selectedTile != null)
                {
                    CreateProjection(selectedTile);
                }
            }                
        }
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
            buildingType = TileContentType.Wall;
        if (Input.GetKey(KeyCode.Alpha2))
            buildingType = TileContentType.SpawnPoint;
        if (Input.GetKey(KeyCode.Alpha3))
            buildingType = TileContentType.Destination;


        selectedTile = _board.GetTile(_touchRay);
        if (Input.GetMouseButtonDown(0))
        {
            _board.CreateBuilding(selectedTile, _buildingType);
        }


        _character.Rotate(Input.GetAxis(Axis.MouseX), Input.GetAxis(Axis.MouseY));

        _spawnProgress += _spawnSpeed * Time.deltaTime;
        while (_spawnProgress >= 1f)
        {
            _spawnProgress -= 1f;
            SpawnEnemy();
        }
        _enemies.EnemiesUpdate();
    }

    private void CreateProjection(Tile tile)
    {
        if (tile.Content == null || tile.Content.ContentType == ContentType.Projection)
        {
            switch (buildingType)
            {
                case TileContentType.Wall:
                    if (tile.Content == null || tile.Content.Type != TileContentType.WallProjection)
                        tile.Content = ProjectionPool.Instance.GetContent(TileContentType.WallProjection);
                    break;
                case TileContentType.Destination:
                    if (tile.Content == null || tile.Content.Type != TileContentType.DestinationProjection)
                        tile.Content = ProjectionPool.Instance.GetContent(TileContentType.DestinationProjection);
                    break;
                case TileContentType.SpawnPoint:
                    if (tile.Content == null || tile.Content.Type != TileContentType.SpawnPointProjection)
                        tile.Content = ProjectionPool.Instance.GetContent(TileContentType.SpawnPointProjection);
                    break;
            }
        }
    }

    private void SpawnEnemy()
    {
        Tile tile = _board.GetSpawnPoint(UnityEngine.Random.Range(0, _board.SpawnPointsCount));
        EnemyContent enemy = EnemyPool.Instance.GetContent(EnemyType.Sphere);
        enemy.SpawnOn(tile);
        _enemies.Add(enemy);
    }

    private void CreateTower()
    {
        WallContent wallContent = _board.GetWall(_touchRay);
        if (wallContent != null)
        {
            _board.ToggleTower(wallContent);
        }
    }
}

