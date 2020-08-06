using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallClimbMovement {
    Begin,
    Up,
    Down,
    Right,
    Left,
    AfterWallRun
}

public class PlayerWallClimb : MonoBehaviour
{
    [SerializeField] private Transform _airClimbChecker = null;
    [SerializeField] private Transform _standClimbChecker = null;
    [SerializeField] private Transform _upClimbChecker = null;
    [SerializeField] private Transform _downClimbChecker = null;
    [SerializeField] private Transform _rightClimbChecker = null;
    [SerializeField] private Transform _leftClimbChecker = null;
    [SerializeField] private Transform _mountDestination = null;
    [SerializeField] private float _checkForwardDistance = 4f;
    [SerializeField] private float _upwardsJumpForce = 10f;

    [SerializeField] private float _climbTime = .5f;
    [SerializeField] private float _climbOffset = .5f;

    #region Cache Fields

    private Player _player = null;

    #endregion 

    private bool _canBeginClimb = false;
    private bool _potentialClimbRight = false;
    private bool _potentialClimbLeft = false;
    private bool _potentialClimbUp = false;
    private bool _potentialClimbDown = false;
    private bool _canMount = false;
    private float _maxMountableNormalYValue = .7f;


    private RaycastHit _beginClimbHitInfo;
    private RaycastHit _upClimbHitInfo;
    private RaycastHit _downClimbHitInfo;
    private RaycastHit _rightClimbHitInfo;
    private RaycastHit _leftClimbHitInfo;
    private RaycastHit _wallRunHitInfo;
    private IEnumerator _climbToPointCoroutine = null;

