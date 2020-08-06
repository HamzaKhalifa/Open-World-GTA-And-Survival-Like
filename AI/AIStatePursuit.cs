using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStatePursuit : AIState
{
    [SerializeField] private float _newDestinationDelay = 1f;
    [SerializeField] private float _boomBoxMinStoppingDistanceBonus = 3f;
    [SerializeField] private float _socialInteractionStoppingDistanceBonus = 2f;

    private float _newDestinationTimer = 0f;
    // Using this variable to determine a consensus on who has got to make the move towards the other
    private bool _shouldMakeAMove = false;

    public bool ShouldMakeAMove { set { _shouldMakeAMove = value; } }

    public override AIStateType GetStateType()
    {
        return AIStateType.Pursuit;
    }

    public override void OnEnter()
    {
        _newDestinationTimer = _newDestinationDelay;
    }

    public override AIStateType OnUpdate()
    {
        // Sometimes we are pusuing something but the target is null. We would like to still go to the last position of the target. 
        if (_stateMachine.CurrentTarget == null)
        {
            if (_stateMachine.Agent.hasPath)
            {
                if (!_stateMachine.Agent.pathPending &&
                    (_stateMachine.Agent.remainingDistance <= _stateMachine.Agent.stoppingDistance
                    || _stateMachine.Agent.isPathStale))
                    return AIStateType.Alert;
            }
            else
            {
                return AIStateType.Alert;
            }
        }

        // If we can't reach the target, we go into hurdle mode
        if (_stateMachine.Agent.isPathStale)
        {
            return AIStateType.Hurdle;
        }

        PeriodicDestination();

        float stoppingDistance = GetStoppingDistance(_stateMachine.Agent.stoppingDistance);

        if (_stateMachine.Agent.remainingDistance <= stoppingDistance
            && !_stateMachine.Agent.isPathStale)
        {
            if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.Player
                && Vector3.Distance(transform.position, _stateMachine.CurrentTarget.TargetTransform.position) <= _stateMachine.Agent.stoppingDistance)
            {
                // We go into attack mode
                return AIStateType.Attack;
            }
            if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.Sound)
            {
                // We reset the target and go into sound mode if it's a sound
                _stateMachine.ResetTarget();
                return AIStateType.Alert;
            }
            if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.BoomBox)
            {
                return AIStateType.Dancing;
            }
            if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.Friend)
            {
                return AIStateType.SocialInteraction;
            }
            if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.Vehicle)
            {
                return AIStateType.Vehicle;
            }
            if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.Chair)
            {
                // We need to check if the chair is already occupied
                AIStateSitting aiStateSitting = GetComponent<AIStateSitting>();
                InteractiveChair _interactiveChair = _stateMachine.CurrentTarget.TargetTransform.GetComponent<InteractiveChair>();
                if (aiStateSitting != null && !_interactiveChair.IsOccupied)
                    return AIStateType.Sitting;
                else {
                    _stateMachine.ResetTarget();
                    _stateMachine.Agent.ResetPath();
                }
            }
        }

        LookAtObjective();

        HandleAnimator();

        return AIStateType.Pursuit;
    }

    #region Helper Methods

    private void LookAtObjective()
    {
        // Keeping the objective in sight
        if (_stateMachine.Agent.desiredVelocity != Vector3.zero)
        {
            Quaternion nextRotation = Quaternion.LookRotation(_stateMachine.Agent.desiredVelocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, _followRotationSpeed * Time.deltaTime);
        }
    }

    private void PeriodicDestination()
    {
        // Periodically setting a new destination
        _newDestinationTimer += Time.deltaTime;
        if (_newDestinationTimer >= _newDestinationDelay)
        {
            _newDestinationTimer = 0f;
            if (_stateMachine.CurrentTarget != null)
                _stateMachine.Agent.SetDestination(_stateMachine.CurrentTarget.TargetTransform.position);
        }
    }

    private float GetStoppingDistance(float stoppingDistance)
    {
        // If it's a boom box, we stop somewhere away from it, randomly
        if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.BoomBox)
        {
            stoppingDistance = Random.Range(stoppingDistance, stoppingDistance + 3f);
        }

        // If it's a boom box, we stop somewhere away from it, randomly
        if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.Friend)
        {
            stoppingDistance = Random.Range(stoppingDistance, stoppingDistance + _socialInteractionStoppingDistanceBonus);
        }

        return stoppingDistance;
    }

    #endregion

    public override void OnExit()
    {
        _shouldMakeAMove = false;
    }

    public override void HandleAnimator()
    {
        int animatorSpeedToSet = _stateMachine.Agent.desiredVelocity.magnitude > 0 ? 1 : 0;
        if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.Friend && !_shouldMakeAMove)
            animatorSpeedToSet = 0;

        _stateMachine.Animator.SetInteger("Speed", animatorSpeedToSet);

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
