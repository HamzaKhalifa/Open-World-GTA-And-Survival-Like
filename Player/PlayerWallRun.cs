using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallRun : MonoBehaviour
{
    [SerializeField] private float _wallRunSpeed = 5f;
    [SerializeField] private float _checkDistance = 1f;

    private Player _player = null;
    private CharacterController _characterController = null;
    private PlayerWallClimb _playerWallClimb = null;

    private bool _canWallRun = false;
    private bool _isWallRunningRight = false;
    private bool _isWallRunningLeft = false;
    private bool _canApplyRunMovement = false;
    private RaycastHit _wallRunHitInfo;

    public bool CanWallRun => _canWallRun;
    public bool IsWallRunning => _isWallRunningRight || _isWallRunningLeft; 
    public bool IsWallRunningRight => _isWallRunningRight;
    public bool IsWallRunningLeft => _isWallRunningLeft;
    public bool CanApplayRunMovement { set { _canApplyRunMovement = value; } }

    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerWallClimb = GetComponentInChildren<PlayerWallClimb>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        _canWallRun = Physics.Raycast(transform.position, transform.forward, out _wallRunHitInfo, _checkDistance, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"));
        
        if (_player.PlayerState.PlayerMoveState == PlayerMoveState.WallRunning)
        {
            // To avoid stuttering and to cancel the current climb animation and go directly in wall run mode
            if (!_characterController.enabled)
            {
                _player.PlayerWallClimb.CancelClimb();
            }

            float direction = Mathf.Sign(GameManager.Instance.InputManager.Horizontal);
            _isWallRunningRight = direction > 0;
            _isWallRunningLeft = !_isWallRunningRight;

            // This bool is set by the begin run animation behavior
            if (_canApplyRunMovement)
                _characterController.Move(direction * _player.transform.right * _wallRunSpeed * Time.deltaTime);
        }
        else
        {
            _canApplyRunMovement = false;
            _isWallRunningRight = false;
            _isWallRunningLeft = false;
        }
    }

    public void BackToClimb()
    {
        _playerWallClimb.WallRunHitInfo = _wallRunHitInfo;
        _playerWallClimb.Climb(WallClimbMovement.AfterWallRun);
    }
}
