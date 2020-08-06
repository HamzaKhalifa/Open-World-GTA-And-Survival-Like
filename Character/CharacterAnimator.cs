using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    protected Animator _animator = null;

    public Animator Animator => _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    #region Animator Callbacks

    protected virtual void OnAnimatorMove()
    {
        
    }

    protected virtual void OnAnimatorIK(int layerIndex)
    {
        
    }

    #endregion
}
