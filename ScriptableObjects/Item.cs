using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string Name = "";
    [TextArea] public string Description = "";
    public Sprite Sprite = null;
    public float HealthBonus = 0f;
    public float EnergyBonus = 0f;
    public float HungerBonus = 0f;
    public string UseAnimation = "Eat";
    public InteractivePickable InteractivePickable = null;

    public void Use(Player player)
    {
        player.PlayerHealth.RegainHealth(HealthBonus);
        player.PlayerStatus.RegainEnergy(EnergyBonus);
        player.PlayerStatus.RegainHunger(HungerBonus);
    }
}
