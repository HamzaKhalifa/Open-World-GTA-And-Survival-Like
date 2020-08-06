using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackUI : MonoBehaviour
{
    [SerializeField] private List<Item> _items = new List<Item>();
    [SerializeField] private List<ItemUI> _itemsUIs = new List<ItemUI>();
    [SerializeField] private Animator _backPackPanel = null;
    [SerializeField] private AudioClip _showHideSound = null;
    [SerializeField] private Text _itemTitleText = null;
    [SerializeField] private Text _itemDescriptionText = null;
    [SerializeField] private AudioClip _dropItemSound = null;

    private bool _showing = false;

    private Item _selectedItem = null;

    private void Awake()
    {
        RefreshItems();
    }

    private void Update()
    {
        if (GameManager.Instance.InputManager.Backpack)
        {
            _showing = !_showing;
            _backPackPanel.SetBool("Showing", _showing);

            HandleCursor();

            GameManager.Instance.AudioManager.PlayOneShotSound(_showHideSound, 1, 0, 2, transform.position);
        }
    }

    private void HandleCursor()
    {
        // Handle Cursor
        if (_showing)
        {
            GameManager.Instance.PlayerCamera.MouseLock(false);
        }
        else
        {
            GameManager.Instance.PlayerCamera.MouseLock(true);
        }
    }

    private void RefreshItems()
    {
        UnSelectAllItems();
        for (int i = 0; i < _itemsUIs.Count; i++)
        {
            if (_items.Count > i)
            {
                _itemsUIs[i].Put(_items[i]);
            } else
            {
                _itemsUIs[i].TakeOff();
            }
        }
    }

    public void UnSelectAllItems()
    {
        foreach (ItemUI itemUI in _itemsUIs)
        {
            itemUI.UnSelectItem();
        }
    }

    public void SelectOrUnselectItem(Item item, bool selected)
    {
        if (selected)
            _selectedItem = item;
        else _selectedItem = null;

        if (item == null) return;

        _itemDescriptionText.text = selected ? item.Description : "";
        _itemTitleText.text = selected ? item.Name : "";

    }

    public bool AddItem(Item item)
    {
        if (_items.Count <= _itemsUIs.Count)
        {
            _items.Add(item);
            RefreshItems();
            return true;
        }
        else return false;
    }

    public void UseItemButton()
    {
        if (_selectedItem == null) return;

        if (GameManager.Instance.Player.PlayerState.IsBusyWithWall) return;

        if (GameManager.Instance.Player.PlayerState.PlayerMoveState == PlayerMoveState.InteractingWithItem) return;

        _items.Remove(_selectedItem);
        RefreshItems();

        GameManager.Instance.Player.PlayerState.UnprepareWeapon();
        GameManager.Instance.Player.PlayerBackpackAnimator.SelectedItem = _selectedItem;
        GameManager.Instance.Player.Animator.SetTrigger(_selectedItem.UseAnimation);
    }

    public void DropItem()
    {
        if (_selectedItem == null) return;

        // We instantiate the interactive pickable right in front of the player:
        InteractivePickable interactivePickable = Instantiate(_selectedItem.InteractivePickable, GameManager.Instance.Player.transform.position + Vector3.up + GameManager.Instance.Player.transform.forward, Quaternion.identity);

        GameManager.Instance.AudioManager.PlayOneShotSound(_dropItemSound, 1, 0, 2, GameManager.Instance.Player.transform.position);


        _items.Remove(_selectedItem);

        // The takeoff item function will be called via the refreshitems function
        RefreshItems();

        _selectedItem = null;
    }
}
