using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateDancing : AIState
{
    [SerializeField] [Range(0, 1)] private float _satisfaction = 1;
    [SerializeField] private float _satisfactionRecoveryRate = .1f;
    [SerializeField] private float _satisfactionDepletionRate = .01f;
    [SerializeField] private float _hungryThreshold = .3f;

    public bool IsHungry
    {
        get
        {
            return _satisfaction <= _hungryThreshold;
        }
    }

    private void Start()
    {
        _satisfaction = Random.Range(0f, 1f);
    }

    private void Update()
    {
        if (_stateMachine == null) return;

        // If we aren't dancing, we should get hungry for dancing slowly
        if (_stateMachine.CurrentState != null && _stateMachine.CurrentState.GetStateType() != AIStateType.Dancing)
        {
            _satisfaction -= Time.deltaTime * _satisfactionDepletionRate;
            _satisfaction = Mathf.Max(0, _satisfaction);
        }
    }

    public override AIStateType GetStateType()
    {
        return AIStateType.Dancing;
    }

    public override void OnEnter()
    {
        SwitchAnimation();

        _stateMachine.Animator.SetBool("IsDancing", true);
        _stateMachine.Animator.SetInteger("Speed", 0);

        _stateMachine.Animator.SetBool("Alert", true);
    }

    public override AIStateType OnUpdate()
    {
        if (_stateMachine.HasHighPriorityTarget)
        {
            return AIStateType.Alert;
        }

        if (_stateMachine.CurrentTarget == null || _stateMachine.CurrentTarget.Type != AITargetType.BoomBox) { return AIStateType.Alert; }

        // Recover satisfaction as we keep dancing
        _satisfaction += Time.deltaTime * _satisfactionRecoveryRate;
        _satisfaction = Mathf.Min(1, _satisfaction);

        if (_satisfaction == 1) {
            _stateMachine.ResetTarget();
            return AIStateType.Idle;
        };

        KeepObjectiveInSight();

        return AIStateType.Dancing;
    }

    public override void OnExit()
    {
        _stateMachine.Animator.SetBool("IsDancing", false);
    }


    public override void HandleAnimator()
    {
        
    }
}
