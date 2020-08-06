using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] float _health = 100f;

    #region Cache Fields

    private Ragdoll _ragdoll = null;
    private AIStateMachine _stateMachine = null;

    #endregion

    private float _initialHealth = 100f;
    // Using this to know when we got hit out of nowhere and for the alert mode to not quickly switch back to patrol
    private bool _justGotHit = false;
    private float _resetJustGotHitTime = 0f;

    public bool IsDead => _health <= 0;
    public System.Action OnDeath = null;
    public bool JustGotHit { get { return _justGotHit; } }
    public float HealthValue => _health;

    #region Monobehavior Callbacks

    private void Awake()
    {
        _stateMachine = GetComponent<AIStateMachine>();
        _initialHealth = _health;
        _ragdoll = GetComponent<Ragdoll>();
    }

    private void Update()
    {
        if (Time.time >= _resetJustGotHitTime && _justGotHit)
        {
            _justGotHit = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            if (other.GetComponent<Rigidbody>().velocity.magnitude > 2)
            {
                TakeDamage(100, other.gameObject, ParticleType.Impact, transform);
            }
        }
    }

    #endregion

    public void RegainHealth(float amount)
    {
        _health += amount;
        _health = Mathf.Min(100, _health);
    }

    public virtual void TakeDamage(float damage, GameObject attacker = null, ParticleType particleType = ParticleType.None, Transform hitSource = null)
    {
        Character character = GetComponentInParent<Character>();
        // Nobody should be able to hit him/herself
        if (attacker == character.gameObject) return;

        if (character != null)
        {
            bool rightHit = true;
            // Figuring out whether it's a hit from the right or left
            float sign = Mathf.Sign(Vector3.Cross((transform.position - character.transform.position).normalized, character.transform.forward).y);
            if (sign < 0)
            {
                rightHit = false;
            }

            character.Animator.SetTrigger(rightHit ? "HitRight" : "HitLeft");
        }

        if (_stateMachine != null)
        {
            _stateMachine.AISCanner.AddPotentialThreat(attacker);

            // If we just took damage from nowhere (when we don't have a player or a character as target, then we look around and set the attacker as an angry at threat)
            if ((_stateMachine.CurrentTarget == null
                || (_stateMachine.CurrentTarget != null &&
                    (_stateMachine.CurrentTarget.Type != AITargetType.Enemy && _stateMachine.CurrentTarget.Type != AITargetType.Player))))
            {
                _stateMachine.CurrentTarget = null;
                _stateMachine.Agent.ResetPath();
                _stateMachine.SwitchState(AIState.AIStateType.Alert);
                _justGotHit = true;
                _resetJustGotHitTime = Time.time + 5f;
            }
        }


        _health -= damage;
        _health = Mathf.Max(0, _health);

        if (particleType != ParticleType.None)
        {
            GameManager.Instance.ParticlesManager.InstantiatePaticle(particleType, hitSource == null ? transform.position :hitSource.position);
        }

        if (_health <= 0) Die();
    }

    protected virtual void Die()
    {
        if (_health > 0) return;

        if (_ragdoll != null)
            _ragdoll.HandleRagdoll(true);

        if (_stateMachine != null)
        {
            _stateMachine.Agent.ResetPath();
        }

        if (OnDeath != null) OnDeath();
    }
}
