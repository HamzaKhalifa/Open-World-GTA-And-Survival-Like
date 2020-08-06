using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBackpackAnimator : MonoBehaviour
{
    private Player _player = null;

    private Item _selectedItem = null;

    public Item SelectedItem
    {
        set
        {
            _selectedItem = value;
        }
    }

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    public void UseItemAnimationEvent()
    {
        if (_selectedItem == null) return;

        _selectedItem.Use(_player);
    }
}
