using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ForwardAxis
{
    Forward,
    Right,
    Up
}
[RequireComponent(typeof(Animator))]
public class InteractiveDoor : InteractiveObject
{
    [SerializeField] protected bool _breakable = false;
    [SerializeField] protected bool _isClosed = true;
    [SerializeField] private AudioClip _interactSound = null;
    [SerializeField] private AudioClip _breakSound = null;
    [SerializeField] private bool _reversed = false;
    [SerializeField] private ForwardAxis _forwardAxis = ForwardAxis.Forward;

    private Animator _animator = null;

    public bool Breakable => _breakable;
    public bool IsClosed => _isClosed;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _animator.SetBool("Breakable", _breakable);
        Debug.Log(_breakable);
        Debug.Log("Dude " + transform.name);
    }

    public override void Interact(Transform interactor)
    {
        base.Interact(interactor);

        _isClosed = !_isClosed;

        AudioClip clip = _interactSound;
        if (_breakable && !_isClosed)
        {
            clip = _breakSound;
        }

        GameManager.Instance.AudioManager.PlayOneShotSound(clip, 1, 0, 1, transform.position);

        Vector3 forward = transform.forward;
        switch(_forwardAxis)
        {
            case ForwardAxis.Up:
                forward = transform.up;
                break;
            case ForwardAxis.Right:
                forward = transform.right;
                break;
            default:
                forward = transform.forward;
                break;
        }
        float angle = Vector3.Angle(interactor.forward, forward);

        if (_isClosed) {
            _animator.SetTrigger("Close");
        } else
        {
            if (angle < 120)
            {
                if(_reversed)
                    _animator.SetTrigger("OpenBack");
                else
                    _animator.SetTrigger("OpenFront");
            }
            else
            {
                if (_reversed)
                    _animator.SetTrigger("OpenFront");
                else
                    _animator.SetTrigger("OpenBack");
            }
        }
    }
}
