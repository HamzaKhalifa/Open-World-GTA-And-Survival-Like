using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractorAnimator : MonoBehaviour
{
    private Player _player = null;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    public void BreakDoorAnimationEvent()
    {
        _player.PlayerInteractor.InteractiveDoor.Interact(_player.transform);
    }
}
