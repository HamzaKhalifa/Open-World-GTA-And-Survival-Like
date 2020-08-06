using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveAutomaticDoor : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;

    private bool _collidingWithPlayer = false;
    private float _nextResetTime = 0;

    private void Update()
    {
        _animator.SetBool("OpenDoor", _collidingWithPlayer);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _collidingWithPlayer = true;
            _nextResetTime = Time.time + 2f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke("ResetCollidingWithPlayer", 3f);
        }
    }

    private void ResetCollidingWithPlayer()
    {
        if (Time.time >= _nextResetTime)
        {
            _collidingWithPlayer = false;
        }
    }
}
