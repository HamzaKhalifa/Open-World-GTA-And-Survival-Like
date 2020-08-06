using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateAttack : AIState
{
    [SerializeField] private List<Collider> _attacks = new List<Collider>();
    [SerializeField] private List<string> _attacksParameters = new List<string>();
    [SerializeField] private float _timeBetweenAttacks = 1f;

    private float _nextAttackTime = 0f;

    private void Start()
    {
        _stateMachine.Health.OnDeath += () => { DeactivateAllAttacks(); };
    }

    public override AIStateType GetStateType()
    {
        return AIStateType.Attack;
    }

    public override void OnEnter()
    {
        // We stop moving when we get into attack mode
        _stateMachine.Agent.ResetPath();
        _stateMachine.Agent.velocity = Vector3.zero;

        _stateMachine.Animator.SetInteger("Speed", 0);

        _stateMachine.Animator.SetBool("Alert", true);
    }

    public override AIStateType OnUpdate()
    {
        // If the target disappears, we go into alert mode
        if (_stateMachine.CurrentTarget == null) return AIStateType.Alert;

        if (_stateMachine.Agent.isPathStale)
        {
            return AIStateType.Hurdle;
        }

        // If the target gets away, we go into pursuit mode
        if (Vector3.Distance(transform.position, _stateMachine.CurrentTarget.TargetTransform.position) > _stateMachine.Agent.stoppingDistance)
        {
            return AIStateType.Pursuit;
        }

        KeepObjectiveInSight();

        if (Time.time >= _nextAttackTime)
        {
            _stateMachine.Animator.SetTrigger(_attacksParameters[Random.Range(0, _attacksParameters.Count)]);
            _nextAttackTime = Time.time + _timeBetweenAttacks;
        }

        HandleAnimator();

        return AIStateType.Attack;
    }

    public override void OnExit()
    {
        _stateMachine.Animator.SetBool("IsAttacking", false);
        foreach(string attackParameter in _attacksParameters)
        {
            _stateMachine.Animator.ResetTrigger(attackParameter);
        }
    }

    public override void HandleAnimator()
    {
    }

    public void AttackAnimationEvent(int attackIndex)
    {
        if (_attacks.Count > attackIndex)
        {
            _attacks[attackIndex].gameObject.SetActive(true);
        }
    }

    public void StopAttackAnimationEvent(int attackIndex)
    {
        if (_attacks.Count > attackIndex)
        {
            _attacks[attackIndex].gameObject.SetActive(false);
        }

        // We switch the attack after we are done with the last one
        SwitchAnimation();
    }

    public void DeactivateAllAttacks()
    {
        foreach(Collider attack in _attacks)
        {
            attack.gameObject.SetActive(false);
        }
    }
}
