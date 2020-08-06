using System.Collections.Generic;
using UnityEngine;

public enum PlayerMoveState
{
    Walking,
    Running,
    Sprinting,
    NinjaSprint,
    WallClimbing,
    WallRunning,
    BusyWithVehicle,
    BusyDressing,
    InteractingWithItem
}

public enum PlayerWeaponState
{
    Unprepared,
    Prepared
}

public enum Shoes
{
    None,
    Barefoot,
    Shoes
}

public enum ModeType
{
    Normal,
    Sprint,
    Ninja,
    PreparedWeapon
}

[System.Serializable]
public class Mode
{
    public ModeType ModeType = ModeType.Normal;
    public PlayerMoveState DefaultMoveState = PlayerMoveState.Walking;
    public PlayerMoveState RushMoveState = PlayerMoveState.Running;
}

public class PlayerState : MonoBehaviour
{
    [SerializeField] private List<Mode> _modes = new List<Mode>();

    private Player _player = null;

    private PlayerMoveState _playerMoveState = PlayerMoveState.Walking;
    public PlayerMoveState PlayerMoveState { get { return _playerMoveState; } set { _playerMoveState = value; } }

    private PlayerWeaponState _playerWeaponState = PlayerWeaponState.Unprepared;
    public PlayerWeaponState PlayerWeaponState => _playerWeaponState;

    private Shoes _shoes = Shoes.Barefoot;
    public Shoes Shoes { get { return _shoes; } set { _shoes = value; } }

    private int _currentMode = 0;
    public Mode CurrentMode
    {
        get
        {
            return _modes[_currentMode];
        }
    }

    public bool IsBusyWithWall => _playerMoveState == PlayerMoveState.WallClimbing || _playerMoveState == PlayerMoveState.WallRunning;
    public bool IsMakingEffort => _playerMoveState == PlayerMoveState.Running || _playerMoveState == PlayerMoveState.Sprinting
        || _playerMoveState == PlayerMoveState.NinjaSprint || _playerMoveState == PlayerMoveState.WallClimbing
        || _playerMoveState == PlayerMoveState.WallRunning;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        // We don't do anything else if we are busy in the dressing room
        if (_playerMoveState == PlayerMoveState.BusyDressing)
        {
            return;
        }

        // We don't do anything else if we are inside a vehicle
        if (_player.PlayerVehicle.BusyWithVehicle)
        {
            _playerMoveState = PlayerMoveState.BusyWithVehicle;

            return;
        }

        // If we are wall running, we don't have to check for other states
        if (GameManager.Instance.InputManager.Rushing && IsBusyWithWall && _player.PlayerWallRun.CanWallRun && Mathf.Abs(GameManager.Instance.InputManager.Horizontal) > 0)
        {
            _playerMoveState = PlayerMoveState.WallRunning;

            return;
        }
        else
        {
            // If we aren't wall running anymore, but were in wall running mode, then we try to climb again if possible, or go back to normal mode
            if (_playerMoveState == PlayerMoveState.WallRunning && _player.PlayerWallRun.CanWallRun)
            {
                // If we stopped wall running, but we were in the wall run state and can still potentially wall run (means we have something to climb on)
                // We go back to climbing
                _player.PlayerWallRun.BackToClimb();
                _playerMoveState = PlayerMoveState.WallClimbing;
            }
            // We go back to current mode when we aren't wall running or wall climbing
            if (_playerMoveState != PlayerMoveState.WallClimbing && _playerMoveState != PlayerMoveState.InteractingWithItem)
                _playerMoveState = _modes[_currentMode].DefaultMoveState;
        }

        // We only listen to changes in move state when we aren't busy with wall and when we aren't using an item
        // And when we aren't recovering energy
        if (_playerMoveState != PlayerMoveState.WallClimbing
            && _playerMoveState != PlayerMoveState.WallRunning
            && _playerMoveState != PlayerMoveState.InteractingWithItem
            && !_player.PlayerStatus.IsRecoveringEnergy)
        {
            if (GameManager.Instance.InputManager.Rushing)
            {
                _playerMoveState = _modes[_currentMode].RushMoveState;
            }
            else
            {
                _playerMoveState = _modes[_currentMode].DefaultMoveState;
            }
        }

        // Preparing and unpreparing weapon
        if (GameManager.Instance.InputManager.PreparedOrUnprepareWeapon && !IsBusyWithWall && !_player.PlayerPickAnimator.HoldingItem)
        {
            if (_playerWeaponState != PlayerWeaponState.Prepared
                && _player.PlayerWeapons.EquippedWeaponMount != null
                && _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType != WeaponType.Hands)
            {
                _playerWeaponState = PlayerWeaponState.Prepared;
            }
            else
            {
                _playerWeaponState = PlayerWeaponState.Unprepared;
            }
        }

        // Begin Climb
        // Can't climb if we are holding an item
        // Can't climb if we are recovering energy
        if (GameManager.Instance.InputManager.WallClimb
            && !_player.PlayerPickAnimator.HoldingItem
            && !_player.PlayerStatus.IsRecoveringEnergy)
        {
            if (_playerMoveState != PlayerMoveState.WallClimbing && _player.PlayerWallClimb.CanBeginClimb)
            {
                _playerWeaponState = PlayerWeaponState.Unprepared;
                _playerMoveState = PlayerMoveState.WallClimbing;
                _player.PlayerWallClimbAnimator.StartClimb();
            }
            else
            {
                if (_playerMoveState == PlayerMoveState.WallClimbing)
                {
                    _player.AnimatorSounds.PlayJumpSound();
                    _player.PlayerAnimator.Animator.SetTrigger("StopWallClimbing");
                }
                _playerMoveState = _modes[_currentMode].DefaultMoveState;
            }
        }

        // Change Move State
        if (GameManager.Instance.InputManager.ChangeMode)
        {
            _currentMode++;
            if (_currentMode >= _modes.Count)
                _currentMode = 0;
        }

        // Forcing states when tired (recovering energy)
        if (_player.PlayerStatus.IsRecoveringEnergy)
        {
            _currentMode = 0;
            if (IsMakingEffort)
            {
                _playerMoveState = PlayerMoveState.Walking;
            }
        }
    }

    public void EndWallRunningOrClimbing()
    {
        _playerMoveState = PlayerMoveState.Walking;
    }

    public void UnprepareWeapon()
    {
        _playerWeaponState = PlayerWeaponState.Unprepared;
    }

    public bool IsSprinting()
    {
        bool isSprinting = false;

        if (_playerWeaponState == PlayerWeaponState.Unprepared)
        {
            isSprinting = (_player.PlayerState.PlayerMoveState == PlayerMoveState.Sprinting ||
            _player.PlayerState.PlayerMoveState == PlayerMoveState.NinjaSprint) && (Mathf.Abs(GameManager.Instance.InputManager.Horizontal) > .1f || Mathf.Abs(GameManager.Instance.InputManager.Vertical) > .1f);
        } else
        {
            if (_player.PlayerState.PlayerMoveState == PlayerMoveState.Sprinting ||
            _player.PlayerState.PlayerMoveState == PlayerMoveState.NinjaSprint)
            {
                if (GameManager.Instance.InputManager.Vertical > .1f && Mathf.Abs(GameManager.Instance.InputManager.Horizontal) < .1f)
                    isSprinting = true;
            }
        }

        return isSprinting;
    }
}
