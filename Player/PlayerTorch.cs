using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTorch : MonoBehaviour
{
    [SerializeField] private Light _torchLight = null;
    [SerializeField] private Material _torchMaterial = null;
    [SerializeField] private AudioClip _toggleSound = null;

    private bool _activated = true;

    private void Awake()
    {
        ToggleTorch(false);
    }

    private void Update()
    {
        if (GameManager.Instance.InputManager.ToggleTorch)
        {
            ToggleTorch();
        }
    }

    private void ToggleTorch(bool playSound = true)
    {
        _activated = !_activated;

        _torchLight.gameObject.SetActive(_activated);

        if (_activated)
            _torchMaterial.EnableKeyword("_EMISSION");
        else
            _torchMaterial.DisableKeyword("_EMISSION");

        if (playSound)
            GameManager.Instance.AudioManager.PlayOneShotSound(_toggleSound, 1, 0, 1, transform.position);
    }
}
