using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterVehicle : MonoBehaviour
{
    [SerializeField] protected Vehicle _drivenVehicle = null;
    
    protected Vehicle _enteredVehicle = null;
    protected InteractiveVehicle _interactiveVehicle = null;
    protected bool _interactingWithVehicle = false;

    #region Public Accessors

    public Vehicle DrivenVehicle => _drivenVehicle;
    public Vehicle EnteredVehicle => _enteredVehicle;

    // This stores the vehicle's door
    public InteractiveVehicle InteractiveVehicle { get { return _interactiveVehicle; } set { _interactiveVehicle = value; } }
    public bool IsDriving => _drivenVehicle != null && _drivenVehicle.Driver != null && !_interactingWithVehicle;
    public bool IsPassenger => _drivenVehicle == null && _enteredVehicle != null && !_interactingWithVehicle;
    public bool InteractingWithVehicle { get { return _interactingWithVehicle; } set { _interactingWithVehicle = value; } }
    // To know when we can exit the car
    public bool IsInsideVehicle => _enteredVehicle != null && !InteractingWithVehicle;
    // To not allow the player from moving when we are busy doing something with a vehicle
    public bool BusyWithVehicle => _enteredVehicle != null || _interactingWithVehicle;

    #endregion

    #region Cache Fields

    private CharacterVehicleAnimator _characterVehicleAnimator = null;
    public CharacterVehicleAnimator CharacterVehicleAnimator => _characterVehicleAnimator;

    #endregion

    protected virtual void Awake()
    {
        _characterVehicleAnimator = GetComponentInChildren<CharacterVehicleAnimator>();

        // The entered vehicle is always the same as the driven vehicle
        _enteredVehicle = _drivenVehicle;
    }

    public virtual void GetIntoVehicle(bool driverSeat, Vehicle vehicle, InteractiveVehicle interactiveVehicle)
    {
        _interactingWithVehicle = true;
        _enteredVehicle = vehicle;
        _interactiveVehicle = interactiveVehicle;
        _characterVehicleAnimator.GetIntoCarAnimation(driverSeat);

        if (driverSeat)
        {
            _drivenVehicle = vehicle;
            // If we are a driver, we resest the vehicle's RPM
            vehicle.ResetRPM();
        }

        // Deactive character controller for player, or navmesh for AI
        LoseControl();

        // Putting the character right in front of the door
        StartCoroutine(GoToPointCoroutine(interactiveVehicle.StartAnimationPoint.position, interactiveVehicle.StartAnimationPoint.rotation, 1));

        _interactiveVehicle.Occupied = true;
    }

    public virtual void ExitVehicle()
    {
        _interactingWithVehicle = true;

        if (_drivenVehicle != null)
        {
            // By setting the driver to null, we also play the turn off engine 
            _drivenVehicle.Driver = null;
        }

        _enteredVehicle = null;
        _drivenVehicle = null;

        _characterVehicleAnimator.ExitCarAnimation();

        _interactiveVehicle.Occupied = false;
    }

    public void TranslateToSeat()
    {
        transform.parent = _interactiveVehicle.Seat;
        StartCoroutine(GoToPointCoroutine(Vector3.zero, Quaternion.identity, 1, true));
    }

    public void TranslateToOut(InteractiveVehicle interactiveVehicle)
    {
        StartCoroutine(GoToPointCoroutine(interactiveVehicle.StartAnimationPoint.position, interactiveVehicle.StartAnimationPoint.rotation, 1));
    }

    // Deactive/Activate character controller for player, or navmesh for AI
    abstract protected void LoseControl();
    abstract public void RegainControl();

    private IEnumerator GoToPointCoroutine(Vector3 destination, Quaternion destinationRotation, float delay, bool local = false)
    {
        float time = 0f;

        Vector3 initialPosition = local ? transform.localPosition : transform.position;
        Quaternion initialRotation = local ? transform.localRotation : transform.rotation;

        while (time <= delay)
        {
            time += Time.deltaTime;
            float normalizedTime = time / delay;

            if (local)
            {
                transform.localPosition = Vector3.Lerp(initialPosition, destination, normalizedTime);
                transform.localRotation = Quaternion.Lerp(initialRotation, destinationRotation, normalizedTime);
            }
            else {
                transform.position = Vector3.Lerp(initialPosition, destination, normalizedTime);
                transform.rotation = Quaternion.Lerp(initialRotation, destinationRotation, normalizedTime);
            }

            yield return null;
        }

        if (local)
        {
            transform.localPosition = destination;
            transform.localRotation = destinationRotation;
        } else {
            transform.position = destination;
            transform.rotation = destinationRotation;
        }
    }

}
