using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAcrobaticAnimator : MonoBehaviour
{
    [SerializeField] private float _frontFlipJumpForce = 10f;

    #region Cache Fields

    private Player _player = null;
    private Animator _animator = null;

    #endregion

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance.InputManager.FrontFlip)
        {
            _animator.SetTrigger("FrontFlip");
        }
    }

    public void FrontFlipAnimationEvent()
    {
        if (!_player.PlayerMove.IsGrounded) return;

        // We unprepared the weapon when we jump and when we don't have the melee equipped
        if (_player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType != WeaponType.Melee)
            _player.PlayerState.UnprepareWeapon();

        _player.PlayerState.EndWallRunningOrClimbing();
        _player.PlayerMove.DoJump(_frontFlipJumpForce);
    }
}
