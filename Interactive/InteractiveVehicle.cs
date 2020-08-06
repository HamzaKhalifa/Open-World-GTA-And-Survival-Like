using UnityEngine;

[RequireComponent(typeof(Animator))]
public class InteractiveVehicle : InteractiveDoor
{
    [SerializeField] private bool _driverSeat = true;
    [SerializeField] private Transform _startAnimationPoint = null;
    [SerializeField] private Transform _seat = null;

    #region Cache Fields

    private Vehicle _vehicle = null;

    #endregion

    public Transform StartAnimationPoint => _startAnimationPoint;
    public Transform Seat => _seat;
    public bool Occupied { get { return _occupied; } set { _occupied = value; } }
    public bool DriverSeat => _driverSeat;

    private bool _occupied = false;

    protected override void Awake()
    {
        base.Awake();

        _vehicle = GetComponentInParent<Vehicle>();
    }

    public override void Interact(Transform interactor)
    {
        if (_vehicle.IsReversed || _occupied) return;

        _occupied = true;

        Character character = interactor.GetComponent<Character>();
        if (character != null)
        {
            character.GetIntoVehicle(_driverSeat, _vehicle, this);
        }
    }

    public void AssignDriver(Character character)
    {
        _vehicle.Driver = character;
    }

    public void OpenDoor(Transform interactor)
    {
        // We set is closed to true so that it is set back to false when we call the interact function
        _isClosed = true;
        base.Interact(interactor);
    }

    public void CloseDoor(Transform interactor)
    {
        _isClosed = false;
        base.Interact(interactor);
    }
}
