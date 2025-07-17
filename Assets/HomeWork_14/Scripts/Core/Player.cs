using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Rigidbody _player;
    [SerializeField] private float _playerHeight = 1.6f;
    [Header("Look")]
    [SerializeField] private float _sensitivity = 15f;
    [SerializeField] private Vector2 _pitchBounds = new Vector2(-80f, 80f);
    [Header("Move")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _sprintSpeed = 12f;
    [Header("Jump")]
    [SerializeField] private float _grondCheckDistance = 1f;
    [SerializeField] private float _jumpPower = 10f;
    [Header("Effectts")]
    [SerializeField] private ParticleSystem _landingParticles;

    private GameInput _gameInput;
    private GameInput.PlayerMapActions _actions;

    private Camera _mainCamera;
    private float _cameraPitch;
    
    private Vector2 _moveInput;
    private bool _isSprint;

    private bool _wasGrounded = true;
    private float _lastFallVelocity;

    private void Awake()
    {
        _gameInput = new GameInput();
        _actions = _gameInput.PlayerMap;

        _actions.Walk.performed += OnMove;
        _actions.Jump.performed += OnJump;
        _actions.Look.performed += OnLook;
        _actions.Sprint.performed += OnSprint;

        _mainCamera = Camera.main;

        if (_mainCamera != null)
        {
            _mainCamera.transform.SetParent(transform);
            _mainCamera.transform.localPosition = new Vector3(0, _playerHeight, 0);
            _mainCamera.transform.localRotation = Quaternion.identity;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        var absoluteDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
        var relativeDirection = _mainCamera.transform.TransformDirection(absoluteDirection);
        var moveSpeed = _isSprint ? _sprintSpeed : _moveSpeed;
        var move = relativeDirection.normalized * moveSpeed * Time.deltaTime;

        if(Physics.Raycast(_player.position + Vector3.up * _playerHeight * 0.4f, move.normalized, 0.6f))
        {
            move = Vector3.zero;
        }

        _player.linearVelocity = new Vector3(move.x, _player.linearVelocity.y, move.z);


        bool isGrounded = Physics.Raycast(_player.transform.position, Vector3.down, 0.2f);

        if (!isGrounded)
        {
            _lastFallVelocity = -_player.linearVelocity.y;
        }

        if (!_wasGrounded && isGrounded)
        {

            _landingParticles.Play();
            float fallVelocity = _lastFallVelocity;

            Debug.Log(fallVelocity);

            if (fallVelocity >= 1f && _landingParticles != null)
            {
                float t = Mathf.InverseLerp(2.5f, 20f, fallVelocity);

                var emission = _landingParticles.emission;
                var burst = emission.GetBurst(0);

                burst.count = Mathf.Lerp(25f, 100f, t);
                emission.SetBurst(0, burst);

                var main = _landingParticles.main;
                main.startSize = new ParticleSystem.MinMaxCurve(
                    Mathf.Lerp(0.02f, 0.15f, t),
                    Mathf.Lerp(0.02f, 0.25f, t)
                );

                main.startSpeed = Mathf.Lerp(3.5f, 8f, t);

                _landingParticles.Play();
            }
        }

        _wasGrounded = isGrounded;

    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        _isSprint = context.ReadValue<float>() > 0f;
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        var delta = context.ReadValue<Vector2>();
        var mouseX = delta.x * _sensitivity * Time.deltaTime;
        var mouseY = delta.y * _sensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, _pitchBounds.x, _pitchBounds.y);

        _mainCamera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if(Physics.Raycast(_player.transform.position, Vector3.down, _grondCheckDistance))
        {
            _player.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        _gameInput.Enable();
    }

    private void OnDisable()
    {
        _gameInput.Disable();
    }

   
}
