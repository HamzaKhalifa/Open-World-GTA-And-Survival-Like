using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : MonoBehaviour
{
    [SerializeField] protected float _followRotationSpeed = 10f;

    public enum AIStateType
    {
        None, Idle, Patrol, Pursuit, Attack, Alert, Hurdle, Dancing, SocialInteraction, Vehicle, Sitting
    }

    [SerializeField] protected bool _useRootPosition = true;
    [SerializeField] protected bool _useRootRotation = true;
    [SerializeField] private float _numberOfAnimations = 1;
    [SerializeField] private string _animationParameter = "";

    #region Cache Fields

    protected AIStateMachine _stateMachine = null;
    public AIStateMachine StateMachine
    {
        set
        {
            _stateMachine = value;
        }

        get
        {
            return _stateMachine;
        }
    }

    #endregion

    #region Abstract Methods

    public abstract AIStateType GetStateType();
    public abstract void OnEnter();
    public abstract AIStateType OnUpdate();
    public abstract void OnExit();
    public abstract void HandleAnimator();

    #endregion

    #region Monobehvior Callbacks

    public void OnAnimatorUpdated()
    {
        if (_useRootPosition)
            _stateMachine.Agent.velocity = _stateMachine.Animator.deltaPosition / Time.deltaTime;
        if (_useRootRotation)
            transform.rotation = _stateMachine.Animator.rootRotation;
    }

    #endregion

    public void SwitchAnimation()
    {
        if (_stateMachine == null || _numberOfAnimations == 0 || _animationParameter == "") return;
        float numberOfAnimations = _numberOfAnimations;

        float nextAnimationParameterValue = Random.Range(0, (int)numberOfAnimations);
        float currentValue = _stateMachine.Animator.GetFloat(_animationParameter);

        _stateMachine.Animator.SetFloat(_animationParameter, (float)nextAnimationParameterValue);
        // Setting the next animation value in blend tree progressively
        //StartCoroutine(SwitchAnimationCoroutine(_animationParameter, currentValue, (float)nextAnimationParameterValue));
    }

    protected void KeepObjectiveInSight()
    {
        // Keeping the objective in sight
        Quaternion nextRotation = Quaternion.LookRotation(_stateMachine.CurrentTarget.TargetTransform.position - transform.position);
        nextRotation.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, nextRotation.eulerAngles.y, nextRotation.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, _followRotationSpeed * Time.deltaTime);
    }


    private IEnumerator SwitchAnimationCoroutine(string animationParameter, float currentValue, float nextValue)
    {
        float time = 0f;
        float delay = 1f;

        while (time < delay)
        {
            float normalizedTime = time / delay;

            float valueToSet = 0f;
            if (nextValue > currentValue)
            {
                valueToSet = currentValue + (nextValue - currentValue) * normalizedTime;
            }
            else valueToSet = currentValue - (currentValue - nextValue) * normalizedTime;

            _stateMachine.Animator.SetFloat(animationParameter, valueToSet);

            yield return null;
        }

        _stateMachine.Animator.SetFloat(_animationParameter, nextValue);
    }
}
