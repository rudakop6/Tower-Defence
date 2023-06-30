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

    private Ray _touchRay => _camera.ScreenPointToRay(Input.mousePosition);

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 120;
        Cursor.visible = false;
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

