using UnityEngine;

public class BodyPartHealth : Health
{
    [SerializeField] private Health _parentHealth = null;

    public override void TakeDamage(float damage, GameObject attacker, ParticleType particleType, Transform hitSource = null)
    {
        base.TakeDamage(damage, attacker, particleType, hitSource);

        _parentHealth.TakeDamage(damage, attacker);
    }

    protected override void Die()
    {
        base.Die();
    }
}
