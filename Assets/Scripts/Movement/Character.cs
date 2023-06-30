using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    [SerializeField]
    private int _moveSpeed = 3;
    [SerializeField]
    private int _rotateSpeed = 100;

    [Header("Jump stats")]
    [SerializeField, Range(0, 20)]
    private float _gravityForce = 9.8f;
    [SerializeField, Range(0, 10)]
    private int _heightJump = 3;

    private float xRotation = 0f;
    private float yRotation = 0f;
    
    private CharacterController _characterController;
    private PlayerInput _playerInput;
    [HideInInspector]
    public Vector3 _moveDirection = Vector3.zero;
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = new PlayerInput();
        _playerInput.Player.Jump.performed += context => Jump();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }
    private void OnDisable()
    {
        _playerInput.Disable();
    }
    private void Update()
    {
        Vector2 direction = _playerInput.Player.Move.ReadValue<Vector2>();
        Move(transform.forward * direction.y + transform.right * direction.x);

        FreeFall();
    }

    private void FreeFall()
    {
        if (_characterController.isGrounded)
        {
            _moveDirection.y = 0f;
            if (_playerInput.Player.Jump.IsPressed())
            {
                Jump();
            }
        }
        else
        {
            _moveDirection.y -= _gravityForce * Time.deltaTime;
            _characterController.Move(_moveDirection * Time.deltaTime);
        }
    }

    private void Move(Vector3 moveDirection)
    {
        moveDirection *= _moveSpeed;
        _moveDirection.x = moveDirection.x;// * _moveSpeed;
        _moveDirection.z = moveDirection.z;// * _moveSpeed;
        _characterController.Move(_moveDirection * Time.deltaTime);
        //if(_characterController.isGrounded)
        //{
        //    _moveDirection.x = 0;//_moveDirection.x * Time.deltaTime;
        //    _moveDirection.z = 0;//_moveDirection.z * Time.deltaTime;
        //}
        
        //_moveDirection.x = 0;
        //_moveDirection.z = 0;
    }

    private void Jump()
    {
        if (_characterController.isGrounded)
        {
            _moveDirection.y = _heightJump;
            _characterController.Move(_moveDirection * Time.deltaTime);
        }
    }
    public void Rotate(float valueX, float valueY)
    {
        float mouseX = valueX * _rotateSpeed * Time.deltaTime;
        float mouseY = valueY * _rotateSpeed * Time.deltaTime;
        xRotation -= mouseX;
        yRotation -= mouseY;

        yRotation = Mathf.Clamp(yRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(yRotation, -xRotation, 0f);
    }

}
