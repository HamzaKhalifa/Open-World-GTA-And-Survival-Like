using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerBodyPart
{
    Feet,
    Forearms,
    Hands,
    Hips,
    Legs,
    Shoulders,
    Torso
}

public enum ClothType
{
    Shirt,
    Pants,
    Shoes,
    Gloves,
    Hat,
    Vest
}

[CreateAssetMenu(menuName = "Scriptable Objects/Cloth")]
public class Cloth : ScriptableObject
{
    [Tooltip("Every cloth is identified by its name")]
    [SerializeField] private string _name = null;
    [SerializeField] private Sprite _sprite = null;
    [SerializeField] private List<PlayerBodyPart> _partsToHide = new List<PlayerBodyPart>();
    [SerializeField] private List<ClothType> _clothTypes = new List<ClothType>();
    [SerializeField] private Shoes _shoes = Shoes.None;

    public string Name => _name;
    public Sprite Sprite => _sprite;
    public List<PlayerBodyPart> PartsToHide => _partsToHide;
    public List<ClothType> ClothTypes => _clothTypes;
    public Shoes Shoes => _shoes;
}