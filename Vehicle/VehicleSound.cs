using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleSound : MonoBehaviour
{
    [SerializeField] private AudioSource _engineAudioSource = null;
    [SerializeField] private AudioClip _turnOnEngineSound = null;
    [SerializeField] private AudioClip _turnOffEngineSound = null;

    private Vehicle _vehicle = null;
    private float _pitchBonus = 0f;

    private void Awake()
    {
        _vehicle = GetComponent<Vehicle>();

        _vehicle.OnEngineTurnedOn += () =>
        {
            GameManager.Instance.AudioManager.PlayOneShotSound(_turnOnEngineSound, 1, 0, 1, transform.position);
        };

        _vehicle.OnEngineTurnedOff += () =>
        {
            GameManager.Instance.AudioManager.PlayOneShotSound(_turnOffEngineSound, 1, 1, 1, transform.position);
        };
    }

    private void Update()
    {
        if (!_vehicle.IsBeingDriven)
        {
            if (_engineAudioSource.isPlaying) _engineAudioSource.Stop();

            return;
        }

        if (_engineAudioSource.clip != _vehicle.EngineSound)
        {
            _engineAudioSource.clip = _vehicle.EngineSound;
        }

        HandleEnginePitch();
    }

    private void HandleEnginePitch()
    {
        if (!_engineAudioSource.isPlaying)
        {
            _engineAudioSource.Play();
        }

        // Do no add pitch bonus when the vehicle isn't accelerating (vertical > .1) or when it's braking
        float pitchBonus = (_vehicle.IsBraking || Mathf.Abs(_vehicle.Verticle) <= .1) ? 0 : _vehicle.NormalizedSpeed;
        _pitchBonus = Mathf.Lerp(_pitchBonus, pitchBonus, 5f * Time.deltaTime);

        _engineAudioSource.pitch = 1 + _pitchBonus;
    }
}
