using System.Collections.Generic;
using UnityEngine;

public class AIScanner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _angryAt = new List<GameObject>();
    [SerializeField] private float _fieldOfView = 90f;
    [SerializeField] private LayerMask _seeablesMask;
    [Tooltip("Sometimes we are making a round kick or round attack, so the player is instantly out of sight. We should still have him set ast he target")]
    [SerializeField] private float _targetOutOfSightTolerance = 1f;

    #region Cache Fields

    private AIStateMachine _stateMachine = null;
    private SphereCollider _sphereCollider = null;

    #endregion

    private float _targetOutOfSightTime = 0f;

    #region Monobehavior Callbacks

    private void Start()
    {
        _stateMachine = GetComponentInParent<AIStateMachine>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (_stateMachine.Health.IsDead) return;

        bool enemyOrPlayerThreatInSight = false;

        // This is for potential threats
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (_angryAt.Count == 0) goto NotAngryAtAnyoneInSight;

            // First check if we are angry at the potential target
            GameObject potentialTargetObject = _angryAt.Find(potentialTarget => potentialTarget == other.gameObject);
            if (potentialTargetObject == null) goto NotAngryAtAnyoneInSight;

            // If we are already seeing a player or enemy and the target we just saw is a different one, then we don't do anything
            if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.TargetTransform.gameObject != other.gameObject
                && (_stateMachine.CurrentTarget.Type == AITargetType.Player || _stateMachine.CurrentTarget.Type == AITargetType.Enemy))
                return;

            // At this point, we are only interacting with either the target that we already have as a target or a new one (since we don't have a target at all)


            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, (other.transform.position + Vector3.up) - transform.position, out hitInfo, _sphereCollider.radius, _seeablesMask))
            {
                if (hitInfo.transform.gameObject == other.gameObject)
                {
                    float angle = Vector3.Angle(transform.forward, (other.transform.position + Vector3.up) - transform.position);
                    if (angle <= _fieldOfView)
                    {
                        _stateMachine.SetTarget(other.transform, AITargetType.Player);
                        enemyOrPlayerThreatInSight = true;
                    }
                }
            }

            // Resetting target for when the player or the enemy is inside the scanner collider but not inside the field of view
            // If we were seeing the player or the enemy and no longer see him, then we reset the target after the tolerance time
            if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.Player && !enemyOrPlayerThreatInSight)
            {
                _targetOutOfSightTime += Time.deltaTime;

                if (_targetOutOfSightTime >= _targetOutOfSightTolerance)
                {
                    _targetOutOfSightTime = 0f;
                    _stateMachine.ResetTarget();
                }
            }
        }

        if (enemyOrPlayerThreatInSight) return;

        NotAngryAtAnyoneInSight:

        if (other.CompareTag("AIVehicleDetector")
            && (_stateMachine.CurrentTarget == null || _stateMachine.CurrentTarget.Type != AITargetType.Vehicle)
            && _stateMachine.CurrentState.GetStateType() != AIState.AIStateType.Vehicle)
        {
            InteractiveVehicle interactiveVehicle = other.GetComponent<InteractiveVehicle>();
            if (interactiveVehicle != null && !interactiveVehicle.Occupied)
            {
                AIStateVehicle aiStateDriving = (AIStateVehicle)(_stateMachine.GetState(AIState.AIStateType.Vehicle));
                if((aiStateDriving.WantToDrive && interactiveVehicle.DriverSeat) || (aiStateDriving.WantToBePassenger && !interactiveVehicle.DriverSeat))
                {
                    _stateMachine.SetTarget(other.transform, AITargetType.Vehicle);
                    return;
                }
            }
        }

        if (other.CompareTag("Chair")
            && (_stateMachine.CurrentTarget == null || _stateMachine.CurrentTarget.Type != AITargetType.Chair)
            && _stateMachine.CurrentState.GetStateType() != AIState.AIStateType.Sitting)
        {
            InteractiveChair interactiveChair = other.GetComponent<InteractiveChair>();
            if (interactiveChair != null && !interactiveChair.IsOccupied)
            {
                AIStateSitting aiStateDriving = (AIStateSitting)(_stateMachine.GetState(AIState.AIStateType.Sitting));
                if (aiStateDriving.IsTired)
                {
                    _stateMachine.SetTarget(other.transform, AITargetType.Chair);
                    return;
                }
            }
        }

        // For social Interaction
        if (other.CompareTag("Enemy"))
        {
            // if we are already interacting socially, then there is nothing to do here
            if (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.Friend) return;

            AIStateMachine otherStateMachine = other.GetComponent<AIStateMachine>();

            AIStateSocialInteraction mySocialInteractionState = (AIStateSocialInteraction)(_stateMachine.GetState(AIState.AIStateType.SocialInteraction));
            AIStatePursuit myPursuitState = (AIStatePursuit)(_stateMachine.GetState(AIState.AIStateType.Pursuit));
            AIStateSocialInteraction friendSocialInteractionState = (AIStateSocialInteraction)(otherStateMachine.GetState(AIState.AIStateType.SocialInteraction));
            AIStatePursuit friendPursuitState = (AIStatePursuit)(otherStateMachine.GetState(AIState.AIStateType.Pursuit));
            if (mySocialInteractionState.IsFeelingLonely && friendSocialInteractionState.IsFeelingLonely
                && !otherStateMachine.Health.IsDead)
            {
                // If the friend hasn't gotten into social mode yet by setting a target of type friend, then we are the ones who are going to be making the first move
                if (otherStateMachine.CurrentTarget == null
                    || otherStateMachine.CurrentTarget.Type == AITargetType.None
                    || otherStateMachine.CurrentTarget.Type == AITargetType.NavigationPoint
                    || otherStateMachine.CurrentTarget.Type == AITargetType.BoomBox)
                {
                    myPursuitState.ShouldMakeAMove = true;
                    mySocialInteractionState.FriendSocialInteraction = friendSocialInteractionState;
                    // We set our pursuit mode by setting a target
                    _stateMachine.SetTarget(other.transform, AITargetType.Friend);

                    // the friend should instantly get into social mode
                    friendSocialInteractionState.FriendSocialInteraction = mySocialInteractionState;
                    otherStateMachine.SetTarget(transform, AITargetType.Friend);
                    otherStateMachine.SwitchState(AIState.AIStateType.SocialInteraction);
                }
            }
        }

        if (other.CompareTag("BoomBox"))
        {
            // If we have a more important target: Another AI Agent as an enemy, a friend or a player, then we ignore the boombox
            if (_stateMachine.CurrentTarget != null
                && (_stateMachine.CurrentTarget.Type == AITargetType.Player
                || _stateMachine.CurrentTarget.Type == AITargetType.Enemy
                || _stateMachine.CurrentTarget.Type == AITargetType.Friend))
                return;

            AIStateDancing aiStateDancing = (AIStateDancing)(_stateMachine.GetState(AIState.AIStateType.Dancing));
            if (aiStateDancing.IsHungry)
            {
                _stateMachine.SetTarget(other.transform, AITargetType.BoomBox);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Enemy"))
            && _stateMachine.CurrentTarget != null
            && (_stateMachine.CurrentTarget.Type == AITargetType.Player || _stateMachine.CurrentTarget.Type == AITargetType.Enemy))
        {
            _targetOutOfSightTime += Time.deltaTime;

            if (_targetOutOfSightTime >= _targetOutOfSightTolerance)
            {
                _targetOutOfSightTime = 0f;
                _stateMachine.ResetTarget();
            }
        }

        // If the player picks the boombox and takes it elsewhere, we should reset the target. Then the AIStateDancing script will divert back to idling
        if (other.CompareTag("BoomBox") && _stateMachine.CurrentState.GetStateType() == AIState.AIStateType.Dancing)
        {
            _stateMachine.ResetTarget();
        }
    }

    #endregion

    public void AddPotentialThreat(GameObject potentialThreatObject)
    {
        if (_angryAt.Contains(potentialThreatObject)) return;

        // We only add  threat when the threat isn't outside of our sphere collider view
        if (Vector3.Distance(_stateMachine.transform.position, potentialThreatObject.transform.position) > _sphereCollider.radius) return;

        // And when he is inide the field of view
        /*float angle = Vector3.Angle(transform.forward, (potentialThreatObject.transform.position + Vector3.up) - transform.position);
        if (angle > _fieldOfView)
        {
            return;
        }*/

        _angryAt.Add(potentialThreatObject);
    }
}
