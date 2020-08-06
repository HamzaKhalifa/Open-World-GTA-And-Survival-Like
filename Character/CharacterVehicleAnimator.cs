using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVehicleAnimator : MonoBehaviour
{
    private Animator _animator = null;
    private Character _character = null;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _character = GetComponentInParent<Character>();
    }

    public void GetIntoCarAnimation(bool driverSeat)
    {
        if (driverSeat)
            _animator.SetBool("IsDriving", true);
        else
            _animator.SetBool("IsPassenger", true);
    }

    public void ExitCarAnimation()
    {
        _animator.SetBool("IsDriving", false);
        _animator.SetBool("IsPassenger", false);
    }

    public void OpenVehicleDoorAnimationEvent()
    {
        _character.CharacterVehicle.InteractiveVehicle.OpenDoor(_character.transform);
    }

    public void TranslateToSeatAnimationEvent()
    {
        _character.CharacterVehicle.TranslateToSeat();
    }

    public void CloseVehicleDoorAnimationEvent()
    {
        _character.CharacterVehicle.InteractiveVehicle.CloseDoor(_character.transform);
    }

    public void TurnOnEngineAnimationEvent()
    {
        _character.CharacterVehicle.InteractiveVehicle.AssignDriver(_character);
        _character.CharacterVehicle.InteractingWithVehicle = false;
    }

    public void FinishedInteractingWithVehicleAnimationEvent()
    {
        _character.CharacterVehicle.InteractingWithVehicle = false;
    }

    public void TranslateToOutAnimationEvent()
    {
        _character.CharacterVehicle.TranslateToOut(_character.CharacterVehicle.InteractiveVehicle);
    }

    public void RegainControlBehavior()
    {
        _character.CharacterVehicle.InteractingWithVehicle = false;
        _character.CharacterVehicle.transform.parent = null;
        _character.CharacterVehicle.InteractiveVehicle = null;
        _character.CharacterVehicle.RegainControl();
    }
}
