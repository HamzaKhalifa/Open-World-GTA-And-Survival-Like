using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponFire : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    private Player _player = null;

    private float _nextFireTime = 0f;
    private bool _isCharging = false;

    public bool IsCharging => _isCharging;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (_player.PlayerWeapons.EquippedWeaponMount == null) return;

        WeaponMount weaponMount = _player.PlayerWeapons.EquippedWeaponMount;

        // Can't fire when spriting or busy with wall or weapon is unprepared
        if (GameManager.Instance.InputManager.Firing
            //&& !_player.PlayerState.IsSprinting()
            && !_player.PlayerState.IsBusyWithWall
            && _player.PlayerState.PlayerWeaponState == PlayerWeaponState.Prepared
            && weaponMount.MagazineAmmo != 0
            && !_player.PlayerWeaponsAnimator.IsReloading)
        {
            if (Time.time > _nextFireTime)
            {
                _nextFireTime = Time.time + weaponMount.Weapon.TimeBetweenBullets;

                if (weaponMount.Weapon.WithFireAnimation)
                {
                    if (weaponMount.Weapon.ChargingWeapon)
                    {
                        if (_isCharging) return;

                        _isCharging = true;
                        _player.PlayerWeaponsAnimator.Charge(true);
                        //GameManager.Instance.AudioManager.PlayOneShotSound(weaponMount.Weapon.ChargingSound, 1, 0, 1, transform.position);

                        _player.PlayerWeaponsAnimator.Fire();
                    } else
                    {
                        _player.PlayerWeaponsAnimator.Fire();
                    }
                } else
                {
                    Fire(weaponMount);
                }
            }
        }

        // If we are done charging the weapon, we let go and exert the attack
        if (!GameManager.Instance.InputManager.Firing && _isCharging)
        {
            _isCharging = false;
            _player.PlayerWeaponsAnimator.Charge(false);
        }
    }

    public virtual void Fire(WeaponMount weaponMount)
    {
        // We can't fire when we are taking a hit
        if (_player.TakingHit) return;

        if (weaponMount.MuzzleFlash != null)
        {
            weaponMount.MuzzleFlash.gameObject.SetActive(true);
            weaponMount.MuzzleFlash.Play();
        }

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 1));
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, weaponMount.Weapon.Range, _layerMask);

        int closestHitIndex = -1;
        if (hitInfos.Length > 0)
        {
            for (int i = 0; i < hitInfos.Length; i++)
            {
                RaycastHit hitInfo = hitInfos[i];

                // We make sure we don't hit something behind us
                if (Vector3.Angle(hitInfo.transform.position - _player.transform.position, _player.transform.forward) > 180)
                {
                    continue;
                }

                if (closestHitIndex == -1) closestHitIndex = i;
                else
                {
                    float thisDistance = (hitInfo.point - transform.position).magnitude;
                    float closestDistance = (hitInfos[closestHitIndex].point - transform.position).magnitude;
                    if (thisDistance < closestDistance)
                    {
                        closestHitIndex = i;
                    }
                }
            }

            if (closestHitIndex != -1)
            {
                RaycastHit hitInfo = hitInfos[closestHitIndex];

                HitSomething(hitInfo, weaponMount);
            }
        }

        // If we didn't hit anything but the weapon uses a projectile, we still want to instantiate the projectile
        if (closestHitIndex == -1 && weaponMount.Weapon.Projectile != null)
        {
            InstantiateProjectile(weaponMount, Camera.main.transform.forward);
        }

        GameManager.Instance.AudioManager.PlayOneShotSound(weaponMount.Weapon.FireSound, 1, 0, 1, transform.position);

        weaponMount.MagazineAmmo -= 1;
        // We don't want to reload when we have a bow equipped because the bow is automatically going to reload after playing the fire animation (for smoother movements of the player)
        if (weaponMount.MagazineAmmo == 0 && weaponMount.Ammo > 0 && !_player.PlayerWeaponsAnimator.PreparedBow)
        {
            _player.PlayerWeaponsAnimator.Reload();
        }
    }

    private void HitSomething(RaycastHit hitInfo, WeaponMount weaponMount)
    {
        if (weaponMount.Weapon.Projectile != null)
        {
            InstantiateProjectile(weaponMount, (hitInfo.point - weaponMount.FirePosition.position).normalized);
        }
        else
        {
            Health health = hitInfo.transform.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(weaponMount.Weapon.Damage, _player.gameObject, weaponMount.Weapon.DamageParticleType);
            }
            else
            {
                GlassBreaker glassBreaker = hitInfo.transform.GetComponent<GlassBreaker>();
                if (glassBreaker != null)
                {
                    glassBreaker.BreakerGlass();
                } else
                {
                    Rigidbody rigidbody = hitInfo.transform.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.AddForce(_player.transform.forward * weaponMount.Weapon.PushForce);
                    }
                    else
                    {
                        GameObject tmp = Instantiate(weaponMount.Weapon.BulletHole, hitInfo.point + hitInfo.normal * .01f, Quaternion.identity);
                        tmp.transform.forward = -hitInfo.normal;
                        tmp.transform.parent = hitInfo.transform;
                    }
                }
            }
            
        }
    }

    private void InstantiateProjectile(WeaponMount weaponMount, Vector3 direction)
    {
        Projectile projectileTmp = Instantiate(weaponMount.Weapon.Projectile, weaponMount.FirePosition.position, Quaternion.identity);
        projectileTmp.Initialize(gameObject, direction);
    }
}
