using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallClimbAnimator : MonoBehaviour
{
    private Player _player = null;
    private Animator _animator = null;

    public bool HasJustTriggeredClimAnimation { set { _hasJustTriggeredClimbAnimation = value; } }
    public bool IsClimbIdling { set { _isClimbIdling = value; } }

    // We need this variable becaue when we trigger the climb animation, the animator will call the coroutine after some delay, means that we can still trigger another climb animation immediately thanks to the remaining input movements values
    private bool _hasJustTriggeredClimbAnimation = false;
    private bool _isClimbIdling = false;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleClimb();
    }

    private void HandleClimb()
    {
        if (_player.PlayerState.PlayerMoveState == PlayerMoveState.WallClimbing)
        {
            if (!_hasJustTriggeredClimbAnimation && _isClimbIdling)
            {
                // If we are climbing, we handle the climb movements animations and trigger the climb
                if (GameManager.Instance.InputManager.Horizontal > 0f && _player.PlayerWallClimb.CanClimbRight)
                {
                    _animator.SetTrigger("ClimbRight");
                    _hasJustTriggeredClimbAnimation = true;
                }
                else if (GameManager.Instance.InputManager.Horizontal < 0f && _player.PlayerWallClimb.CanClimbLeft)
                {
                    _animator.SetTrigger("ClimbLeft");
                    _hasJustTriggeredClimbAnimation = true;
                }
                else if (GameManager.Instance.InputManager.Vertical > 0f && _player.PlayerWallClimb.CanClimbUp)
                {
                    _animator.SetTrigger("ClimbUp");
                    _hasJustTriggeredClimbAnimation = true;
                }
                else if (GameManager.Instance.InputManager.Vertical < 0f && _player.PlayerWallClimb.CanClimbDown)
                {
                    _animator.SetTrigger("ClimbDown");
                    _hasJustTriggeredClimbAnimation = true;
                }
            }

            // Handling climb jump
            if (GameManager.Instance.InputManager.SpaceDown)
            {
                _player.PlayerWallClimb.CancelClimb();
                _animator.SetTrigger("WallClimbUpwardsJump");
            }

            if (GameManager.Instance.InputManager.Interact && _player.PlayerWallClimb.CanMount)
            {
                _player.PlayerState.EndWallRunningOrClimbing();
                _player.PlayerWallClimb.Mount();
                _animator.SetTrigger("MountWall");
            }
        }

        _animator.SetLayerWeight(_animator.GetLayerIndex("Wall Layer"), _player.PlayerState.IsBusyWithWall ? 1 : 0);
        _animator.SetBool("IsClimbing", _player.PlayerState.PlayerMoveState == PlayerMoveState.WallClimbing);

    }

    // Gets called by playerState when the climb is clicked
    public void StartClimb()
    {
        // If we just started to climb, then we play the start climb animation
        _player.PlayerWallClimb.Climb();

        if (_player.PlayerMove.IsGrounded)
            _animator.SetTrigger("StandToClimb");
        else _animator.SetTrigger("AirToClimb");
    }

    public void ClimbUpwardsJumpAnimatorEvent()
    {
        _player.PlayerState.EndWallRunningOrClimbing();
        _player.PlayerWallClimb.DoUpwardsJump();
    }
}
