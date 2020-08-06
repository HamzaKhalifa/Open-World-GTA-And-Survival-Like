using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponMount
{
    public Weapon Weapon = null;
    public Transform EquippedWeaponTransform = null;
    public Transform EquippedThrowableDecorationTransform = null;
    public Transform DecorationTransform = null;
    public Transform CarrierDecoration = null;
    public Transform FirePosition = null;
    public bool InPossession = false;
    public int Ammo = 0;
    public int MagazineAmmo = 0;
    public ParticleSystem MuzzleFlash = null;
    public List<GameObject> AmmunationDecorations = new List<GameObject>();
    public string EquipAnimatorTrigger = "";
    public GameObject MeleeAttackTrigger = null;
    public TrailRenderer MeleeTrailRenderer = null;
}

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] private List<WeaponMount> _allWeaponMounts = new List<WeaponMount>();
    [SerializeField] private AudioClip _equipWeaponSound = null;

    private List<WeaponMount> _weaponMountsInPossession = new List<WeaponMount>();

    private WeaponMount _equippedWeaponMount = null;
    public WeaponMount EquippedWeaponMount => _equippedWeaponMount;

    private Player _player = null;

    private void Start()
    {
        _player = GetComponent<Player>();
        UpdateWeaponStatesFromAllWeapons();
    }

    private void Update()
    {
        if (GameManager.Instance.InputManager.SwitchWeaponUp)
        {
            Equip(1);
        }
        if (GameManager.Instance.InputManager.SwitchWeaponDown)
        {
            Equip(-1);
        }

        // Showing or unshowing the equipped weapon Throwable decoration (for the bow for example)
        if(EquippedWeaponMount.EquippedThrowableDecorationTransform != null)
        {
            if (EquippedWeaponMount.MagazineAmmo > 0 && !EquippedWeaponMount.EquippedThrowableDecorationTransform.gameObject.activeSelf)
            {
                EquippedWeaponMount.EquippedThrowableDecorationTransform.gameObject.SetActive(true);
            }

            if (EquippedWeaponMount.MagazineAmmo <= 0 && EquippedWeaponMount.EquippedThrowableDecorationTransform.gameObject.activeSelf)
            {
                EquippedWeaponMount.EquippedThrowableDecorationTransform.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// equipping the current weapon, updating the weapons in possession list and managing the decorations
    /// </summary>
    private void UpdateWeaponStatesFromAllWeapons()
    {
        _weaponMountsInPossession.Clear();
        foreach (WeaponMount weaponMount in _allWeaponMounts)
        {
            RefreshAmmunationDecoration(weaponMount);

            EquipOrUnequip(weaponMount, false);
            if (weaponMount.InPossession)
            {
                _weaponMountsInPossession.Add(weaponMount);
            }
            else
            {
                if (weaponMount.DecorationTransform != null)
                {
                    weaponMount.DecorationTransform.gameObject.SetActive(false);

                    if (weaponMount.CarrierDecoration != null)
                        weaponMount.CarrierDecoration.gameObject.SetActive(false);
                }
            }
        }

        // We equip the current weapon
        Equip(0);
    }

    private void Equip(int scroll)
    {
        if (scroll != 0)
            GameManager.Instance.AudioManager.PlayOneShotSound(_equipWeaponSound, 1, 0, 1, transform.position);

        // When we equip a new weapon, we unprepare the previous one
        if (scroll != 0)
            _player.PlayerState.UnprepareWeapon();

        int equippedWeaponIndex = 0;
        for (int i = 0; i < _weaponMountsInPossession.Count; i++)
        {
            if (_weaponMountsInPossession[i] == _equippedWeaponMount)
            {
                equippedWeaponIndex = i;
            }
        }

        equippedWeaponIndex += scroll;

        if (equippedWeaponIndex >= _weaponMountsInPossession.Count) equippedWeaponIndex = 0;
        if (equippedWeaponIndex < 0)
            equippedWeaponIndex = _weaponMountsInPossession.Count - 1;

        foreach(WeaponMount weaponMount in _weaponMountsInPossession)
        {
            EquipOrUnequip(weaponMount, false);
        }

        EquipOrUnequip(_weaponMountsInPossession[equippedWeaponIndex], true);

        _equippedWeaponMount = _weaponMountsInPossession[equippedWeaponIndex];
    }

    public void EquipOrUnequip(WeaponMount weaponMount, bool equip)
    {
        if (weaponMount.DecorationTransform != null)
            weaponMount.DecorationTransform.gameObject.SetActive(!equip);

        if (weaponMount.EquippedWeaponTransform != null)
            weaponMount.EquippedWeaponTransform.gameObject.SetActive(equip);

        if (weaponMount.EquippedThrowableDecorationTransform != null)
        {
            weaponMount.EquippedThrowableDecorationTransform.gameObject.SetActive(equip);
        }

        if (weaponMount.CarrierDecoration != null)
            weaponMount.CarrierDecoration.gameObject.SetActive(true);

        // We play the equip animation
        if (equip)
            _player.PlayerWeaponsAnimator.EquipAnimation(weaponMount);
    }

    public void RefreshAmmunationDecoration(WeaponMount weaponMount)
    {
        if (weaponMount.AmmunationDecorations.Count == 0) return;

        for (int i = 0; i < weaponMount.AmmunationDecorations.Count; i++)
        {
            if (i < weaponMount.Ammo)
            {
                weaponMount.AmmunationDecorations[i].gameObject.SetActive(true);
            } else
            {
                weaponMount.AmmunationDecorations[i].gameObject.SetActive(false);
            }
        }
    }

    public void ObtainWeapon(Weapon weapon)
    {
        foreach(WeaponMount weaponMount in _allWeaponMounts)
        {
            if (weaponMount.Weapon == weapon)
            {
                weaponMount.InPossession = true;
                weaponMount.Ammo = weapon.MaxAmmo;
                weaponMount.MagazineAmmo = weapon.MaxMagazineAmmo;

                RefreshAmmunationDecoration(weaponMount);
            }
        }

        UpdateWeaponStatesFromAllWeapons();
    }
}
