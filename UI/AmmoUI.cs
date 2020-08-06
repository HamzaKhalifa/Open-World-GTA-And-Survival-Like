using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private GameObject _ammoPanel = null;
    [SerializeField] private Text _magazineAmmoText = null;
    [SerializeField] private Text _ammoText = null;

    private void Update()
    {
        WeaponMount weaponMount = GameManager.Instance.Player.PlayerWeapons.EquippedWeaponMount;
        if (weaponMount.Weapon.WeaponType == WeaponType.Hands && _ammoPanel.gameObject.activeSelf)
        {
            _ammoPanel.gameObject.SetActive(false);
        } else if ((weaponMount.Weapon.IsRangeWeapon)
             && !_ammoPanel.gameObject.activeSelf)
        {
            _ammoPanel.gameObject.SetActive(true);
        }

        _magazineAmmoText.text = weaponMount.MagazineAmmo + "";
        _ammoText.text = weaponMount.Ammo + "";
    }
}