    public bool CanBeginClimb => _canBeginClimb;
    public bool CanClimbUp => _potentialClimbUp && _climbToPointCoroutine == null;
    public bool CanClimbDown => _potentialClimbDown && _climbToPointCoroutine == null;
    public bool CanClimbRight => _potentialClimbRight && _climbToPointCoroutine == null;
    public bool CanClimbLeft => _potentialClimbLeft && _climbToPointCoroutine == null;
    public bool PotentialClimbUp { get { return _potentialClimbUp; } }
    public bool CanMount => _canMount;
    public bool IsPlayingClimbAnimation => _climbToPointCoroutine != null;
    public RaycastHit WallRunHitInfo { set { _wallRunHitInfo = value; } }

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        // If we aren't climbing, we shoulD only check if we can climb
        // It's the playerMove script that takes the upperhand in that case
        if (_player.PlayerState.PlayerMoveState != PlayerMoveState.WallClimbing)
        {
            Transform climbChecker = _player.PlayerMove.IsGrounded ? _standClimbChecker : _airClimbChecker;
            // We first register the climb destination
            Physics.Raycast(climbChecker.position, climbChecker.transform.forward, out _beginClimbHitInfo, 1f, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"));
            // We can only begin climbing when the current climb position PLUS the difference between the up climb position and the position of the player projected on plane (height of the player when climbing) is hitting, so we don't climb on air
            // Because when we climb, the player's position (foot) is going to be translated to the climb checker and his hands are going to be on air
            _canBeginClimb = Physics.Raycast(climbChecker.position + (Vector3.ProjectOnPlane(_upClimbChecker.position, climbChecker.transform.forward) - Vector3.ProjectOnPlane(_player.transform.position, climbChecker.transform.forward)), climbChecker.transform.forward, 1f, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"));
            _canMount = false;

            return;
        }

        _potentialClimbUp = Physics.Raycast(_upClimbChecker.position, _upClimbChecker.transform.forward, out _upClimbHitInfo, _checkForwardDistance, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"));

        RaycastHit normalYCheckerHitInfo;
        _canMount = (!Physics.Raycast(_upClimbChecker.position + (Vector3.up * .2f), _upClimbChecker.transform.forward, out normalYCheckerHitInfo, _checkForwardDistance, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"))
            && _climbToPointCoroutine == null);
        // If we can't mount, there is still the possibility that we could mount something which normal is inclined. (At the top of building with an inclined roof)
        if (!_canMount && _climbToPointCoroutine == null)
        {
            _canMount = normalYCheckerHitInfo.normal.y > _maxMountableNormalYValue;
        }

        _potentialClimbDown = Physics.Raycast(_downClimbChecker.position, _downClimbChecker.transform.forward, out _downClimbHitInfo, _checkForwardDistance, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"));
        _potentialClimbRight = Physics.Raycast(_rightClimbChecker.position, _rightClimbChecker.transform.forward, out _rightClimbHitInfo, _checkForwardDistance, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"));
        _potentialClimbLeft = Physics.Raycast(_leftClimbChecker.position, _leftClimbChecker.transform.forward, out _leftClimbHitInfo, _checkForwardDistance, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"));
    }

    public void Climb(WallClimbMovement climbMovement = WallClimbMovement.Begin)
    {
        // If we are already climbing to a point, then we wait till we get to the point position
        if (_climbToPointCoroutine != null) return;

        _climbToPointCoroutine = ClimbToPointCoroutine(climbMovement);
        StartCoroutine(_climbToPointCoroutine);
    }

    public void CancelClimb()
    {
        if (_climbToPointCoroutine != null)
        {
            StopCoroutine(_climbToPointCoroutine);
            _climbToPointCoroutine = null;
            _player.CharacterController.enabled = true;
        }
    }

    public void DoUpwardsJump()
    {
        _player.PlayerMove.DoJump(_upwardsJumpForce);
    }

    public void Mount()
    {
        if (_climbToPointCoroutine != null) return;

        _climbToPointCoroutine = MountCoroutine();
        StartCoroutine(_climbToPointCoroutine);
    }

    private IEnumerator ClimbToPointCoroutine(WallClimbMovement climbMovement)
    {
        RaycastHit climbHitInfo;

        // When we start playing the climbing animation coroutine, it means that the canclim will return false for the playeranimator script.
        // It means, it's safe to resest the HasJustTriggeredClimAnimation to false. We know for a fact that the playerAnimator script won't be able to trigger another climb animation now that we have the current one playing
        _player.PlayerWallClimbAnimator.HasJustTriggeredClimAnimation = false;

        Transform climbChecker = null;

        switch(climbMovement)
        {
            case WallClimbMovement.Begin:
                climbHitInfo = _beginClimbHitInfo;
                break;
            case WallClimbMovement.Up:
                climbHitInfo = _upClimbHitInfo;
                climbChecker = _upClimbChecker;
                break;
            case WallClimbMovement.Down:
                climbHitInfo = _downClimbHitInfo;
                climbChecker = _downClimbChecker;
                break;
            case WallClimbMovement.Right:
                climbHitInfo = _rightClimbHitInfo;
                climbChecker = _rightClimbChecker;
                break;
            case WallClimbMovement.Left:
                climbHitInfo = _leftClimbHitInfo;
                climbChecker = _leftClimbChecker;
                break;
            case WallClimbMovement.AfterWallRun:
                climbHitInfo = _wallRunHitInfo;
                break;

            default:
                climbHitInfo = _beginClimbHitInfo;
                break;
        }

        _player.CharacterController.enabled = false;

        if (climbHitInfo.normal == Vector3.zero)
        {
            CancelClimb();
            yield break;
        }

        _player.transform.forward = -new Vector3(climbHitInfo.normal.x, 0, climbHitInfo.normal.z);

        float time = 0f;
        Vector3 initialPosition = _player.transform.position;

        while (time < _climbTime)
        {
            time += Time.deltaTime;
            float normalizedTime = time / _climbTime;

            bool reachedAnEnd = false;

            // If while climbing, the climb checker's raycast is no longer colliding with anything, it means we reached the end of what we are climbing, so we stop the climbing
            bool canStillClimb = true;
            if (climbChecker != null)
            {
                reachedAnEnd = !Physics.Raycast(climbChecker.position, climbChecker.transform.forward, _checkForwardDistance, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"));
                canStillClimb = !reachedAnEnd;
            }

            // If we can still climb, we need to check if the roof is inclined
            if (canStillClimb && climbChecker != null)
            {
                RaycastHit reachedEndHitInfoChecker;
                Physics.Raycast(climbChecker.position, climbChecker.transform.forward, out reachedEndHitInfoChecker, _checkForwardDistance, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"));
                // If the normal of the next hitinfo is inclined (it's an inclined roof), then we reached an end
                if (Mathf.Abs(reachedEndHitInfoChecker.normal.y) > _maxMountableNormalYValue)
                {
                    reachedAnEnd = true;
                }
            }

            // If we haven't reached an end yet
            if (!reachedAnEnd)
            {
                Vector3 nextPosition = climbHitInfo.point + (climbHitInfo.normal * _climbOffset);

                // We need to keep the player's position aligned with the hit of his upper check (that of his hands)
                // So that we don't see his hands inside the geometry when climbing
                // We don't want to apply this behavior when we are the top of a building with an inclined roof (problems)
                // We need to test if we can actually climb up so that our _upClimbHitinfo is not set to the previous one (possibly in  far away building: as in the last building we climbing)
                bool canClimbUp = Physics.Raycast(_upClimbChecker.position, _upClimbChecker.transform.forward, out _upClimbHitInfo, _checkForwardDistance, LayerMask.GetMask("Default", "Wood", "Gravel", "Metal", "Grass"));
                if (_upClimbHitInfo.normal.y <.3f && canClimbUp)
                {
                    Vector3 currentHitPointProjectedOnPlane = Vector3.ProjectOnPlane(climbHitInfo.point, _upClimbHitInfo.normal);
                    Vector3 upClimbHitPointProjectedOnPlane = Vector3.ProjectOnPlane(_upClimbHitInfo.point, _upClimbHitInfo.normal);

                    Vector3 differenceWithUpHitInfo = currentHitPointProjectedOnPlane - upClimbHitPointProjectedOnPlane;

                    Vector3 currentHitPoint = _upClimbHitInfo.point + differenceWithUpHitInfo;

                    nextPosition = currentHitPoint + (climbHitInfo.normal * _climbOffset);
                }

                _player.transform.position = Vector3.Lerp(initialPosition, nextPosition, normalizedTime);
            }

            yield return null;
        }

        _player.CharacterController.enabled = true;

        _climbToPointCoroutine = null;
    }

    private IEnumerator MountCoroutine()
    {
        // When we start playing the climbing animation coroutine, it means that the canclimb variable will return false for the playeranimator script.
        // It means, it's safe to resest the HasJustTriggeredClimAnimation to false. We know for a fact that the playerAnimator script won't be able to trigger another climb animation now that we have the current one playing
        _player.PlayerWallClimbAnimator.HasJustTriggeredClimAnimation = false;

        _player.CharacterController.enabled = false;

        float time = 0f;
        Vector3 initialPosition = _player.transform.position;

        // We need to store the final position value because the _mountDestination variable value changes as the player moves 
        Vector3 mountFinalPosition = _mountDestination.position;

        while (time < _climbTime)
        {
            time += Time.deltaTime;
            float normalizedTime = time / _climbTime;

            _player.transform.position = Vector3.Lerp(initialPosition, mountFinalPosition, normalizedTime);

            yield return null;
        }

        _player.CharacterController.enabled = true;

        _climbToPointCoroutine = null;
    }
}
