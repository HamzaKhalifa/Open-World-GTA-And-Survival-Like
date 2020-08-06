using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Footsteps
{
    public Shoes Shoes = Shoes.Barefoot;
    public List<AudioClip> Default = new List<AudioClip>();
    public List<AudioClip> Metal = new List<AudioClip>();
    public List<AudioClip> Wood = new List<AudioClip>();
    public List<AudioClip> Grass = new List<AudioClip>();
    public List<AudioClip> Gravel = new List<AudioClip>();

    public Dictionary<int, List<AudioClip>> LayerClips = new Dictionary<int, List<AudioClip>>();
}

public class FootstepsSound : MonoBehaviour
{
    [SerializeField] List<Footsteps> _footsteps = new List<Footsteps>();
    [SerializeField] private float _timeBetweenSteps = .2f;
    [SerializeField] private float _ninjaRunTimeBetweenSteps = .01f;
    [SerializeField] private Transform _rayOrigin = null;


    #region Cache Fields

    private Player _player = null;

    #endregion

    private Footsteps _currentFootsteps = null;
    private List<AudioClip> _currentClips = new List<AudioClip>();
    private float _lastStepTime = 0f;
    private Dictionary<Shoes, Footsteps> _footstepsDictionary = new Dictionary<Shoes, Footsteps>();

    #region Monobehavior Callbacks

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void Start()
    {
        foreach (Footsteps footsteps in _footsteps)
        {
            footsteps.LayerClips.Add(LayerMask.NameToLayer("Default"), footsteps.Default);
            footsteps.LayerClips.Add(LayerMask.NameToLayer("Metal"), footsteps.Metal);
            footsteps.LayerClips.Add(LayerMask.NameToLayer("Wood"), footsteps.Wood);
            footsteps.LayerClips.Add(LayerMask.NameToLayer("Grass"), footsteps.Grass);
            footsteps.LayerClips.Add(LayerMask.NameToLayer("Gravel"), footsteps.Gravel);

            _footstepsDictionary.Add(footsteps.Shoes, footsteps);
        }

        if (_rayOrigin == null) _rayOrigin = transform;
    }

    private void Update()
    {
        if (_player != null)
            _footstepsDictionary.TryGetValue(_player.PlayerState.Shoes, out _currentFootsteps);
        else
            _footstepsDictionary.TryGetValue(Shoes.Shoes, out _currentFootsteps);
    }

    #endregion

    public void PlayFootstep()
    {
        // We don't play foot steps if we are grounded
        if (_player != null && !_player.PlayerMove.IsGrounded) return;

        float timeBetweenSteps = _timeBetweenSteps;
        if (_player != null)
        {
            timeBetweenSteps = _player.PlayerState.PlayerMoveState == PlayerMoveState.NinjaSprint ? _ninjaRunTimeBetweenSteps : _timeBetweenSteps;
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(_rayOrigin.position, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Default", "Metal", "Wood", "Grass", "Gravel")))
        {
            _currentFootsteps.LayerClips.TryGetValue(hitInfo.transform.gameObject.layer, out _currentClips);
        }

        if (Time.time < _lastStepTime + timeBetweenSteps || _currentClips == null || _currentClips.Count == 0) return;
        _lastStepTime = Time.time;

        AudioClip clip = _currentClips[Random.Range(0, _currentClips.Count)];
        if (clip != null)
        {
            GameManager.Instance.AudioManager.PlayOneShotSound(clip, 1, 1, 1, transform.position);
        }
    }
}
