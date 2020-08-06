using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttackTrigger : MonoBehaviour
{
    [SerializeField] private float _damageDelay = .5f;

    #region Cache Fields

    private Player _player = null;

    #endregion

    private float _timer = 0f;

    #region Monobehavior Callbacks

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType != WeaponType.Melee) return;

        if (_timer >= _damageDelay && other.CompareTag("EnemyBodyPart"))
        {
            // Exert damage here
            Health health = other.GetComponent<Health>();
            health.TakeDamage(_player.PlayerWeapons.EquippedWeaponMount.Weapon.Damage, _player.gameObject, _player.PlayerWeapons.EquippedWeaponMount.Weapon.DamageParticleType, transform);

            _timer = 0f;
        }
    }

    private void OnEnable()
    {
        _timer = _damageDelay;
    }

    #endregion
}
