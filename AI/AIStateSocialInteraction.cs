using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateSocialInteraction : AIState
{
    [SerializeField] [Range(0, 1)] private float _satisfaction = 1;
    [SerializeField] private float _satisfactionRecoveryRate = .1f;
    [SerializeField] private float _satisfactionDepletionRate = .01f;
    [SerializeField] private float _hungryThreshold = .3f;
    [SerializeField] private float _emotionDelayMin = 10f;
    [SerializeField] private float _emotionDelayMax = 40f;
    [SerializeField] private List<string> _emotionTriggers = new List<string>();

    private AIStateSocialInteraction _friendSocialInteraction = null;
    private float _nextEmotionTime = 0f;

    public AIStateSocialInteraction FriendSocialInteraction { set { _friendSocialInteraction = value; } }

    public bool IsFeelingLonely => _satisfaction <= _hungryThreshold;

    public bool IsNoLongerLonely => _satisfaction == 1;

    private void Start()
    {
        _satisfaction = Random.Range(0f, 1f);
    }

    private void Update()
    {
        if (_stateMachine == null) return;

        // If we aren't interacting with anyone, we should progressively start feeling lonely
        if (_stateMachine.CurrentState != null && _stateMachine.CurrentState.GetStateType() != AIStateType.Dancing)
        {
            _satisfaction -= Time.deltaTime * _satisfactionDepletionRate;
            _satisfaction = Mathf.Max(0, _satisfaction);
        }
    }

    public override AIStateType GetStateType()
    {
        return AIStateType.SocialInteraction;
    }

    public override void OnEnter()
    {
        SwitchAnimation();

        _stateMachine.Animator.SetBool("IsInteractingSocially", true);
        _stateMachine.Animator.SetInteger("Speed", 0);

        // React just after starting the conversation 
        _nextEmotionTime = Time.time;

        _stateMachine.Animator.SetBool("Alert", true);
    }

    public override AIStateType OnUpdate()
    {
        if (_stateMachine.HasHighPriorityTarget)
        {
            return AIStateType.Alert;
        }

        // If we no longer see a friend or a person to interact with, or the friend got distracted by something else, then we go back to idling
        if (_stateMachine.CurrentTarget == null
            || (_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type != AITargetType.Friend)
            || _friendSocialInteraction == null
            || _friendSocialInteraction.StateMachine.Health.IsDead
            || _friendSocialInteraction.IsNoLongerLonely
            || _friendSocialInteraction.StateMachine.CurrentTarget.Type != AITargetType.Friend)
        {
            // If we were no longer interacting with a friend because he is no longer interacting with us for whatever reason, then we reset the target (to not keep looping between idle and social interaction)
            if(_stateMachine.CurrentTarget != null && _stateMachine.CurrentTarget.Type == AITargetType.Friend)
            {
                _stateMachine.ResetTarget();
            }

            // If our friend is angry at something, then we become angry at it too
            if(_friendSocialInteraction != null && _friendSocialInteraction.StateMachine.CurrentTarget != null
                &&( _friendSocialInteraction.StateMachine.CurrentTarget.Type == AITargetType.Enemy || _friendSocialInteraction.StateMachine.CurrentTarget.Type == AITargetType.Player))
            {
                _stateMachine.SetTarget(_friendSocialInteraction.StateMachine.CurrentTarget.TargetTransform, _friendSocialInteraction.StateMachine.CurrentTarget.Type);
                _stateMachine.AISCanner.AddPotentialThreat(_friendSocialInteraction.StateMachine.CurrentTarget.TargetTransform.gameObject);
            }
            return AIStateType.Idle;
        }

        // Recover satisfaction as we keep interacting
        _satisfaction += Time.deltaTime * _satisfactionRecoveryRate;
        _satisfaction = Mathf.Min(1, _satisfaction);

        if (_satisfaction >= 1)
        {
            _stateMachine.ResetTarget();
            return AIStateType.Idle;
        };

        // Triggering emotions periodically
        if (Time.time >= _nextEmotionTime)
        {
            _stateMachine.Animator.SetTrigger(_emotionTriggers[Random.Range(0, _emotionTriggers.Count)]);
            _nextEmotionTime = Time.time + Random.Range(_emotionDelayMin, _emotionDelayMax);
        }

        KeepObjectiveInSight();

        return AIStateType.SocialInteraction;
    }

    public override void OnExit()
    {
        _stateMachine.Animator.SetBool("IsInteractingSocially", false);
        FriendSocialInteraction = null;
    }


    public override void HandleAnimator()
    {
    }

}
