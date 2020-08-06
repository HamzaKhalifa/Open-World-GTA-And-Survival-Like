using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float _lookSmoothness = 10f;

    private Player _player = null;
    private Camera _mainCamera = null;

    private Vector3 _directionVector = Vector3.zero;
    public Vector3 DirectionVector => _directionVector;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Look();
    }

    private void Look()
    {
        // We shouldn't be able to look around when we are climbing
        // Or when we are driving
        if (_player.PlayerState.IsBusyWithWall
            || _player.PlayerState.PlayerMoveState == PlayerMoveState.BusyWithVehicle
            || _player.PlayerState.PlayerMoveState == PlayerMoveState.InteractingWithItem
            || _player.PlayerMove.AppliedRootMotion == AppliedRootMotion.All) return;

        // If we have a weapon prepared, and the weapon is not of type melee, then we are going to have a different looking behavior
        if (_player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared && _player.PlayerMove.IsGrounded && _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType != WeaponType.Melee) return;

        float horizontal = GameManager.Instance.InputManager.Horizontal;
        float vertical = GameManager.Instance.InputManager.Vertical;

        Vector3 projectedCameraFrontDirection = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
        Vector3 projectedCameraSideDirection = Vector3.ProjectOnPlane(_mainCamera.transform.right, Vector3.up).normalized;

        _directionVector = (vertical * projectedCameraFrontDirection + horizontal * projectedCameraSideDirection).normalized;

        if (_directionVector != Vector3.zero)
        {
            Quaternion nextRotation = Quaternion.LookRotation(_directionVector);
            transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, _lookSmoothness * Time.deltaTime);
        }
    }
}
