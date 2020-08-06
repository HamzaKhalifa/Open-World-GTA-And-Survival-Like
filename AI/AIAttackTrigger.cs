using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttackTrigger : MonoBehaviour
{
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _damageDelay = .5f;
    [SerializeField] private ParticleType _damageParticleType = ParticleType.Blood;

    #region Cache Fields

    private AIStateMachine _stateMachine = null;

    #endregion

    private float _timer = 0f;

    #region Monobehavior Callbacks

    private void Awake()
    {
        _stateMachine = GetComponentInParent<AIStateMachine>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        // The enemy shouldn't hitting himself (testing this twice: here and in the TakeDamage function because when we collide with our own body part here, the timer is going to be reset to 0
        if (other.CompareTag("EnemyBodyPart"))
        {
            if (_stateMachine.gameObject == other.GetComponentInParent<AIStateMachine>().gameObject) return;
        }

        if (_timer >= _damageDelay && (other.CompareTag("Player") || other.CompareTag("EnemyBodyPart")))
        {
            // Exert damage here
            Health health = other.GetComponent<Health>();
            health.TakeDamage(_damage, _stateMachine.gameObject, _damageParticleType, transform);

            _timer = 0f;
        }
    }

    private void OnEnable()
    {
        _timer = _damageDelay;
    }

    #endregion
}
