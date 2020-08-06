using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider = null;

    [SerializeField] private Slider _energySlider = null;
    [SerializeField] private Image _energySliderImage = null;
    [SerializeField] private Color _recoveringEnergySliderColor = new Color(84, 0, 1, 1);

    [SerializeField] private Slider _hungerSlider = null;


    private Color _initialEnergySliderColor = Color.black;

    private void Awake()
    {
        _initialEnergySliderColor = _energySliderImage.color;
    }

    private void Update()
    {
        Player player = GameManager.Instance.Player;

        _healthSlider.value = player.PlayerStatus.Health / 100;
        _energySlider.value = player.PlayerStatus.Enery / 100;
        _hungerSlider.value = player.PlayerStatus.Hunger / 100;

        Color color = player.PlayerStatus.IsRecoveringEnergy ? _recoveringEnergySliderColor : _initialEnergySliderColor;
        if (_energySliderImage.color != color) _energySliderImage.color = color;
    }
}
