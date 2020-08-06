using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponMove : MonoBehaviour
{
    Player _player = null;

    private void Awake()
    {
        _player = GetComponent<Player>(); 
    }

    private void Update()
    {
        if (_player.PlayerState.PlayerWeaponState != PlayerWeaponState.Prepared
            || !_player.PlayerMove.IsGrounded
            || _player.PlayerMove.AppliedRootMotion == AppliedRootMotion.All
            || _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType == WeaponType.Melee) return;

        float speed = _player.PlayerMove.AnimatorSpeed;

        float horizontal = GameManager.Instance.InputManager.Horizontal;
        float vertical = GameManager.Instance.InputManager.Vertical;

        Vector3 desiredMovement = horizontal * _player.transform.right + vertical * _player.transform.forward;

        // The speed is calculated by the player move script (depending on which playerMoveState we are in)
        desiredMovement *= _player.PlayerMove.GetCurrentSpeed() * Time.deltaTime;

        // Still applying the gravity applied by the player move script
        desiredMovement.y = _player.PlayerMove.VSpeed;

        _player.CharacterController.Move(desiredMovement);

    }
}
