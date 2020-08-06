using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AITargetType
{
    None, NavigationPoint, Sound, Player, Enemy, BoomBox, Friend, Vehicle, Chair
}

[System.Serializable]
public class AITarget
{
    public AITargetType Type = AITargetType.None;
    public Vector3 LastSeenPosition = Vector3.zero;
    public Transform TargetTransform = null;
}

[RequireComponent(typeof(AIStateIdle))]
[RequireComponent(typeof(AIStatePatrol))]
[RequireComponent(typeof(AIStateAlert))]
[RequireComponent(typeof(AIStatePursuit))]
[RequireComponent(typeof(AIStateAttack))]
[RequireComponent(typeof(AIStateDancing))]
public class AIStateMachine : Character
{
    #region Inspector Assigned Fields

    // For testing
    [SerializeField] private AITarget _target = null;

    [SerializeField] private AIScanner _aiScanner = null;

    #endregion

    #region Cache Fields

    private NavMeshAgent _agent = null;
    public NavMeshAgent Agent
    {
        get
        {
            if (_agent == null)
                _agent = GetComponent<NavMeshAgent>();

            return _agent;
        }
    }

    private Health _health = null;
    public Health Health
    {
        get
        {
            if (_health == null)
                _health = GetComponent<Health>();

            return _health;
        }
    }

    private AIVehicle _aiVehicle = null;
    public AIVehicle AIVehicle
    {
        get
        {
            if (_aiVehicle == null)
                _aiVehicle = GetComponent<AIVehicle>();

            return _aiVehicle;
        }
    }

    public AIState GetState(AIState.AIStateType stateType)
    {
        return _statesDictionary[stateType];
    }

    public AIScanner AISCanner => _aiScanner;

    #endregion

    #region Private Fields

    private AIState _currentState = null;
    private Dictionary<AIState.AIStateType, AIState> _statesDictionary = new Dictionary<AIState.AIStateType, AIState>();
    private AITarget _currentTarget = null;

    #endregion

    #region Public Accessors

    public AITarget CurrentTarget { get { return _currentTarget; } set { _currentTarget = value; } }
    public AIState CurrentState { get { return _currentState; } }
    public bool HasHighPriorityTarget => CurrentTarget != null
        && CurrentTarget.Type != AITargetType.Friend
        && CurrentTarget.Type != AITargetType.BoomBox
        && CurrentTarget.Type != AITargetType.Vehicle
        && CurrentTarget.Type != AITargetType.NavigationPoint
        && CurrentTarget.Type != AITargetType.Chair
        && CurrentTarget.Type != AITargetType.None;

    #endregion

    #region Monobehvior Callbacks

    protected override void Awake()
    {
        base.Awake();

        Health.OnDeath += () => { ResetTarget(); };

        // Getting all the states first
        AIState[] states = GetComponents<AIState>();
        foreach (AIState state in states)
        {
            _statesDictionary.Add(state.GetStateType(), state);
            state.StateMachine = this;
            state.SwitchAnimation();
        }

    }

    private void Start()
    {
        // We go into idle state mode by default
        SwitchState(AIState.AIStateType.Idle);
    }

    private void Update()
    {
        //Debug.Log(_currentState.GetStateType());
        _target = _currentTarget;

        if (_currentState == null || _health.IsDead) return;

        AIState.AIStateType nextStateType = _currentState.OnUpdate();

        if (nextStateType != _currentState.GetStateType()) SwitchState(nextStateType);
    }

    private void OnAnimatorMove()
    {
        if (_currentState == null) return;

        _currentState.OnAnimatorUpdated();
    }

    #endregion

    #region Methods

    public void SwitchState(AIState.AIStateType stateType)
    {
        if (_currentState != null) _currentState.OnExit();

        if (_statesDictionary.TryGetValue(stateType, out _currentState))
        {
            _currentState.OnEnter();
        } else
        {
            // If we don't find the state, we go into Idle.
            SwitchState(AIState.AIStateType.Idle);
        }
    }

    public void SetTarget(Transform transform, AITargetType targetType)
    {
        AITarget target = new AITarget();
        target.TargetTransform = transform;
        target.LastSeenPosition = transform.position + Vector3.up;
        target.Type = targetType;

        _currentTarget = target;
    }

    public void ResetTarget()
    {
        _currentTarget = null;
    }

    private void OnDeath()
    {
        Agent.ResetPath();
        Agent.enabled = false;
    }

    #endregion
}
