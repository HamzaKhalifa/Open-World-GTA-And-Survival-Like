using UnityEngine;

public class PlayerWeaponLook : MonoBehaviour
{
    private Player _player = null;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (_player.PlayerState.PlayerWeaponState != PlayerWeaponState.Prepared || _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType == WeaponType.Melee ) return;

        transform.rotation = Quaternion.Euler(transform.rotation.x, Camera.main.transform.rotation.eulerAngles.y, transform.rotation.z);
    }
}
