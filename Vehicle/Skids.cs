using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skids : MonoBehaviour
{
    [System.Serializable]
    public class WheelSkidMarks
    {
        public WheelCollider WheelCollider = null;
        public TrailRenderer TrailRenderer = null;
    }

    [SerializeField] private List<WheelSkidMarks> _wheelsSkidMarks = new List<WheelSkidMarks>();
    [SerializeField] private float _skidThreshold = .7f;
    [SerializeField] private AudioSource _skidMarkAudioSource = null;
    [SerializeField] private float _minimumSkidSoundPlayingTime = .3f;
 
    private Vehicle _vehicle = null;

    private float _timeSpentOnPlayingSkidSound = 0f;

    private void Awake()
    {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Update()
    {
        bool playSkidAudio = false;

        foreach(WheelSkidMarks wheelSkidMarks in _wheelsSkidMarks)
        {
            WheelHit wheelHit;
            bool emit = false;
            if (wheelSkidMarks.WheelCollider.GetGroundHit(out wheelHit))
            {
                emit = Mathf.Abs(wheelHit.forwardSlip) > _skidThreshold || Mathf.Abs(wheelHit.sidewaysSlip) > _skidThreshold;
            }

            wheelSkidMarks.TrailRenderer.emitting = emit;

            if (emit && _vehicle.IsBeingDriven)
            {
                playSkidAudio = true;
            }
        }

        if (_skidMarkAudioSource.isPlaying)
        {
            _timeSpentOnPlayingSkidSound += Time.deltaTime;
        }

        if (playSkidAudio && !_skidMarkAudioSource.isPlaying)
        {
            _skidMarkAudioSource.Play();
        } else if(!playSkidAudio && _skidMarkAudioSource.isPlaying && _timeSpentOnPlayingSkidSound >= _minimumSkidSoundPlayingTime)
        {
            _timeSpentOnPlayingSkidSound = 0f;
            _skidMarkAudioSource.Stop();
        }
    }
}
