using UnityEngine;

public class PlayerWeaponMeleeAttackAnimator : MonoBehaviour
{
    private Player _player = null;
    private Animator _animator = null;

    private bool _isAttacking = false;

    private bool _canAttack = true;

    public bool CanAttack { set { _canAttack = value; } }
    
    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_player.PlayerState.PlayerWeaponState == PlayerWeaponState.Unprepared || _player.PlayerWeapons.EquippedWeaponMount == null || _player.PlayerWeapons.EquippedWeaponMount.Weapon.WeaponType != WeaponType.Melee) return;

        if (_canAttack && GameManager.Instance.InputManager.Attack)
        {
            _animator.SetTrigger("MeleeAttack");
        }

        if (_canAttack && GameManager.Instance.InputManager.HeavyAttack)
        {
            _animator.SetTrigger("MeleeHeavyAttack");
        }

        TrailRenderer trailRenderer = _player.PlayerWeapons.EquippedWeaponMount.MeleeTrailRenderer;
        if ((_isAttacking && !trailRenderer.gameObject.activeSelf) || (!_isAttacking && trailRenderer.gameObject.activeSelf))
        {
            trailRenderer.gameObject.SetActive(!trailRenderer.gameObject.activeSelf);
        }
    }

    #region For can attack

    public void SetCanAttackAnimationBehavior()
    {
        _canAttack = true;
    }

    public void CantAttackAnimationBehavior()
    {
        _canAttack = false;
    }

    #endregion

    public void AttackAnimationEvent()
    {
        _isAttacking = true;

        AudioClip clip = _player.PlayerWeapons.EquippedWeaponMount.Weapon.MeleeSound;
        GameManager.Instance.AudioManager.PlayOneShotSound(clip, 1, 0, 1, transform.position);

        GameObject attackTrigger = _player.PlayerWeapons.EquippedWeaponMount.MeleeAttackTrigger;
        attackTrigger.SetActive(true);
    }

    public void EndAttackAnimationEvent()
    {
        _isAttacking = false;

        GameObject attackTrigger = _player.PlayerWeapons.EquippedWeaponMount.MeleeAttackTrigger;
        attackTrigger.SetActive(false);
    }

}
