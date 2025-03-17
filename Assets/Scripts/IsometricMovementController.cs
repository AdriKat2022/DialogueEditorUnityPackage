using AdriKat.Toolkit.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricMovementController : MonoBehaviour
{
    [HideInInspector]
    public int SpeedMultiplier = 100; // Modified by the floor for example to alter the player's speed

    #region Variables
    [Header("Animations")]
    [SerializeField] private GameObject _sprite;

    [Header("Speed & forces")]
    [Tooltip("Prevents speeding up if the player is inputing two different directions.")]
    [SerializeField] private bool _normalizeSpeed;
    [Range(0f, 10f)]
    [Tooltip("Units per seconds")]
    [SerializeField] private float _topSpeed = 2f;
    [Tooltip("Using this will control the velocity overriding it directly, which might not work well if you're working with other forces.")]
    [SerializeField] private bool _instantAcceleration = false;
    [ShowIf(nameof(_instantAcceleration), invert: true)]
    [SerializeField] private float _acceleration = 50f;
    [Tooltip("Using this will control the velocity overriding it directly, which might not work well if you're working with other forces.")]
    [SerializeField] private bool _instantDeceleration = false;
    [ShowIf(nameof(_instantDeceleration), invert: true)]
    [SerializeField] private float _deceleration = 50f;

    [Header("Isometric Movement")]
    [Range(0f, 90f)]
    [Tooltip("In how much degrees the axis are titled. 0 is for classic orthogonal movement, 45 is default for most isometric games.")]
    [SerializeField] private float _tiltAxisAngle = 45;
    [Range(0f, 5f)]
    [Tooltip("In how much degrees the y axis is 'squished'. 0 is for classic orthogonal movement, 45 is default for most isometric games.")]
    [SerializeField] private float _squishAxisFactor = 10;

    [Header("Input References")]
    [SerializeField] private InputActionReference _movementInputReference;

    [Header("Debug")]
    [SerializeField] private bool _debug = true;

    private Rigidbody2D _rigidbody;
    private Vector2 _currentInput;
    #endregion

    #region Gizmos Debug
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_debug) return;

        // Draw the axis tilt and squished
        Vector2 xAxis = SquishVector(TiltVector(Vector2.right, _tiltAxisAngle), _squishAxisFactor);
        Vector2 yAxis = SquishVector(TiltVector(Vector2.up, _tiltAxisAngle), _squishAxisFactor);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)xAxis);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)yAxis);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + (Vector3)xAxis, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + (Vector3)yAxis, 0.1f);
    }
#endif
    #endregion

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    #region Input Management

    private void OnEnable()
    {
        _movementInputReference.action.Enable();
        _movementInputReference.action.performed += OnInput;
        _movementInputReference.action.canceled += OnInput;
    }

    private void OnDisable()
    {
        _movementInputReference.action.Disable();
        _movementInputReference.action.performed -= OnInput;
        _movementInputReference.action.canceled -= OnInput;
    }

    private void OnInput(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            _currentInput = ctx.ReadValue<Vector2>();
        }
        else
        {
            _currentInput = Vector2.zero;
        }
    }

    #endregion

    private void FixedUpdate()
    {
        ManageMovement();
    }

    private void Update()
    {
        ManageRotationAnimation();
    }

    private void ManageMovement()
    {
        if (_currentInput != Vector2.zero)
        {
            // Normalize the input if needed
            Vector2 movementInput = _normalizeSpeed ? _currentInput.normalized : _currentInput;

            // Tilt the axis to the desired angle
            Vector2 tiltedInput = TiltVector(movementInput, _tiltAxisAngle);
            tiltedInput = SquishVector(tiltedInput, _squishAxisFactor);

            // Calculate the target velocity and acceleration
            Vector2 targetVelocity = tiltedInput * _topSpeed * (SpeedMultiplier / 100f);

            if (_instantAcceleration)
            {
                // Compute needed acceleration to reach the target velocity instantly as an impulse force
                Vector2 impulse = (targetVelocity - _rigidbody.linearVelocity) * _rigidbody.mass;
                _rigidbody.AddForce(impulse, ForceMode2D.Impulse);
            }
            else
            {
                Vector2 acceleration = (targetVelocity - _rigidbody.linearVelocity) * _acceleration;
                _rigidbody.AddForce(acceleration, ForceMode2D.Force);
            }
        }
        else
        {
            if (_instantDeceleration)
            {
                // Compute needed deceleration to reach 0 velocity instantly as an impulse force
                Vector2 impulse = -_rigidbody.linearVelocity * _rigidbody.mass;
                _rigidbody.AddForce(impulse, ForceMode2D.Impulse);
            }
            else
            {
                // Decelerate the player
                Vector2 deceleration = -_rigidbody.linearVelocity * _deceleration;
                _rigidbody.AddForce(deceleration, ForceMode2D.Force);
            }
        }
    }

    private void ManageRotationAnimation()
    {
        if (_currentInput != Vector2.zero)
        {
            float angle = Vector2.SignedAngle(Vector2.up, GetIsometricVector(_currentInput, _tiltAxisAngle, _squishAxisFactor));
            _sprite.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    static private Vector2 TiltVector(Vector2 vector, float angle)
    {
        return Quaternion.Euler(0, 0, -angle) * vector;
    }

    static private Vector2 SquishVector(Vector2 vector, float squishFactor)
    {
        return new Vector2(vector.x, vector.y * squishFactor);
    }

    static private Vector2 GetIsometricVector(Vector2 vector, float tiltAngle, float squishFactor)
    {
        return SquishVector(TiltVector(vector, tiltAngle), squishFactor);
    }
}
