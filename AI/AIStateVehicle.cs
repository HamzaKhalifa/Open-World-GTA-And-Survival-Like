using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterVehicle))]
[RequireComponent(typeof(CharacterVehicleAnimator))]
public class AIStateVehicle : AIState
{
    [SerializeField] private bool _wantToDrive = false;
    [SerializeField] private bool _wantToBePassenger = false;
    [SerializeField] private List<Transform> _navigationPoints = new List<Transform>();

    public bool WantToDrive => _wantToDrive;
    public bool WantToBePassenger => _wantToBePassenger;

    #region Cache Fields

    private CharacterVehicle _characterVehicle = null;

    #endregion

    private InteractiveVehicle _interactiveVehicle = null;
    private int _currentNavigationPoint = 0;
    private float _vertical = 0f;
    private float _horizontal = 0f;
    private bool _canDrive = false;

    private void Awake()
    {
        _characterVehicle = GetComponent<CharacterVehicle>();
    }

    public override AIStateType GetStateType()
    {
        return AIStateType.Vehicle;
    }

    public override void OnEnter()
    {
        // We get inside the car right after getting into the state
        _interactiveVehicle = _stateMachine.CurrentTarget.TargetTransform.GetComponent<InteractiveVehicle>();
        if (_interactiveVehicle == null)
        {
            _stateMachine.SwitchState(AIStateType.Idle);
            return;
        }

        _interactiveVehicle.Interact(_stateMachine.transform);

        _horizontal = 0f;
        _vertical = 0f;

        Invoke("SetCanDrive", 5);
    }

    public override AIStateType OnUpdate()
    {
        if (_stateMachine.HasHighPriorityTarget)
        {
            return AIStateType.Alert;
        }

        if (!_wantToDrive && _stateMachine.AIVehicle.DrivenVehicle != null)
        {
            return AIStateType.Idle;
        }

        if (!_wantToBePassenger && _stateMachine.AIVehicle.DrivenVehicle == null && _stateMachine.AIVehicle.EnteredVehicle != null)
        {
            return AIStateType.Idle;
        }

        if (_stateMachine.AIVehicle.EnteredVehicle == null) return AIStateType.Idle;

        if (_interactiveVehicle.DriverSeat && _canDrive)
        {
            // We are driving here
            if (_navigationPoints.Count > 0)
            {
                Vehicle vehicle = _stateMachine.AIVehicle.DrivenVehicle;

                // Handle steering
                float _nextHorizontal = 0f;
                float angle = Vector3.Angle(vehicle.transform.forward, _navigationPoints[_currentNavigationPoint].transform.position - vehicle.transform.position);
                if (angle > 10f)
                {
                    float sign = Mathf.Sign(Vector3.Cross(vehicle.transform.forward, _navigationPoints[_currentNavigationPoint].transform.position - vehicle.transform.position).y);
                    _nextHorizontal = sign;
                }
                _horizontal = Mathf.Lerp(_horizontal, _nextHorizontal, Time.deltaTime * 10f);
                vehicle.HandleSteering(_horizontal);

                // Handle moving
                float distance = (_navigationPoints[_currentNavigationPoint].transform.position - vehicle.transform.position).magnitude;

                float nextVertical = 0f;
                if (distance > 2)
                {
                    float minimumVertical = .2f;
                    // Reduce by angle
                    nextVertical = 1 - angle / 90;

                    // Reduce by distance
                    if (angle < 10)
                    {
                        nextVertical = distance / 60;
                        nextVertical = Mathf.Min(1, nextVertical);
                    }
                    
                    nextVertical = Mathf.Max(nextVertical, minimumVertical);
                } else
                {
                    _currentNavigationPoint++;
                    if (_currentNavigationPoint >= _navigationPoints.Count) _currentNavigationPoint = 0;
                }
                _vertical = Mathf.Lerp(_vertical, nextVertical, Time.deltaTime * 10);
                vehicle.HandleMove(_vertical);

                // Handle Braking
                vehicle.HandleBrake(angle > 30 && vehicle.Speed > 10);
            }

        }

        return AIStateType.Vehicle;
    }

    public override void OnExit()
    {
        _stateMachine.ResetTarget();
        if (_stateMachine.AIVehicle.EnteredVehicle != null)
            _stateMachine.AIVehicle.ExitVehicle();
        _canDrive = false;
    }

    private void SetCanDrive()
    {
        _canDrive = true;
    }

    public override void HandleAnimator()
    {
        throw new System.NotImplementedException();
    }
}
