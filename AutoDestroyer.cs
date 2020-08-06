using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyer : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 2f;
    [SerializeField] private AudioClip _appearSound = null;

    // Start is called before the first frame update
    void Start()
    {
        if (_appearSound != null)
            GameManager.Instance.AudioManager.PlayOneShotSound(_appearSound, 1, 0, 1);

        Invoke("Kill", _destroyDelay);
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
