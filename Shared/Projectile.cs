using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OnHitBehavior
{
    Destroy,
    GetStuck
}

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleType _damageParticleType = ParticleType.Blood;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _force = 1000f;
    [SerializeField] private float _minDamageVelocity = 1f;
    [SerializeField] private OnHitBehavior _hitBehavior = OnHitBehavior.GetStuck;
    [SerializeField] private GameObject _hitEffect = null;
    [SerializeField] private Transform _hitEffectSpawnPositionTransform = null;

    private bool _didHit = false;
    private GameObject _attacker = null;
    private Rigidbody _rigidBody = null;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_speed != 0)
            _rigidBody.velocity = transform.forward * _speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        Hit(other.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Hit(other.gameObject);
    }

    private void Hit (GameObject other)
    {
        if (_rigidBody.velocity.magnitude < _minDamageVelocity) return;

        if (_didHit) return;

        // We don't hit ourselves
        if (other == _attacker) return;

        _didHit = true;

        GlassBreaker glassBreaker = other.transform.GetComponent<GlassBreaker>();
        if (glassBreaker != null)
        {
            glassBreaker.BreakerGlass();
        }

        Health health = other.transform.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(_damage, _attacker, _damageParticleType);

        if (_hitEffect != null)
        {
            Instantiate(_hitEffect, _hitEffectSpawnPositionTransform.position, Quaternion.identity);
        }

        switch (_hitBehavior)
        {
            case OnHitBehavior.Destroy:
                Destroy(gameObject);
                break;
            case OnHitBehavior.GetStuck:
                if (health == null)
                {
                    _rigidBody.isKinematic = true;
                    GetComponent<Collider>().enabled = false;
                }

                transform.parent = other.transform;
                TrailRenderer trailRenderer = GetComponent<TrailRenderer>();
                if (trailRenderer != null) trailRenderer.enabled = false;
                break;
            default:
                break;
        }
    }

    public void Initialize(GameObject attacker, Vector3 forward)
    {
        _attacker = attacker;
        transform.forward = forward;

        _rigidBody.AddForce(forward * _force);
    }
}
