using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterVehicle _characterVehicle = null;
    public CharacterVehicle CharacterVehicle => _characterVehicle;

    protected bool _takingHit = false;

    public bool TakingHit { set { _takingHit = value; } get { return _takingHit; } }

    protected Animator _animator = null;
    public Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                if (_animator == null)
                    _animator = GetComponentInChildren<Animator>();
            }

            return _animator;
        }
    }

    protected virtual void Awake()
    {
        _characterVehicle = GetComponent<CharacterVehicle>();
    }

    public void GetIntoVehicle(bool driverSeat, Vehicle vehicle, InteractiveVehicle interactiveVehicle)
    {
        _characterVehicle.GetIntoVehicle(driverSeat, vehicle, interactiveVehicle);
    }
}
