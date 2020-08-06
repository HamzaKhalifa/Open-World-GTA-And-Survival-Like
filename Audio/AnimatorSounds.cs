using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSounds : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _sounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _smallJumpVoices = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _jumpVoices = new List<AudioClip>();
    [SerializeField] private AudioClip _jumpSound = null;
    [SerializeField] private List<AudioClip> _landVoices = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _heroicLandVoices = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _pistolReloadSounds = new List<AudioClip>();
    [SerializeField] private AudioClip _bowChargeSound = null;
    [SerializeField] private AudioClip _fillFuelSound = null;

    [SerializeField] private AudioClip _handsClimbSound = null;
    [SerializeField] private AudioClip _footClimbSound = null;

    public void PlaySound(int index)
    {
        GameManager.Instance.AudioManager.PlayOneShotSound(_sounds[index], 1, 0, 1);
    }

    public void PlaySmallJumpVoice()
    {
        AudioClip clip = _smallJumpVoices[Random.Range(0, _smallJumpVoices.Count)];
        if (clip != null)
        {
            GameManager.Instance.AudioManager.PlayOneShotSound(clip, 1, 0, 1);
        }
    }

    public void PlayJumpVoice()
    {
        AudioClip clip = _jumpVoices[Random.Range(0, _jumpVoices.Count)];
        if (clip != null)
        {
            GameManager.Instance.AudioManager.PlayOneShotSound(clip, 1, 0, 1);
        }
    }

    public void PlayJumpSound()
    {
        GameManager.Instance.AudioManager.PlayOneShotSound(_jumpSound, 1, 0, 1);
    }

    public void PlayLandVoice()
    {
        AudioClip clip = _landVoices[Random.Range(0, _landVoices.Count)];
        if (clip != null)
        {
            GameManager.Instance.AudioManager.PlayOneShotSound(clip, 1, 0, 1);
        }
    }

    public void PlayHeroicLandVoice()
    {
        AudioClip clip = _heroicLandVoices[Random.Range(0, _heroicLandVoices.Count)];
        if (clip != null)
        {
            GameManager.Instance.AudioManager.PlayOneShotSound(clip, 1, 0, 1);
        }
    }

    public void PlayHandClimbSound()
    {
        GameManager.Instance.AudioManager.PlayOneShotSound(_handsClimbSound, 1, 0, 1);
    }

    public void PlayFootClimbSound()
    {
        GameManager.Instance.AudioManager.PlayOneShotSound(_footClimbSound, 1, 0, 1);
    }

    public void PlayPistolReloadSound(int index)
    {
        GameManager.Instance.AudioManager.PlayOneShotSound(_pistolReloadSounds[index], 1, 0, 1);
    }

    public void PlayBowChargeSound()
    {
        GameManager.Instance.AudioManager.PlayOneShotSound(_bowChargeSound, 1, 0, 1);
    }


    public void PlayFillingFuelSound()
    {
        GameManager.Instance.AudioManager.PlayOneShotSound(_fillFuelSound, 1, 0, 1);
    }

    public void PlayObjectSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = _sounds[0];
        audioSource.time = 0f;
        audioSource.Play();
    }
}
