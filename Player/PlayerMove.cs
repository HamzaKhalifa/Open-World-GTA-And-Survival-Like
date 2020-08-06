using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveStateSpeed
{
    public PlayerMoveState MoveState = PlayerMoveState.Walking;
    public float Speed = 3f;
    public float MaxJumpForce = 10f;
}

public enum AppliedRootMotion
{
    None,
    All,
    Speed,
    AllWithGravity
}

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private List<MoveStateSpeed> _moveStateSpeeds = new List<MoveStateSpeed>();
    [SerializeField] private float _gravityForce = 10f;
    [SerializeField] private float _isGroundedThreshold = .6f;
    [SerializeField] private float _jumpForceIncreaseRate = 0.1f;
    [SerializeField] private Transform _isGroundedTester = null;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _minimumJumpForce = 8f;
    [SerializeField] private float _minimumPreparedKatanaJumpForce = 14f;

    [Header("Sounds")]
    [SerializeField] private AudioClip _jumpSound = null;

    #region Cache Fields

    private Player _player = null;

    #endregion

    private AppliedRootMotion _appliedRootMotion = AppliedRootMotion.None;
    private Dictionary<PlayerMoveState, float> _moveStateSpeedsDic = new Dictionary<PlayerMoveState, float>();
    private Dictionary<PlayerMoveState, float> _moveStateJumpForcesDic = new Dictionary<PlayerMoveState, float>();
    private float _vSpeed = 0f;
    private bool _hasJustJumped = false;
    private IEnumerator _jumpCoroutine = null;
    private bool _isGrounded = false;
    private float _jumpForce = 0f;
    private bool _isChargingJump = false;
    private bool _canMove = true;
    private float _animatorSpeed = 0f;
    private RaycastHit _groundHitInfo;

    #region Root Motion Fields

    private Vector3 _animatorPosition = Vector3.zero;
    private Quaternion _animatorRotation = Quaternion.identity;
    private Vector3 _animatorVelocity = Vector3.zero;
    private float _initialIsGroundedThreshold = 0;

    public float AnimatorSpeed { get { return _animatorSpeed; } set { _animatorSpeed = value; } }
    public Vector3 AnimatorPosition { set { _animatorPosition = value; } }
    public Quaternion AnimatorRotation { set { _animatorRotation = value; } }
    public Vector3 AnimatorVelocity { set { _animatorVelocity = value; } }
    public float IsGroundedThreshold { set { _isGroundedThreshold = value; } }

    #endregion

    public bool IsGrounded => _isGrounded;
    public bool CanMove { set { _canMove = value; } }
    public AppliedRootMotion AppliedRootMotion { get { return _appliedRootMotion; } set { _appliedRootMotion = value; } }
    public float VSpeed => _vSpeed;
    public float InitialIsGroundedThreshold => _initialIsGroundedThreshold;

    private void Awake()
    {
        _player = GetComponent<Player>();

        _initialIsGroundedThreshold = _isGroundedThreshold;

        foreach (MoveStateSpeed moveState in _moveStateSpeeds)
        {
            _moveStateSpeedsDic.Add(moveState.MoveState, moveState.Speed);
        }

        foreach (MoveStateSpeed moveState in _moveStateSpeeds)
        {
            _moveStateJumpForcesDic.Add(moveState.MoveState, moveState.MaxJumpForce);
        }
    }

    private void Update()
    {
        // If we are busy in the dressing room, there is nothing to do here
        if (_player.PlayerState.PlayerMoveState == PlayerMoveState.BusyDressing) return;

        // If we are busy with a vehicle, there is nothing to do here
        if (_player.PlayerState.PlayerMoveState == PlayerMoveState.BusyWithVehicle) return;

        // We check our own isGrounded instead of that of character controller so that we could still play footsteps sound when we are close to the ground
        // Also so that the land animations get played and become smooth
        _isGrounded = IsGroundedMethod();

        // If the player is climbing or wall running, we give the upperhand to the playerClimb script or the player wall script and don't do anything here apart from putting the vertical speed to 0 to avoid the player from falling rapidly when stopping from climbing
        if (_player.PlayerState.IsBusyWithWall)
        {
            _vSpeed = 0f;
            return;
        }

        float speed = GetCurrentSpeed();

        // If we are applying root motion, there is absolutely nothing we can do in player move, the animator gets the entire upperhand
        if (_appliedRootMotion == AppliedRootMotion.All || _appliedRootMotion == AppliedRootMotion.AllWithGravity)
        {
            transform.rotation = _animatorRotation;

            Vector3 velocity = _animatorVelocity;
            if (_appliedRootMotion == AppliedRootMotion.AllWithGravity)
            {
                velocity += new Vector3(0, -_gravityForce, 0);
            }
            _player.CharacterController.Move(velocity * Time.deltaTime);
            //transform.position = _animatorPosition;

            return;
        }

        Vector3 desiredMovement = Vector3.zero;
        if (_canMove)
            desiredMovement = _player.PlayerLook.DirectionVector * speed;

        if (_isGrounded)
            desiredMovement = Vector3.ProjectOnPlane(desiredMovement, _groundHitInfo.normal);

        desiredMovement = HandleJumpAndGravity(desiredMovement);


        // If we have a weapon prepared, and the weapon isn't of type melee we aren't going to be moving via this script
        if (_player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared && IsGrounded && _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType != WeaponType.Melee) return;

        if (_player.CharacterController.enabled)
            _player.CharacterController.Move(desiredMovement * Time.deltaTime);
    }

    private Vector3 HandleJumpAndGravity(Vector3 desiredMovement)
    {
        // If we are grounded, we apply the gravity force only by setting the vspeed to 0
        // We should have a left over jump force from the previous after we go out of climbing
        // So we set the vspeed = 0 if we are climbing
        if (_player.CharacterController.isGrounded && !_hasJustJumped)
        {
            _vSpeed = 0f;
        }

        // If we have a weapon prepared, we can't jump
        if (GameManager.Instance.InputManager.SpaceDown)
        {
            _isChargingJump = true;
        }

        if (_isChargingJump)
        {
            _jumpForce = Mathf.Min(_moveStateJumpForcesDic[_player.PlayerState.PlayerMoveState], _jumpForce + _jumpForceIncreaseRate * Time.deltaTime);
        }

        if (GameManager.Instance.InputManager.SpaceUp)
        {
            if (!_isGrounded)
            {
                _jumpForce = 0;
            }
            else
            {
                // When we jump, we unprepare the weapon in the jump animator function
                _player.PlayerAnimator.Jump();
            }

            _isChargingJump = false;
        }

        _vSpeed -= _gravityForce * Time.deltaTime;

        desiredMovement.y = _vSpeed;

        return desiredMovement;
    }

    public float GetCurrentSpeed()
    {
        float speed = 0f;

        _moveStateSpeedsDic.TryGetValue(_player.PlayerState.PlayerMoveState, out speed);

        // We apply the animator speed
        if (_appliedRootMotion == AppliedRootMotion.Speed)
            speed = _animatorSpeed;

        return speed;
    }

    public bool IsGroundedMethod()
    {
        _isGrounded = Physics.Raycast(_isGroundedTester.position, Vector3.down, out _groundHitInfo, _isGroundedThreshold, _groundLayerMask);
        // We are only grounded when the y velocity is inferior or equal to an epsilon (we should test as grounded when we just jumped: cases of lag)
        //_isGrounded = _isGrounded && _player.CharacterController.velocity.y <= 0.1f;

        return _isGrounded;
    }

    public void DoJump(float jumpForce = 0f)
    {
        GameManager.Instance.AudioManager.PlayOneShotSound(_jumpSound, 1, 0, 1);

        _hasJustJumped = true;

        float minimumJumpForce = _minimumJumpForce;
        if (_player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared && _player.PlayerWeapons.EquippedWeaponMount.Weapon.name == "Katana")
        {
            minimumJumpForce = _minimumPreparedKatanaJumpForce;
        }

        if (jumpForce.Equals(0))
            _vSpeed = Mathf.Max(minimumJumpForce, _jumpForce);
        else
            _vSpeed = jumpForce;

        _jumpForce = 0;

        if (_jumpCoroutine != null)
            StopCoroutine(_jumpCoroutine);

        _jumpCoroutine = JumpCoroutine();
        StartCoroutine(_jumpCoroutine);
    }

    /// <summary>
    /// IMPORTANT. This is because we are calling the jump from the animator,
    /// the vSpeed in the update function will reset our jumpforce to zero
    /// if you don't tell it to not do that when we just jumped
    /// </summary>
    /// <returns></returns>
    private IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(.5f);

        _hasJustJumped = false;

        _jumpCoroutine = null;
    }
}
