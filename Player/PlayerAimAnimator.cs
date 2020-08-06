using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimAnimator : MonoBehaviour
{
    [SerializeField] private float _verticalMinAngle = -45f;
    [SerializeField] private float _verticalMaxAngle = 45f;

    private Player _player = null;
    private Animator _animator = null;
    private Camera _mainCamera = null;
    private float _animatorLerpedHorizontalValue = 0f;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_player.PlayerState.PlayerMoveState == PlayerMoveState.BusyDressing)
        {
            _animator.SetFloat("AimHorizontal", 0);
            _animator.SetFloat("AimVertical", 0);

            return;
        }

        #region Vertical Look

        float currentXAngle = _mainCamera.transform.rotation.eulerAngles.x;
        float animatorAimVerticalValue = 0f;
        if (currentXAngle > 300)
        {
            animatorAimVerticalValue = (360 - currentXAngle) / _verticalMaxAngle;
        } else
        {
            animatorAimVerticalValue = currentXAngle / _verticalMinAngle;
        }

        _animator.SetFloat("AimVertical", animatorAimVerticalValue);

        #endregion

        #region Horizontal Look

        float currentYAngle = _mainCamera.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;

        float animatorNextHorizontalValue = 0f;
        if (currentYAngle > 0)
        {
            if (currentYAngle < 180)
            {
                animatorNextHorizontalValue = currentYAngle / 180;
            }
            else
            {
                animatorNextHorizontalValue = -((360 - currentYAngle) / 180);
            }
        } else
        {
            if (currentYAngle > -180)
            {
                animatorNextHorizontalValue = currentYAngle / 180;
            } else
            {
                animatorNextHorizontalValue = (360 - Mathf.Abs(currentYAngle)) / 180;
            }
        }

        _animatorLerpedHorizontalValue = Mathf.Lerp(_animatorLerpedHorizontalValue, animatorNextHorizontalValue, 10f * Time.deltaTime);
        _animator.SetFloat("AimHorizontal", _animatorLerpedHorizontalValue);

        #endregion
    }
}
