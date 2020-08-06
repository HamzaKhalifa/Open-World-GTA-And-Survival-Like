using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateAlert : AIState
{
    [SerializeField] private float _turnDelay = 4f;
    [SerializeField] private float _stopTurningThreshold = 5f;
    [SerializeField] private float _alertDelay = 5f;

    private float _turnTimer = 0f;
    private bool _turningRight = false;
    private bool _turningLeft = false;
    private float _alertTimer = 0f;

    public override AIStateType GetStateType()
    {
        return AIStateType.Alert;
    }

    public override void OnEnter()
    {
        _turnTimer = _turnDelay;

        _alertTimer = 0f;

        // When we go into alert mode, we are gonna check whether we have a target and try to set the destination to the target
        if (_stateMachine.CurrentTarget != null && _stateMachine.Agent.enabled)
        {
            _stateMachine.Agent.SetDestination(_stateMachine.CurrentTarget.LastSeenPosition);
        }
    }

    public override AIStateType OnUpdate()
    {
        // If the target is a player, then we go into pursuit mode
        if (_stateMachine.CurrentTarget != null)
        {
            if (_stateMachine.CurrentTarget.Type == AITargetType.Player)
            {
                return AIStateType.Pursuit;
            }
        }

        // If don't have a path, we turn around in circle
        if (!_stateMachine.Agent.hasPath)
        {
            _turnTimer += Time.deltaTime;

            if (_turnTimer > _turnDelay)
            {
                _turnTimer = 0f;
                _turningRight = Random.Range(0, 2) == 1;
            }
        } else
        {
            // If the target is a navigation Point or a sound, we try to turn towards it
            float angleWithDesiredVelocity = Vector3.Angle(transform.forward, _stateMachine.Agent.desiredVelocity);
            if (angleWithDesiredVelocity < _stopTurningThreshold)
            {
                // If it's a sound or a boombox or a friend, we go into pursuit mode
                if (_stateMachine.CurrentTarget != null
                    && (_stateMachine.CurrentTarget.Type == AITargetType.Sound
                    || _stateMachine.CurrentTarget.Type == AITargetType.BoomBox
                    || _stateMachine.CurrentTarget.Type == AITargetType.Friend
                    || _stateMachine.CurrentTarget.Type == AITargetType.Chair
                    || _stateMachine.CurrentTarget.Type == AITargetType.Vehicle))
                {
                    return AIStateType.Pursuit;
                }
                else if (_stateMachine.CurrentTarget != null
                    && _stateMachine.CurrentTarget.Type == AITargetType.NavigationPoint
                    && !_stateMachine.Health.JustGotHit)
                {
                    // If it's a navigation point, we go into patrol mode
                    return AIStateType.Patrol;
                }
            }

            _turningRight = Mathf.Sign(Vector3.Cross(transform.forward, _stateMachine.Agent.desiredVelocity).y) > 0;
        }

        _turningLeft = !_turningRight;

        _alertTimer += Time.deltaTime;
        if (_alertTimer >= _alertDelay)
        {
            return AIStateType.Idle;
        }

        HandleAnimator();

        return AIStateType.Alert;
    }

    public override void OnExit()
    {
        _stateMachine.Animator.SetBool("IsTurningRight", false);
        _stateMachine.Animator.SetBool("IsTurningLeft", false);
    }

    public override void HandleAnimator()
    {
        _stateMachine.Animator.SetBool("IsTurningRight", _turningRight);
        _stateMachine.Animator.SetBool("IsTurningLeft", _turningLeft);

        bool alertLayerActivated = false;

        if (_stateMachine.CurrentTarget != null)
        {
            AITargetType targetType = _stateMachine.CurrentTarget.Type;
            if (targetType == AITargetType.BoomBox || targetType == AITargetType.Friend
                || targetType == AITargetType.NavigationPoint
                || targetType == AITargetType.None)
            {
                alertLayerActivated = false;
            }

            if (targetType == AITargetType.Enemy || targetType == AITargetType.Player || targetType == AITargetType.Sound)
            {
                alertLayerActivated = true;
            }
        }

        _stateMachine.Animator.SetBool("Alert", alertLayerActivated);
    }
}
