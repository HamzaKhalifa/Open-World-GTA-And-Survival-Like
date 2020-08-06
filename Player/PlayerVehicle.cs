using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicle : CharacterVehicle
{
    private Player _player = null;

    protected override void Awake()
    {
        base.Awake();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (IsDriving)
        {
            _drivenVehicle.HandleMove(GameManager.Instance.InputManager.Vertical);
            _drivenVehicle.HandleSteering(GameManager.Instance.InputManager.Horizontal);
            _drivenVehicle.HandleBrake(GameManager.Instance.InputManager.SpaceHeldDown);
        }

        if (IsInsideVehicle && GameManager.Instance.InputManager.Interact)
        {
            ExitVehicle();
        }
    }

    protected override void LoseControl()
    {
        // When we try to get inside a vehicle, we shouldn't be in aim mode anymore
        _player.PlayerState.UnprepareWeapon();
        _player.CharacterController.enabled = false;
    }

    public override void RegainControl()
    {
        _player.CharacterController.enabled = true;
    }
}

