using UnityEngine;

namespace Avaturn.Samples.Runtime._Data.Plugins.Third_Person_Controller.Scripts
{
  [RequireComponent(typeof(CharacterController))]
  [RequireComponent(typeof(PlayerInput))]
  public class ThirdPersonController : MonoBehaviour
  {
    [Header("Player")] [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")] [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Space(10)] [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)] [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")] [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")] [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition;

    public Animator Animator;

    private PlayerInput _playerInput;
    private CharacterController _controller;
    private GameObject _mainCamera;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private float _speed;
    private float _animationBlend;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private const float _threshold = 0.01f;

    private void Awake()
    {
      if (_mainCamera == null)
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {
      _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

      _controller = GetComponent<CharacterController>();
      _playerInput = GetComponent<PlayerInput>();

      AssignAnimationIDs();

      _jumpTimeoutDelta = JumpTimeout;
      _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
      _playerInput.CheckInput();

      JumpAndGravity();
      GroundedCheck();
      Move();
    }

    private void LateUpdate() =>
      CameraRotation();

    private void AssignAnimationIDs()
    {
      _animIDSpeed = Animator.StringToHash("Speed");
      _animIDGrounded = Animator.StringToHash("Grounded");
      _animIDJump = Animator.StringToHash("Jump");
      _animIDFreeFall = Animator.StringToHash("FreeFall");
      _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
      Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
        transform.position.z);

      Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
        QueryTriggerInteraction.Ignore);

      Animator.SetBool(_animIDGrounded, Grounded);
    }

    private void CameraRotation()
    {
      if (_playerInput.Look.sqrMagnitude >= _threshold && !LockCameraPosition)
      {
        float deltaTimeMultiplier = Time.deltaTime;

        _cinemachineTargetYaw += _playerInput.Look.x * deltaTimeMultiplier;
        _cinemachineTargetPitch += _playerInput.Look.y * deltaTimeMultiplier;
      }

      _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
      _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

      CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
        _cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
      float targetSpeed = _playerInput.Sprint ? SprintSpeed : MoveSpeed;

      if (_playerInput.Move == Vector2.zero)
        targetSpeed = 0.0f;

      float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

      float speedOffset = 0.1f;
      float inputMagnitude = 1f; // TODO !!!!!!

      if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
      {
        _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
        _speed = Mathf.Round(_speed * 1000f) / 1000f;
      }
      else
        _speed = targetSpeed;

      _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
      if (_animationBlend < 0.01f) _animationBlend = 0f;

      Vector3 inputDirection = new Vector3(_playerInput.Move.x, 0.0f, _playerInput.Move.y).normalized;

      if (_playerInput.Move != Vector2.zero)
      {
        _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                          _mainCamera.transform.eulerAngles.y;
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
          RotationSmoothTime);

        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
      }

      Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

      _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

      Animator.SetFloat(_animIDSpeed, _animationBlend);
      Animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }

    private void JumpAndGravity()
    {
      if (Grounded)
      {
        _fallTimeoutDelta = FallTimeout;

        Animator.SetBool(_animIDJump, false);
        Animator.SetBool(_animIDFreeFall, false);

        if (_verticalVelocity < 0.0f)
          _verticalVelocity = -2f;

        if (_playerInput.Jump && _jumpTimeoutDelta <= 0.0f)
        {
          _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

          Animator.SetBool(_animIDJump, true);
        }

        if (_jumpTimeoutDelta >= 0.0f)
          _jumpTimeoutDelta -= Time.deltaTime;
      }
      else
      {
        _jumpTimeoutDelta = JumpTimeout;

        if (_fallTimeoutDelta >= 0.0f)
          _fallTimeoutDelta -= Time.deltaTime;
        else
          Animator.SetBool(_animIDFreeFall, true);

        _playerInput.Jump = false;
      }

      if (_verticalVelocity < _terminalVelocity)
        _verticalVelocity += Gravity * Time.deltaTime;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
      if (lfAngle < -360f)
        lfAngle += 360f;

      if (lfAngle > 360f)
        lfAngle -= 360f;

      return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
      Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
      Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

      Gizmos.color = Grounded ? transparentGreen : transparentRed;
      Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }
  }
}