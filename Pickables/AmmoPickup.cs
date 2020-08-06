using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : PickupItem
{
    [SerializeField] Weapon weapon = null;

    protected override void Pickup(Transform playerTransform)
    {
        PlayerWeapons playerWeapons = playerTransform.GetComponent<PlayerWeapons>();
        playerWeapons.ObtainWeapon(weapon);
    }
}
