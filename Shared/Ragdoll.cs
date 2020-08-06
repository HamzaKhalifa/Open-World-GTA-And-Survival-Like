using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] List<Rigidbody> _bodyParts = new List<Rigidbody>();

    private Animator _animator = null;
    public Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                if (_animator == null)
                    _animator = GetComponentInChildren<Animator>();
            }

            return _animator;
        }
    }

    public void HandleRagdoll(bool ragdoll)
    {
        Animator.enabled = !ragdoll;

        foreach (Rigidbody bodyPart in _bodyParts)
        {
            bodyPart.GetComponent<Collider>().isTrigger = !ragdoll;
            bodyPart.isKinematic = !ragdoll;
        }
    }
}
