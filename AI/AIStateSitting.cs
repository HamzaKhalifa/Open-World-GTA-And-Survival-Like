using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateSitting : AIState
{

    [SerializeField] [Range(0, 1)] private float _satisfaction = 1;
    [SerializeField] private float _satisfactionRecoveryRate = .1f;
    [SerializeField] private float _satisfactionDepletionRate = .01f;
    [SerializeField] private float _tiredThreshold = .3f;

    private InteractiveChair _interactiveChair = null;

    public InteractiveChair InteractiveChair => _interactiveChair;
    public bool IsTired => _satisfaction <= _tiredThreshold;

    private void Update()
    {
        if (_stateMachine == null) return;

        // If we aren't sitting, we should get tired slowly
        if (_stateMachine.CurrentState != null && _stateMachine.CurrentState.GetStateType() != AIStateType.Sitting)
        {
            _satisfaction -= Time.deltaTime * _satisfactionDepletionRate;
            _satisfaction = Mathf.Max(0, _satisfaction);
        }
    }

    public override AIStateType GetStateType()
    {
        return AIStateType.Sitting;
    }

    public override void OnEnter()
    {
        _interactiveChair = _stateMachine.CurrentTarget.TargetTransform.GetComponent<InteractiveChair>();

        _interactiveChair.IsOccupied = true;
        _stateMachine.Animator.SetBool("IsSitting", true);
        _stateMachine.Agent.enabled = false;
        StartCoroutine(SlideToChairSitPositionCoroutine());
    }

    public override AIStateType OnUpdate()
    {
        if (_stateMachine.HasHighPriorityTarget)
        {
            return AIStateType.Alert;
        }

        // Recover satisfaction as we keep sitting
        _satisfaction += Time.deltaTime * _satisfactionRecoveryRate;
        _satisfaction = Mathf.Min(1, _satisfaction);

        if (_satisfaction == 1)
        {
            _stateMachine.ResetTarget();
            return AIStateType.Idle;
        };

        return AIStateType.Sitting;
    }

    public override void OnExit()
    {
        _stateMachine.ResetTarget();
        _stateMachine.Animator.SetBool("IsSitting", false);
        _stateMachine.Agent.enabled = true;
        _interactiveChair.IsOccupied = false;
        _interactiveChair = null;
    }


    public override void HandleAnimator()
    {
        
    }

    private IEnumerator SlideToChairSitPositionCoroutine()
    {
        float time = 0f;
        float delay = 1f;

        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;
        Vector3 finalPosition = _interactiveChair.transform.position;
        Quaternion finalRotation = _interactiveChair.transform.rotation;

        while (time <= delay)
        {
            time += Time.deltaTime;
            float normalizedTime = time / delay;

            transform.position = Vector3.Lerp(initialPosition, finalPosition, normalizedTime);
            transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, normalizedTime);

            yield return null;
        }

        transform.position = finalPosition;
        transform.rotation = finalRotation;
    }
}
