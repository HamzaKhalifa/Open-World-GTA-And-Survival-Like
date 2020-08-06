using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    [SerializeField] private Slider _fuelSlider = null;

    private void Update()
    {
        if (_fuelSlider != null && GameManager.Instance.Player != null && GameManager.Instance.Player.PlayerVehicle.EnteredVehicle != null)
        {
            _fuelSlider.value = GameManager.Instance.Player.PlayerVehicle.EnteredVehicle.Fuel;
        }
    }
}
