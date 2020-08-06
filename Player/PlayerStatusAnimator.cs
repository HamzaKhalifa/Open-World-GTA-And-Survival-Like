using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusAnimator : MonoBehaviour
{
    private Player _player = null;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        _player.Animator.SetBool("IsRecoveringEnergy", _player.PlayerStatus.IsRecoveringEnergy);
    }
}
