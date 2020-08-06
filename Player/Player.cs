using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerState))]
public class Player : Character
{
    public Transform CameraTarget = null;

    #region Cache Fields

    private CharacterController _characterController = null;
    public CharacterController CharacterController
    {
        get
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();

            return _characterController;
        }
    }

    private PlayerMoveAnimator _playerAnimator = null;
    public PlayerMoveAnimator PlayerAnimator
    {
        get
        {
            if (_playerAnimator == null)
                _playerAnimator = GetComponentInChildren<PlayerMoveAnimator>();

            return _playerAnimator;
        }
    }

    private AnimatorSounds _animatorSounds = null;
    public AnimatorSounds AnimatorSounds
    {
        get
        {
            if (_animatorSounds == null)
                _animatorSounds = GetComponentInChildren<AnimatorSounds>();

            return _animatorSounds;
        }
    }


    private PlayerState _playerState = null;
    public PlayerState PlayerState
    {
        get
        {
            if (_playerState == null)
                _playerState = GetComponent<PlayerState>();

            return _playerState;
        }
    }

    private PlayerMove _playerMove = null;
    public PlayerMove PlayerMove
    {
        get
        {
            if (_playerMove == null)
                _playerMove = GetComponent<PlayerMove>();

            return _playerMove;
        }
    }

    private PlayerLook _playerLook = null;
    public PlayerLook PlayerLook
    {
        get
        {
            if (_playerLook == null)
                _playerLook = GetComponent<PlayerLook>();

            return _playerLook;
        }
    }

    private PlayerWallClimb _playerWallClimb = null;
    public PlayerWallClimb PlayerWallClimb
    {
        get
        {
            if (_playerWallClimb == null)
                _playerWallClimb = GetComponentInChildren<PlayerWallClimb>();

            return _playerWallClimb;
        }
    }

    private PlayerWallClimbAnimator _playerWallClimbAnimator = null;
    public PlayerWallClimbAnimator PlayerWallClimbAnimator
    {
        get
        {
            if (_playerWallClimbAnimator == null)
                _playerWallClimbAnimator = GetComponentInChildren<PlayerWallClimbAnimator>();

            return _playerWallClimbAnimator;
        }
    }

    private PlayerWallRun _playerWallRun = null;
    public PlayerWallRun PlayerWallRun
    {
        get
        {
            if (_playerWallRun == null)
                _playerWallRun = GetComponentInChildren<PlayerWallRun>();

            return _playerWallRun;
        }
    }

    private PlayerVehicle _playerVehicle = null;
    public PlayerVehicle PlayerVehicle
    {
        get
        {
            if (_playerVehicle == null)
                _playerVehicle = GetComponentInChildren<PlayerVehicle>();

            return _playerVehicle;
        }
    }

    private PlayerWeapons _playerWeapons = null;
    public PlayerWeapons PlayerWeapons
    {
        get
        {
            if (_playerWeapons == null)
                _playerWeapons = GetComponentInChildren<PlayerWeapons>();

            return _playerWeapons;
        }
    }

    private PlayerWeaponFire _playerWeaponFire = null;
    public PlayerWeaponFire PlayerWeaponFire
    {
        get
        {
            if (_playerWeaponFire == null)
                _playerWeaponFire = GetComponentInChildren<PlayerWeaponFire>();

            return _playerWeaponFire;
        }
    }

    private PlayerWeaponsAnimator _playerWeaponsAnimator = null;
    public PlayerWeaponsAnimator PlayerWeaponsAnimator
    {
        get
        {
            if (_playerWeaponsAnimator == null)
                _playerWeaponsAnimator = GetComponentInChildren<PlayerWeaponsAnimator>();

            return _playerWeaponsAnimator;
        }
    }

    private PlayerClothes _playerClothes = null;
    public PlayerClothes PlayerClothes
    {
        get
        {
            if (_playerClothes == null)
                _playerClothes = GetComponentInChildren<PlayerClothes>();

            return _playerClothes;
        }
    }

    private PlayerObstacleMount _playerObstacleMount = null;
    public PlayerObstacleMount PlayerObstacleMount
    {
        get
        {
            if (_playerObstacleMount == null)
                _playerObstacleMount = GetComponentInChildren<PlayerObstacleMount>();

            return _playerObstacleMount;
        }
    }

    private PlayerPickAnimator _playerPickAnimator = null;
    public PlayerPickAnimator PlayerPickAnimator
    {
        get
        {
            if (_playerPickAnimator == null)
                _playerPickAnimator = GetComponentInChildren<PlayerPickAnimator>();

            return _playerPickAnimator;
        }
    }

    private PlayerInteractor _playerInteractor = null;
    public PlayerInteractor PlayerInteractor
    {
        get
        {
            if (_playerInteractor == null)
                _playerInteractor = GetComponentInChildren<PlayerInteractor>();

            return _playerInteractor;
        }
    }

    private PlayerStatus _playerStatus = null;
    public PlayerStatus PlayerStatus
    {
        get
        {
            if (_playerStatus == null)
                _playerStatus = GetComponentInChildren<PlayerStatus>();

            return _playerStatus;
        }
    }

    private Health _playerHealth = null;
    public Health PlayerHealth
    {
        get
        {
            if (_playerHealth == null)
                _playerHealth = GetComponentInChildren<Health>();

            return _playerHealth;
        }
    }

    private PlayerBackpackAnimator _playerBackpackAnimator = null;
    public PlayerBackpackAnimator PlayerBackpackAnimator
    {
        get
        {
            if (_playerBackpackAnimator == null)
                _playerBackpackAnimator = GetComponentInChildren<PlayerBackpackAnimator>();

            return _playerBackpackAnimator;
        }
    }

    #endregion

    #region Monobehavior Callbacks

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GameManager.Instance.Player = this;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("ZoomCamera"))
        {
            //Camera.main.GetComponentInParent<PlayerCamera>().ZoomCamera();
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.transform.CompareTag("ZoomCamera"))
        {
            //Camera.main.GetComponentInParent<PlayerCamera>().ZoomOutCamera();
        }
    }

    #endregion
}
