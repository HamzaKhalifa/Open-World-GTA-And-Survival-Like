using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallRunAnimator : MonoBehaviour
{
    [SerializeField] private float _wallRunJumpForce = 10f;

    private Animator _animator = null;
    private Player _player = null;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        _animator.SetBool("IsWallRunning", _player.PlayerState.PlayerMoveState == PlayerMoveState.WallRunning);
        _animator.SetBool("IsWallRunningRight", _player.PlayerWallRun.IsWallRunningRight);
        _animator.SetBool("IsWallRunningLeft", _player.PlayerWallRun.IsWallRunningLeft);

        _animator.SetLayerWeight(_animator.GetLayerIndex("Wall Layer"), _player.PlayerState.IsBusyWithWall ? 1 : 0);

        // Handling Wall Run Jump
        if (GameManager.Instance.InputManager.SpaceDown && _player.PlayerState.PlayerMoveState == PlayerMoveState.WallRunning)
        {
            _animator.SetTrigger("WallRunJump");
        }
    }

    public void WallRunJumpAnimatorEvent()
    {
        _player.PlayerState.EndWallRunningOrClimbing();
        _player.PlayerMove.DoJump(_wallRunJumpForce);
    }
}
