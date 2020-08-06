using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveAnimator : CharacterAnimator
{
    [SerializeField] private float _movementSmoothness = 4f;
    [SerializeField] private Vector3 _footIKOffset = Vector3.zero;
    [SerializeField] private float _heroicLandThreshold = 4f;
    [SerializeField] private GameObject _heroicLandDustPrefab = null;

    #region Cache Fields

    private Player _player = null;

    #endregion

    private float _speed = 0f;


    #region Monobehavior Callbacks

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetBool("IsGrounded", _player.PlayerMove.IsGrounded);

        HandleMovement();
        HandleFallingAndLanding();
    }

    #endregion

    private void HandleMovement()
    {
        if (_player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared && _player.PlayerMove.IsGrounded && _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType != WeaponType.Melee) return;

        float maxAnimatorSpeed = 0f;

        switch (_player.PlayerState.PlayerMoveState)
        {
            case PlayerMoveState.Walking:
                maxAnimatorSpeed = 1;
                break;
            case PlayerMoveState.Running:
                maxAnimatorSpeed = 2;
                break;
            case PlayerMoveState.Sprinting:
                maxAnimatorSpeed = 3;
                break;
            case PlayerMoveState.NinjaSprint:
                maxAnimatorSpeed = 4;
                break;
        }

        float nextSpeed = Mathf.Max(Mathf.Abs(GameManager.Instance.InputManager.Horizontal), Mathf.Abs(GameManager.Instance.InputManager.Vertical));
        nextSpeed *= maxAnimatorSpeed;
        _speed = Mathf.Lerp(_speed, nextSpeed, _movementSmoothness * Time.deltaTime);

        _speed = (float)System.Math.Round(_speed, 2);
        if (_speed == .01f) _speed = 0;
        _animator.SetFloat("Speed", _speed);

        _animator.SetBool("HeroicLand", _player.CharacterController.velocity.y < 0 && _player.CharacterController.velocity.magnitude > _heroicLandThreshold);
    }

    #region Handling Jump and air

    public void Jump()
    {
        if (_player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType != WeaponType.Melee)
            _player.PlayerState.UnprepareWeapon();

        _animator.SetTrigger("Jump");
    }

    public void JumpAnimationEvent()
    {
        _player.PlayerMove.DoJump();
    }

    private void HandleFallingAndLanding()
    {
        _animator.SetBool("IsFalling", !_player.PlayerMove.IsGrounded && _player.CharacterController.velocity.y < 0);
        _animator.SetBool("IsJumping", _player.CharacterController.velocity.y > 0);
    }

    #endregion

    #region Animator Callbacks

    protected override void OnAnimatorMove()
    {
        float animatorSpeed = _animator.deltaPosition.magnitude / Time.deltaTime;
        Vector3 animatorPosition = _animator.rootPosition;
        Quaternion animatorRotation = _animator.rootRotation;

        // Give the current speed to charactercontroller in playerMove if root motion is being applied to the current animation
        _player.PlayerMove.AnimatorSpeed = animatorSpeed;
        _player.PlayerMove.AnimatorPosition = animatorPosition;
        _player.PlayerMove.AnimatorRotation = animatorRotation;
        _player.PlayerMove.AnimatorVelocity = _animator.velocity;
    }

    /*protected override void OnAnimatorIK(int layerIndex)
    {
        Vector3 leftFootPosition = _animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
        Vector3 rightFootPosition = _animator.GetBoneTransform(HumanBodyBones.RightFoot).position;

        leftFootPosition = GetHitPoint(leftFootPosition + Vector3.up, leftFootPosition - Vector3.up * 5) + _footIKOffset;
        rightFootPosition = GetHitPoint(rightFootPosition + Vector3.up, rightFootPosition - Vector3.up * 5) + _footIKOffset;

        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, .6f);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, .6f);

        _animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPosition);
        _animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPosition);

        transform.localPosition = new Vector3(0, -(Mathf.Abs(leftFootPosition.y - rightFootPosition.y) / 2), 0);
    }*/

    #endregion

    #region Helper Methods

    private Vector3 GetHitPoint(Vector3 start, Vector3 end)
    {
        RaycastHit hitInfo;
        if (Physics.Linecast(start, end, out hitInfo))
        {
            return hitInfo.point;
        }

        return end;
    }

    #endregion

    #region Animation Events

    public void HeroicLandAnimationEvent()
    {
        GameObject tmp = Instantiate(_heroicLandDustPrefab, transform.position, Quaternion.identity);
    }

    #endregion

    #region State Machine Behaviors Methods

    public void StopPlayerBehavior()
    {
        _player.PlayerMove.CanMove = false;
    }

    public void UnStopPlayerBehavior()
    {
        _player.PlayerMove.CanMove = true;
    }

    #endregion
}
