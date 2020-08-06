using System.Collections;
using ThirdPersonCamera;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private AudioClip _PrepareWeaponSound = null;
    [SerializeField] private float _normalCameraDesiredDistance = 3f;
    [SerializeField] private float _zoomCameraDesiredDistance = 1f;
    [SerializeField] private float _vehicleCameraDesiredDistance = 6f;
    [SerializeField] private float _sideOffset = .4f;
    [SerializeField] private float _changeSideDelay = .5f;

    private bool _zoomedIn = false;
    private bool _lookingRight = true;
    private bool _hasWeaponPrepared = false;
    private IEnumerator _setCameraSideCouroutine = null;

    #region CacheFields

    private CameraController _cameraController = null;

    #endregion

    #region Monobehavior Callbacks

    private void Awake()
    {
        MouseLock(true);

        _cameraController = GetComponent<CameraController>();
    }

    private void Update()
    {
        if (_cameraController.target == null)
        {
            _cameraController.target = GameManager.Instance.Player.transform;
        }

        if (GameManager.Instance.Player.PlayerState.PlayerMoveState == PlayerMoveState.BusyDressing) return;

        if (GameManager.Instance.InputManager.ChangeCameraView && GameManager.Instance.Player.PlayerState.PlayerMoveState != PlayerMoveState.BusyWithVehicle)
        {
            _zoomedIn = !_zoomedIn;
        }

        // For distance to camera
        float desiredDistance = _normalCameraDesiredDistance;
        if (GameManager.Instance.Player.PlayerVehicle.IsInsideVehicle)
        {
            desiredDistance = _vehicleCameraDesiredDistance;
        } else
        {
            desiredDistance = _zoomedIn ? _zoomCameraDesiredDistance : _normalCameraDesiredDistance;
        }

        _cameraController.desiredDistance = desiredDistance;

        // Switching zoom camera sides (right or left)
        if (GameManager.Instance.InputManager.SwitchZoomCameraRightOrLeft
            || (GameManager.Instance.Player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared && !_hasWeaponPrepared)
            || (GameManager.Instance.Player.PlayerState.PlayerWeaponState == PlayerWeaponState.Unprepared && _hasWeaponPrepared))
        {
            // If has weapon is already equal to the one stored in player state, it means we are trying to change the look side, not preparing/unpreparing the weapon
            if (_hasWeaponPrepared == (GameManager.Instance.Player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared))
            {
                _lookingRight = !_lookingRight;
            }

            _hasWeaponPrepared = GameManager.Instance.Player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared;

            // For side view
            float offsetX = 0f;
            if (GameManager.Instance.Player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared
                // Melee weapons do not cause us to go into side aim mode
                && GameManager.Instance.Player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType != WeaponType.Melee)
            {
                offsetX = _lookingRight ? _sideOffset : -_sideOffset;
            }
            else
            {
                offsetX = 0f;
            }

            if (_setCameraSideCouroutine != null)
            {
                StopCoroutine(_setCameraSideCouroutine);
            }

            _setCameraSideCouroutine = SetCameraSideView(offsetX);
            StartCoroutine(_setCameraSideCouroutine);
        }
    }

    #endregion

    private IEnumerator SetCameraSideView(float xOffset) {
        PlaySwitchCameraViewSound();

        float time = 0f;

        float initialXOffset = _cameraController.offsetVector.x;

        while (time <= _changeSideDelay)
        {
            time += Time.deltaTime;
            float normalizedTime = time / _changeSideDelay;

            _cameraController.offsetVector.x = Mathf.Lerp(initialXOffset, xOffset, normalizedTime);

            yield return null;
        }

        _cameraController.offsetVector.x = xOffset;

        _setCameraSideCouroutine = null;
    }

    private void PlaySwitchCameraViewSound()
    {
        GameManager.Instance.AudioManager.PlayOneShotSound(_PrepareWeaponSound, 1, 0, 1);
    }


    public void ZoomCamera()
    {
        _zoomedIn = true;
    }

    public void ZoomOutCamera()
    {
        _zoomedIn = false;
    }

    public void MouseLock(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
