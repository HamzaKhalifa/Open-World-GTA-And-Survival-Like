using UnityEngine;

public class PlayerWeaponsAnimator : MonoBehaviour
{
    [SerializeField] private float _bowRightLookOffset = .35f;
    [SerializeField] private Transform _upperBodyPart = null;

    private Player _player = null;
    private Animator _animator = null;

    private float _horizontal = 0f;
    private float _vertical = 0f;
    private bool _isReloading = false;
    private Quaternion _previousRoation = Quaternion.identity;

    public bool IsReloading { get { return _isReloading; } set { _isReloading = value; } }

    public bool PreparedWeapon => _player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared;

    public bool PreparedPistol => _player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared &&
        _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType == WeaponType.Pistol;

    public bool PreparedRifle => _player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared &&
        _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType == WeaponType.Rifle;

    public bool PreparedKatana => _player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared &&
        _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType == WeaponType.Melee;

    public bool PreparedBow => _player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared &&
        _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType == WeaponType.Bow;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetBool("PreparedPistol", PreparedPistol);
        _animator.SetBool("PreparedKatana", PreparedKatana);
        _animator.SetBool("PreparedRifle", PreparedRifle);
        _animator.SetBool("PreparedWeapon", PreparedWeapon);
        _animator.SetBool("PreparedBow", PreparedBow);

        float maxAnimatorSpeed = 0f;

        switch (_player.PlayerState.PlayerMoveState)
        {
            case PlayerMoveState.Walking:
                maxAnimatorSpeed = 1;
                break;
            case PlayerMoveState.Running:
                maxAnimatorSpeed = 2;
                break;
            case PlayerMoveState.Sprinting:
                maxAnimatorSpeed = 3;
                break;
            case PlayerMoveState.NinjaSprint:
                maxAnimatorSpeed = 4;
                break;
        }

        float horizontal = maxAnimatorSpeed * GameManager.Instance.InputManager.Horizontal;
        float vertical = maxAnimatorSpeed * GameManager.Instance.InputManager.Vertical;

        _horizontal = Mathf.Lerp(_horizontal, horizontal, 5 * Time.deltaTime);
        _vertical = Mathf.Lerp(_vertical, vertical, 5 * Time.deltaTime);

        _animator.SetFloat("Horizontal", _horizontal);
        _animator.SetFloat("Vertical", _vertical);

        HandleReload();
    }

    public void Fire()
    {
        _animator.SetTrigger("Fire");
    }

    public void Charge(bool charge)
    {
        _animator.SetBool("IsCharging", charge);
    }

    public void FireAnimationEvent()
    {
        _player.PlayerWeaponFire.Fire(_player.PlayerWeapons.EquippedWeaponMount);
    }

    private void HandleReload()
    {
        if (GameManager.Instance.InputManager.Reload && !_isReloading && _player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared)
        {
            WeaponMount weaponMount = _player.PlayerWeapons.EquippedWeaponMount;

            if (weaponMount.MagazineAmmo < weaponMount.Weapon.MaxMagazineAmmo && weaponMount.Ammo > 0)
            {
                Reload();
            }
        }
    }

    public void Reload()
    {
        _animator.SetTrigger("Reload");
    }

    public void ReloadAnimationEvent()
    {
        WeaponMount weaponMount = _player.PlayerWeapons.EquippedWeaponMount;

        int needed = weaponMount.Weapon.MaxMagazineAmmo - weaponMount.MagazineAmmo;
        int toAdd = 0;
        if (weaponMount.Ammo > needed)
        {
            weaponMount.Ammo -= needed;
            toAdd = needed;
        } else
        {
            toAdd = weaponMount.Ammo;
            weaponMount.Ammo = 0;
        }

        weaponMount.MagazineAmmo += toAdd;

        // Refrech ammuniation decoration
        _player.PlayerWeapons.RefreshAmmunationDecoration(_player.PlayerWeapons.EquippedWeaponMount);
    }

    public void EquipAnimation(WeaponMount weaponMount)
    {
        if (weaponMount.EquipAnimatorTrigger != "")
        {
            _animator.SetTrigger(weaponMount.EquipAnimatorTrigger);
        }
    }

    #region Animator Callbacks

    private void LateUpdate()
    {
        // Making sure the player aims at the center of the screen when using the bow
        AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(_animator.GetLayerIndex("UpperBody"));

        if (PreparedBow && !currentAnimatorStateInfo.IsName("Default") && !currentAnimatorStateInfo.IsName("Bow Reload"))
        {
            Vector3 lookVector = Camera.main.transform.forward + Camera.main.transform.right * _bowRightLookOffset;
            Quaternion targetRotation = Quaternion.LookRotation(lookVector);
            Quaternion nextRotation = Quaternion.Euler(_upperBodyPart.rotation.eulerAngles.x, targetRotation.eulerAngles.y, _upperBodyPart.rotation.eulerAngles.z);

            _previousRoation = Quaternion.Lerp(_previousRoation, nextRotation, Time.deltaTime * 10);
            _upperBodyPart.transform.rotation = _previousRoation;

            //_hips.LookAt(Camera.main.transform.position + (Camera.main.transform.forward + (Camera.main.transform.right * _bowRightLookOffset)) * 1000);
        } else
        {
            _previousRoation = Quaternion.Lerp(_previousRoation, _upperBodyPart.transform.rotation, Time.deltaTime * 10);
            _upperBodyPart.transform.rotation = _previousRoation;
        }
    }

    #endregion
}
