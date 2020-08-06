using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private GameObject _crosshair = null;

    private void Update()
    {
        Player player = GameManager.Instance.Player;

        bool active = (player.PlayerWeapons.EquippedWeaponMount.Weapon.IsRangeWeapon && player.PlayerState.PlayerMoveState != PlayerMoveState.BusyWithVehicle);

        if (active && !_crosshair.gameObject.activeSelf) _crosshair.gameObject.SetActive(true);
        if (!active && _crosshair.gameObject.activeSelf) _crosshair.gameObject.SetActive(false);
    }
}
