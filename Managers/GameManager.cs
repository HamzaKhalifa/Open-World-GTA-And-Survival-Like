using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField] private EnviroSkyMgr _enviroSkyManager = null;

    #region Cache Fields

    public EnviroSkyMgr EnviroSkyManager => _enviroSkyManager;

    public bool IsNight
    {
        get
        {
            if (_enviroSkyManager == null || !_enviroSkyManager.gameObject.activeSelf) return false;
            else return _enviroSkyManager.IsNight();
        }
    }

    private InputManager _inputManager = null;
    public InputManager InputManager
    {
        get
        {
            if (_inputManager == null)
                _inputManager = GetComponent<InputManager>();

            return _inputManager;
        }
    }

    private AudioManager _audioManager = null;
    public AudioManager AudioManager
    {
        get
        {
            if (_audioManager == null)
                _audioManager = GetComponent<AudioManager>();

            return _audioManager;
        }
    }

    private PlayerCamera _playerCamera = null;
    public PlayerCamera PlayerCamera
    {
        get
        {
            if (_playerCamera == null)
                _playerCamera = FindObjectOfType<PlayerCamera>();

            return _playerCamera;
        }
    }

    private ParticlesManager _particlesManager = null;
    public ParticlesManager ParticlesManager
    {
        get
        {
            if (_particlesManager == null)
                _particlesManager = GetComponent<ParticlesManager>();

            return _particlesManager;
        }
    }

    private Player _player = null;
    public Player Player { get { return _player; } set { _player = value; } }

    #endregion

    private void Awake()
    {
        Instance = this;
    }
}
