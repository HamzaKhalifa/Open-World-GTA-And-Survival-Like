using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObstacleMountAnimator : MonoBehaviour
{
    [SerializeField] private float _upwardsMountTime = .25f;
    [SerializeField] private float _forwardMountTime = .5f;
    [SerializeField] private float _mountForwardTransition = 1f;

    private Player _player = null;
    private Animator _animator = null;

    private IEnumerator _mountCouroutine = null;
    private bool _hasJustTriggeredMountAnimation = false;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // If we can mount a tiny obstacle and the character controller is moving and we aren't already mounting, then we mount
        if (_player.PlayerObstacleMount.CanMountTinyObstacle && GameManager.Instance.InputManager.IsTryingToMove && _mountCouroutine == null && !_hasJustTriggeredMountAnimation
            && _player.PlayerMove.IsGrounded)
        {
            _hasJustTriggeredMountAnimation = true;
            _animator.SetTrigger("MountTinyObstacle");
        } 
    }

    public void MountObstacleAnimationEvent()
    {
        _mountCouroutine = MountCoroutine(_player.PlayerObstacleMount.ObstacleHeight);
        StartCoroutine(_mountCouroutine);

        _hasJustTriggeredMountAnimation = false;
    }

    private IEnumerator MountCoroutine(float obstacleHeight)
    {
        _player.CharacterController.enabled = false;

        // This is the upwards transition first
        Vector3 destinationPoint = _player.transform.position + (_player.transform.up * obstacleHeight);
        Vector3 initialPosition = _player.transform.position;

        float time = 0f;

        while (time < _upwardsMountTime)
        {
            time += Time.deltaTime;
            float normalizeTime = time / _upwardsMountTime;

            _player.transform.position = Vector3.Lerp(initialPosition, destinationPoint, normalizeTime);

            yield return null;
        }

        // Then comes the forward transition
        destinationPoint = _player.transform.position + (_player.transform.forward * _mountForwardTransition);
        initialPosition = _player.transform.position;

        time = 0f;

        while (time < _forwardMountTime)
        {
            time += Time.deltaTime;
            float normalizeTime = time / _forwardMountTime;

            _player.transform.position = Vector3.Lerp(initialPosition, destinationPoint, normalizeTime);

            yield return null;
        }

        _player.transform.position = destinationPoint;

        _player.CharacterController.enabled = true;

        _mountCouroutine = null;
    }

}
