using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Vector2Int _boardSize;
    [SerializeField]
    private GameBoard _board;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Character _character;
    [SerializeField]
    private float _spawnSpeed;
    private float _spawnProgress;
    private EnemyCollection _enemies = new EnemyCollection();
    private Ray _touchRay => _camera.ScreenPointToRay(Input.mousePosition);

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 120;
        //Cursor.visible = false;
        _board.Initialize(_boardSize);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.LeftAlt))
                Test();
            else
                HandleTouch();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
        }

        _character.Rotate(Input.GetAxis(Axis.MouseX), Input.GetAxis(Axis.MouseY));
        
        _spawnProgress += _spawnSpeed * Time.deltaTime;
        while(_spawnProgress >= 1f)
        {
            _spawnProgress -= 1f;
            SpawnEnemy();
        }
        _enemies.EnemiesUpdate();
    }

    private void SpawnEnemy()
    {
        Tile tile = _board.GetSpawnPoint(Random.Range(0, _board.SpawnPointsCount));
        EnemyContent enemy = EnemyPool.Instance.GetContent(EnemyType.Sphere);
        enemy.SpawnOn(tile);
        _enemies.Add(enemy);
    }

    private void HandleTouch()
    {
        Tile tile = _board.GetTile(_touchRay);
        if (tile != null)
        {
            _board.ToggleWall(tile);
        }
    }
    private void HandleAlternativeTouch()
    {
        Tile tile = _board.GetTile(_touchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                _board.ToggleDestination(tile);
            else
                _board.ToggleSpawnPoint(tile);
        }
    }

    private void Test()
    {
        WallContent wallContent = _board.GetWall(_touchRay);
        if (wallContent != null)
        {
            _board.ToggleTower(wallContent);
        }
    }
}

