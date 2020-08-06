using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private BackpackUI _backPackUI = null;
    [SerializeField] private GameObject _crossImage = null;
    [SerializeField] private Image _itemImage = null;
    [SerializeField] private Color _selectedBorderColor = Color.black;
    [SerializeField] private Color _unselectedBorderColor = Color.black;
    [SerializeField] private Image _borderImage = null;

    private Item _item = null;
    private bool _selected = true;

    public Item Item => _item;

    private void Awake()
    {
        SelectOrUnselectButton();
    }

    public void Put(Item item)
    {
        _item = item;
        _itemImage.sprite = item.Sprite;
        _itemImage.gameObject.SetActive(true);
        _crossImage.SetActive(false);
    }

    public void TakeOff()
    {
        _item = null;
        _itemImage.sprite = null;
        _itemImage.gameObject.SetActive(false);
        _crossImage.SetActive(true);
    }

    public void SelectOrUnselectButton()
    {
        bool toSelect = !_selected;

        // The field selected gets changed in this function, so we need to create a temporary one : toSelect
        _backPackUI.UnSelectAllItems();

        _borderImage.color = toSelect ? _selectedBorderColor : _unselectedBorderColor;

        if (toSelect)
        {
            _backPackUI.SelectOrUnselectItem(_item, true);
        }
    }

    public void UnSelectItem()
    {
        _selected = false;

        _borderImage.color = _unselectedBorderColor;
    }

}
