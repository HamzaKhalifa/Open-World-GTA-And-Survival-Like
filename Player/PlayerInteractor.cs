using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private List<Transform> _checkOrigins = new List<Transform>();
    [SerializeField] private float _checkDistance = 3f;

    private Player _player = null;
    private InteractiveDoor _interactiveDoor = null;

    public InteractiveDoor InteractiveDoor => _interactiveDoor;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        bool foundSomethingToInteractWith = false;

        RaycastHit hitInfo;
        foreach (Transform checkOrigin in _checkOrigins)
        {
            LayerMask layerMask = LayerMask.GetMask("Interactive");

            // If we are holding an item, we are only going to interact with the objects that the item we are holding can interact with
            if (_player.PlayerPickAnimator.HoldingItem)
            {
                layerMask = _player.PlayerPickAnimator.InteractivePickable.InteractLayerMask;
            }

            if (Physics.Raycast(checkOrigin.position, checkOrigin.transform.forward, out hitInfo, _checkDistance, layerMask)
                || _player.PlayerWallClimb.CanMount)
            {
                foundSomethingToInteractWith = true;
                UIManager.Instance.SetInteractiveSignal(true);

                if (_player.PlayerPickAnimator.HoldingItem && GameManager.Instance.InputManager.Interact)
                {
                    // Here, we use the item we are holding
                    _player.PlayerPickAnimator.Use(hitInfo.transform);
                    _player.transform.forward = -hitInfo.normal;
                } else
                {
                    // We don't need to interact with objects when we are in climb mode
                    if (_player.PlayerState.PlayerMoveState != PlayerMoveState.WallClimbing)
                    {
                        if (GameManager.Instance.InputManager.Interact && hitInfo.transform != null)
                        {
                            InteractiveObject interactiveObject = hitInfo.transform.GetComponentInChildren<InteractiveObject>();
                            if (interactiveObject != null)
                            {
                                // If it's a breakable door, then we play the kick animation first
                                InteractiveDoor interactiveDoor = interactiveObject.GetComponent<InteractiveDoor>();
                                if (interactiveDoor != null && interactiveDoor.Breakable && interactiveDoor.IsClosed)
                                {
                                    _interactiveDoor = interactiveDoor;
                                    Quaternion lookRotation = Quaternion.LookRotation((interactiveDoor.transform.position - _player.transform.position).normalized);
                                    _player.transform.rotation = Quaternion.Euler(_player.transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, _player.transform.rotation.eulerAngles.z);
                                    //_player.transform.forward = (interactiveDoor.transform.position - _player.transform.position).normalized;
                                    
                                    _player.Animator.SetTrigger("Kick");
                                } else
                                {
                                    interactiveObject.Interact(transform);
                                }
                                break;
                            }
                        }
                    }
                }

            }
        }
        if (!foundSomethingToInteractWith)
        {
            UIManager.Instance.SetInteractiveSignal(false);
        }

        // if we are holding an item and we are in front of a target object of the item we are holding, then we should set the canuse variable of playerPickAnimator script
        if (_player.PlayerPickAnimator.HoldingItem && foundSomethingToInteractWith)
        {
            _player.PlayerPickAnimator.CanUse = true;
        } else
        {
            _player.PlayerPickAnimator.CanUse = false;
        }
    }
}
