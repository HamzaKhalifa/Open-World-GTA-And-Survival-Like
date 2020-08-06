using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SickscoreGames.HUDNavigationSystem;

public enum VehicleType
{
    Car,
    Helicopter,
}

[System.Serializable]
public class RPM
{
    public float MaxSpeed = 15f;
    public float MinSpeed = 0f;
    public AudioClip EngineSound = null;
}

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private VehicleType _vehicleType = VehicleType.Car;
    [SerializeField] private float _torque = 10f;
    [SerializeField] private float _maxSteering = 50f;
    [SerializeField] private float _steeringSensitivity = 10f;
    [SerializeField] private float _brakeForce = 100f;
    [SerializeField] private float _timeBetweenRPMs = .5f;
    [SerializeField] private List<RPM> _rpms = new List<RPM>();
    [SerializeField] private float _aiMaxSpeed = 10f;
    [SerializeField] private float _fuel = 1f;
    [SerializeField] private float _fuelDepletionRate = 10f;

    [Header("Inspector Dependencies")]
    [SerializeField] private List<WheelCollider> _wheelColliders = new List<WheelCollider>();

    public VehicleType VehicleType => _vehicleType;
    public AudioClip EngineSound => _currentRPM.EngineSound;
    public float Speed => _rigidBody.velocity.magnitude;
    public float NormalizedSpeed => (_rigidBody.velocity.magnitude - _currentRPM.MinSpeed) / (_currentRPM.MaxSpeed - _currentRPM.MinSpeed);
    public bool IsStopped => _rigidBody.velocity.magnitude <= .5f;
    public bool ReverseGear => IsBeingDriven && GameManager.Instance.InputManager.Vertical < 0;
    public bool IsBraking => _isActiveBraking
        || (!IsStopped && Vector3.Angle(transform.forward, _rigidBody.velocity) > 120 && GameManager.Instance.InputManager.Vertical > .1f
        || (!IsStopped && Vector3.Angle(-transform.forward, _rigidBody.velocity) > 120 && GameManager.Instance.InputManager.Vertical < -.1f));


    public System.Action OnEngineTurnedOn = null;
    public System.Action OnEngineTurnedOff = null;
    public Character Driver { set {
            _driver = value;
            if (value == null)
            {
                _verticle = 0f;
                OnEngineTurnedOff();
                _rigidBody.isKinematic = true;
                GetComponent<HUDNavigationElement>().enabled = true;
            }
            else
            {
                OnEngineTurnedOn();
                _rigidBody.isKinematic = false;
                GetComponent<HUDNavigationElement>().enabled = false;
            }
        }

        get { return _driver; }
    }

    public bool IsBeingDriven => _driver != null;
    public bool IsReversed => Vector3.Angle(transform.up, Vector3.up) > 120;
    public float Verticle => _verticle;
    public float Fuel => _fuel;

    #region Cache Fields

    private Rigidbody _rigidBody = null;

    #endregion

    private bool _isActiveBraking = false;
    private Character _driver = null;
    private RPM _currentRPM = null;
    private int _currentRPMIndex = 0;
    private float _timeSpentOnMaxSpeed = 0f;
    private float _horizontal = 0f;
    private float _verticle = 0f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _currentRPM = _rpms[_currentRPMIndex];
    }

    private void Update()
    {
        if (_driver == null) return;

        HandleMaxSpeed();
    }

    #region Driver Control Methods

    public void HandleMove(float vertical)
    {
        if (_fuel <= 0) return;

        _verticle = vertical;
        foreach (WheelCollider wheelCollider in _wheelColliders)
        {
            float torque = _torque * vertical;
            if (IsBraking)
                torque = 0f;

            wheelCollider.motorTorque = torque;
        }

        if (_driver != null)
        {
            _fuel -= Mathf.Abs(_verticle) * Time.deltaTime * _fuelDepletionRate;
            _fuel = Mathf.Max(_fuel, 0);
        }
    }

    public void HandleSteering(float horizonal)
    {
        _horizontal = Mathf.Lerp(_horizontal, horizonal, Time.deltaTime * _steeringSensitivity);

        _wheelColliders[0].steerAngle = _maxSteering * _horizontal;
        _wheelColliders[1].steerAngle = _maxSteering * _horizontal;
    }

    public void HandleBrake(bool isActiveBraking)
    {
        _isActiveBraking = isActiveBraking;
        foreach (WheelCollider wheelCollider in _wheelColliders)
        {
            if (IsBraking)
            {
                wheelCollider.brakeTorque = _brakeForce;
                wheelCollider.motorTorque = 0f;
            }
            else
            {
                wheelCollider.brakeTorque = 0f;
            }
        }
    }

    #endregion

    public void ResetRPM()
    {
        _currentRPMIndex = 0;
        _currentRPM = _rpms[_currentRPMIndex];
    }

    public void RefillFuel()
    {
        _fuel = 1;
    }

    #region Helper Methods

    private void HandleMaxSpeed()
    {
        // If the driver is an AI agent, then the max speed is set to much lower
        if (_driver != GameManager.Instance.Player && _rigidBody.velocity.magnitude >= _aiMaxSpeed)
        {
            _rigidBody.velocity = _rigidBody.velocity.normalized * _aiMaxSpeed;
            return;
        }


        if (_rigidBody.velocity.magnitude >= _currentRPM.MaxSpeed)
        {
            _timeSpentOnMaxSpeed += Time.deltaTime;
            _rigidBody.velocity = _rigidBody.velocity.normalized * _currentRPM.MaxSpeed;

            if (_timeSpentOnMaxSpeed > _timeBetweenRPMs)
            {
                _timeSpentOnMaxSpeed = 0;
                SwitchRpm(1);
            }
        } else
        {
            _timeSpentOnMaxSpeed = 0f;
        }

        if (_rigidBody.velocity.magnitude < _currentRPM.MinSpeed)
        {
            SwitchRpm(-1);
        }
    }

    private void SwitchRpm(int howMuch)
    {
        _currentRPMIndex += howMuch;
        _currentRPMIndex = Mathf.Max(0, _currentRPMIndex);
        _currentRPMIndex = Mathf.Min(_currentRPMIndex, _rpms.Count - 1);

        _currentRPM = _rpms[_currentRPMIndex];
    }

    #endregion
}
