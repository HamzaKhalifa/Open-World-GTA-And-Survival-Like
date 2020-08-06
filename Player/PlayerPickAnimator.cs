using System.Collections;
using UnityEngine;

public class PlayerPickAnimator : MonoBehaviour
{
    [SerializeField] private Transform _pickedItemPositionTransform = null;
    [SerializeField] private float _downPickThreshold = 20f;
    [SerializeField] private float _highPickThreshold = 90f;

    #region Cache Fields

    private Player _player = null;
    private Animator _animator = null;

    #endregion


    private InteractivePickable _interactivePickable = null;
    // Using this boolean because we don't want to drop the item right after picking it (the update function gets called in the same frame as that during which we set the interactiveItem)
    private bool _canDrop = true;
    private bool _canUse = false;
    private bool _justUsed = false;

    public bool HoldingItem => _interactivePickable != null;
    public InteractivePickable InteractivePickable => _interactivePickable;
    public bool CanUse { set { _canUse = value; } }

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance.InputManager.Interact && _interactivePickable != null
            && _canDrop && !_canUse)
        {
            Drop();
        } 
    }

    private void Drop()
    {
        _interactivePickable.Drop();
        _interactivePickable.transform.parent = null;
        _interactivePickable = null;
    }

    public void Pick(InteractivePickable interactivePickable)
    {
        _player.PlayerState.UnprepareWeapon();

        _canDrop = false;
        Invoke("ResetCanDrop", 1);
        _interactivePickable = interactivePickable;

        // Calculate the angle between the forward vector and that to the item to know which animation to play: Down or front pickup
        float angle = Vector3.Angle(_player.transform.forward, (interactivePickable.transform.position - _player.transform.position).normalized);
        string animatorTrigger = "PickItemFront";
        if (angle < _downPickThreshold)
        {
            animatorTrigger = "PickItemDown";
        } else if (angle >= _downPickThreshold && angle < _highPickThreshold)
        {
            animatorTrigger = "PickItemFront";
        } else if (angle >= _highPickThreshold)
        {
            animatorTrigger = "PickItemHigh";
        }

        _animator.SetTrigger(animatorTrigger);
    }

    private void ResetCanDrop()
    {
        _canDrop = true;
    }

    public void PickAnimationEvent()
    {
        StartCoroutine(PickCoroutine());
    }

    private IEnumerator PickCoroutine()
    {
        if (_interactivePickable == null) yield break;

        _interactivePickable.transform.parent = _pickedItemPositionTransform;

        float time = 0f;
        float delay = .5f;

        Vector3 initialLocalPosition = _interactivePickable.transform.localPosition;
        Quaternion initialLocalRotation = _interactivePickable.transform.rotation;

        while (time <= delay)
        {
            time += Time.deltaTime;
            float normalizedTime = time / delay;

            _interactivePickable.transform.localPosition = Vector3.Lerp(initialLocalPosition, Vector3.zero, normalizedTime);
            _interactivePickable.transform.localRotation = Quaternion.Lerp(initialLocalRotation, Quaternion.identity, normalizedTime);

            yield return null;
        }

        _interactivePickable.transform.localPosition = Vector3.zero;
        _interactivePickable.transform.localRotation = Quaternion.identity;

        bool didPick = false;
        // If we are picking a weapon, then we add it to our playerweapons
        if (_interactivePickable.InteractiveItemType == InteractiveItemType.Weapon)
        {
            // We play the pick sound
            if (_interactivePickable.PickupSound != null)
            {
                GameManager.Instance.AudioManager.PlayOneShotSound(_interactivePickable.PickupSound, 1, 0, 1, transform.position);
            }

            didPick = true;
            _player.PlayerWeapons.ObtainWeapon(_interactivePickable.Weapon);
            
        } else if (_interactivePickable.InteractiveItemType == InteractiveItemType.Item)
        {
            // If we are picking an item, we add it to our backpack
            // We play the pick sound
            if (_interactivePickable.PickupSound != null)
            {
                GameManager.Instance.AudioManager.PlayOneShotSound(_interactivePickable.PickupSound, 1, 0, 1, transform.position);
            }

            if (UIManager.Instance.BackpackUI.AddItem(_interactivePickable.Item))
            {
                didPick = true;
            } else
            {
                Drop();
            }
        }

        if(didPick)
        {
            Destroy(_interactivePickable.gameObject);
            _interactivePickable = null;
        }
    }

    public void Use(Transform targetObject)
    {
        if (_interactivePickable == null && _justUsed) return;

        _justUsed = true;
        Invoke("ResetJustUsed", 1);
        _interactivePickable.UseTrigger(targetObject, (success) => {
            if (success)
                _player.PlayerState.PlayerMoveState = PlayerMoveState.InteractingWithItem;
            else
                Drop();
        });
    }

    private void ResetJustUsed()
    {
        _justUsed = false;
    }

    public void UseAnimationEvent()
    {
        if (_interactivePickable == null) return;

        _interactivePickable.Use();
    }
}
