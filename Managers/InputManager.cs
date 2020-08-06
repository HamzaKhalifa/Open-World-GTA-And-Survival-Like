using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float Horizontal = 0f;
    public float Vertical = 0f;
    public float MouseX = 0f;
    public float MouseY = 0f;
    public bool Rushing = false;
    public bool ChangeMode = false;
    public bool SpaceDown = false;
    public bool SpaceUp = false;
    public bool SpaceHeldDown = false;
    public bool Interact = false;
    public bool ChangeCameraView = false;
    public bool WallClimb = false;
    public bool ToggleTorch = false;
    public bool PreparedOrUnprepareWeapon = false;
    public bool SwitchWeaponUp = false;
    public bool SwitchWeaponDown = false;
    public bool Firing = false;
    public bool Attack = false;
    public bool Reload = false;
    public bool SwitchZoomCameraRightOrLeft = false;
    public bool FrontFlip = false;
    public bool HeavyAttack = false;
    public bool Backpack = false;

    public bool IsTryingToMove => Mathf.Abs(Horizontal) > .3f || Mathf.Abs(Vertical) > .3f;

    private void Update()
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        MouseX = Input.GetAxis("Mouse X");
        MouseY = Input.GetAxis("Mouse Y");

        Rushing = Input.GetKey(KeyCode.LeftShift);
        ChangeMode = Input.GetKeyDown(KeyCode.M);
        SpaceDown = Input.GetKeyDown(KeyCode.Space);
        SpaceUp = Input.GetKeyUp(KeyCode.Space);
        SpaceHeldDown = Input.GetKey(KeyCode.Space);
        Interact = Input.GetKeyDown(KeyCode.E);
        ChangeCameraView = Input.GetKeyDown(KeyCode.V);
        WallClimb = Input.GetKeyDown(KeyCode.RightShift);
        ToggleTorch = Input.GetKeyDown(KeyCode.T);
        PreparedOrUnprepareWeapon = Input.GetKeyDown(KeyCode.RightAlt);
        SwitchWeaponDown = Input.GetKeyDown(KeyCode.A);
        SwitchWeaponUp = Input.GetKeyDown(KeyCode.P);
        Firing = Input.GetKey(KeyCode.Mouse0);
        Attack = Input.GetKeyDown(KeyCode.Mouse0);
        HeavyAttack = Input.GetKeyDown(KeyCode.Mouse1);
        Reload = Input.GetKeyDown(KeyCode.R);
        SwitchZoomCameraRightOrLeft = Input.GetKeyDown(KeyCode.N);
        FrontFlip = Input.GetKeyDown(KeyCode.F);
        Backpack = Input.GetKeyDown(KeyCode.Tab);
    }
}
