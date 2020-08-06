using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBox : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _playList = new List<AudioClip>();

    private AudioSource _audioSource = null;

    private float _nextMusicTime = 0f;
    private int _currentPlayingTrack = -1;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Time.time > _nextMusicTime)
        {
            _currentPlayingTrack++;
            if (_currentPlayingTrack >= _playList.Count) _currentPlayingTrack = 0;

            _audioSource.clip = _playList[_currentPlayingTrack];
            _nextMusicTime = Time.time + _playList[_currentPlayingTrack].length;

            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
    }
}
