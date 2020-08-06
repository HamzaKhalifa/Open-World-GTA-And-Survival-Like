using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    [SerializeField] private GameObject _interactSignal = null;
    [SerializeField] private BlackScreenUI _blackScreenUI = null;
    [SerializeField] private WardrobeUI _wardrobeUI = null;
    [SerializeField] private GameObject _vehicleFuelStatusPanel = null;
    [SerializeField] private BackpackUI _backpackUI = null;

    public BlackScreenUI BlackScreenUI => _blackScreenUI;
    public WardrobeUI WardrobeUI => _wardrobeUI;
    public BackpackUI BackpackUI => _backpackUI;

    private void Awake()
    {
        Instance = this;

        _interactSignal.SetActive(false);
    }

    public void SetInteractiveSignal(bool active)
    {
        if (_interactSignal.activeSelf != active)
            _interactSignal.SetActive(active);

        if (GameManager.Instance.Player.PlayerVehicle.EnteredVehicle != null && !_vehicleFuelStatusPanel.gameObject.activeSelf)
        {
            _vehicleFuelStatusPanel.gameObject.SetActive(true);
        }
        if (GameManager.Instance.Player.PlayerVehicle.EnteredVehicle == null && _vehicleFuelStatusPanel.gameObject.activeSelf)
        {
            _vehicleFuelStatusPanel.gameObject.SetActive(false);
        }
    } 
}
