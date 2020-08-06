using UnityEngine;

public class InteractiveChair : InteractiveObject
{
    [SerializeField] private bool _isOccupied = false;

    public bool IsOccupied { get { return _isOccupied; } set { _isOccupied = value; } }

    public override void Interact(Transform interactor)
    {
        base.Interact(interactor);
    }
}
