using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private float _length = .5f;
    [SerializeField] private float _rotationSpeed = 30f;
    [SerializeField] private AudioClip _pickupSound = null;

    private float _minPos = 0f;

    #region Monobehavior Callbacks

    private void Awake()
    {
        _minPos = transform.position.y - _length / 2;
    }

    protected virtual void Update()
    {
        if (_length != 0)
            transform.position = new Vector3(transform.position.x, _minPos + Mathf.PingPong(Time.time, _length), transform.position.z);
        transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.AudioManager.PlayOneShotSound(_pickupSound, 1, 0, 1, transform.position);

        Pickup(other.transform);

        Destroy(gameObject);
    }

    #endregion

    protected virtual void Pickup(Transform player)
    {
    }
}
