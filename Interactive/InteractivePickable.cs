using System.Collections;
using System.Collections.Generic;
using SickscoreGames.HUDNavigationSystem;
using UnityEngine;

public enum InteractiveItemType
{
    None,
    Jerrycan,
    Weapon,
    Item
}

[RequireComponent(typeof(Rigidbody))]
public class InteractivePickable : InteractiveObject
{
    [SerializeField] private string _useAnimationName = null;
    [SerializeField] private InteractiveItemType _interactiveItemType = InteractiveItemType.None;
    [SerializeField] private bool _used = false;
    [SerializeField] LayerMask _interactLayerMask;
    [Tooltip("If the interactive item is of type weapon")]
    [SerializeField] private Weapon _weapon = null;
    [Tooltip("If the interactive item is of type item")]
    [SerializeField] private Item _item = null;
    [SerializeField] private AudioClip _pickupSound = null;

    private Collider _collider = null;
    private Rigidbody _rigidBody = null;
    private Character _interactor = null;
    private Transform _targetObject = null;
    private HUDNavigationElement _hudNavigationElement = null;

    public InteractiveItemType InteractiveItemType => _interactiveItemType;
    public LayerMask InteractLayerMask => _interactLayerMask;
    public Weapon Weapon => _weapon;
    public Item Item => _item;
    public AudioClip PickupSound => _pickupSound;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidBody = GetComponent<Rigidbody>();
        _hudNavigationElement = GetComponent<HUDNavigationElement>();
    }

    public override void Interact(Transform interactor)
    {
        base.Interact(interactor);

        _interactor = interactor.GetComponent<Character>();
        _collider.enabled = false;
        _rigidBody.isKinematic = true;

        Player player = interactor.GetComponent<Player>();

        if (player != null)
            player.PlayerPickAnimator.Pick(this);

        if (_hudNavigationElement != null)
            _hudNavigationElement.enabled = false;
    }

    public void Drop()
    {
        _interactor = null;
        _collider.enabled = true;
        _rigidBody.isKinematic = false;

        if (!_used && _hudNavigationElement != null) 
            _hudNavigationElement.enabled = true;
    }

    public void UseTrigger(Transform targetObject, System.Action<bool> callback)
    {
        if (_used) {
            callback(false);
            return;
        };

        callback(true);

        _targetObject = targetObject;
        _interactor.Animator.SetTrigger(_useAnimationName);
    }

    public void Use()
    {
        if (_targetObject == null) return;

        switch(_interactiveItemType)
        {
            case InteractiveItemType.Jerrycan:
                Vehicle vehicle = _targetObject.GetComponentInParent<Vehicle>();
                vehicle.RefillFuel();
                break;
            case InteractiveItemType.None:
                break;
        }

        _used = true;
    }
}
