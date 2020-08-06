using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveWardrobe : InteractiveObject
{
    [SerializeField] private Transform _playerPositionTransform = null;
    [SerializeField] private Transform _playerCameraPositionTransform = null;
    [SerializeField] private float _blackScreeFadeTime = .5f;
    [SerializeField] private float _blackScreeTime = .1f;

    private bool _wardrobeOpen = false;
    private bool _canInteract = true;

    public override void Interact(Transform interactor)
    {
        if (!_canInteract) return;

        _wardrobeOpen = !_wardrobeOpen;

        Player player = interactor.GetComponent<Player>();

        if (_wardrobeOpen)
        {
            player.PlayerState.PlayerMoveState = PlayerMoveState.BusyDressing;
            _canInteract = false;
            UIManager.Instance.BlackScreenUI.SetBlackScreen(2, 1, () =>
            {
                player.CharacterController.enabled = false;
                player.transform.position = _playerPositionTransform.position;
                player.transform.rotation = _playerPositionTransform.rotation;
                player.CharacterController.enabled = true;

                GameManager.Instance.PlayerCamera.transform.position = _playerCameraPositionTransform.position;
                GameManager.Instance.PlayerCamera.transform.rotation = _playerCameraPositionTransform.rotation;

                // Now we activate the wardrobe
                UIManager.Instance.WardrobeUI.Activate(true);

                Invoke("CanInteractAgain", 3);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            });
        } else
        {
            player.PlayerState.PlayerMoveState = PlayerMoveState.Walking;

            // We deactivate the wardrobe
            UIManager.Instance.WardrobeUI.Activate(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void CanInteractAgain()
    {
        _canInteract = true;
    }
}
