using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BodyPartMaterial
{
    public PlayerBodyPart PlayerBodyPart;
    public Material Material = null;
}

public class PlayerClothes : MonoBehaviour
{
    [SerializeField] private List<GameObject> _clothesMeshes = new List<GameObject>();
    [SerializeField] private List<BodyPartMaterial> _bodyPartsMaterials = new List<BodyPartMaterial>();
    [SerializeField] private List<Cloth> _inspectorAssignedClothes = new List<Cloth>();

    private Player _player = null;

    private List<Cloth> _wornClothes = new List<Cloth>();

    private void Awake()
    {
        _player = GetComponent<Player>();

        // We need to add the inspector assigned clothes to our list of worn clothes first (to check if there is some conflict between clothes that can be worn) 
        foreach (Cloth cloth in _inspectorAssignedClothes)
        {
            Wear(cloth);
        }
    }

    public void Wear(Cloth clothToWear)
    {
        bool alreadyWearing = false;
        // If we are already wearing the cloth, we are take it off
        if (_wornClothes.Contains(clothToWear)) alreadyWearing = true;

        // Check if we are wearing something of the same type, and take it off
        List<Cloth> wornClothesCopy = new List<Cloth>();
        foreach (Cloth cloth in _wornClothes)
        {
            wornClothesCopy.Add(cloth);
        }

        // Taking off clothes of the same type
        foreach (Cloth cloth in wornClothesCopy)
        {
            foreach (ClothType clothType in clothToWear.ClothTypes)
            {
                if (cloth.ClothTypes.Contains(clothType))
                {
                    _wornClothes.RemoveAll(clothToRemove => clothToRemove.Name == cloth.Name);
                    // We too off the cloth, no need to keep looping through other cloth types.
                    break;
                }
            }
        }

        // Now we add the cloth to our list. Only if we aren't going to take it off
        if (!alreadyWearing)
            _wornClothes.Add(clothToWear);

        // Now we refresh our clothes
        RefreshClothes();
    }

    private void RefreshClothes()
    {
        // We first take off everything and activate the rendering of all body parts:

        // We start by deactivating the meshes objects
        foreach (GameObject meshObject in _clothesMeshes)
        {
            meshObject.SetActive(false);
        }

        // Now we turn on the body parts
        foreach (BodyPartMaterial bodyPartMaterial in _bodyPartsMaterials)
        {
            bodyPartMaterial.Material.SetFloat("_Mode", 0);
            Color fullOpacity = new Color(bodyPartMaterial.Material.color.r, bodyPartMaterial.Material.color.g, bodyPartMaterial.Material.color.b, 1);
            bodyPartMaterial.Material.SetColor("_Color", fullOpacity);
        }

        // Now we start wearing the worn clothes
        foreach (Cloth cloth in _wornClothes)
        {
            // We activate the mesh
            GameObject clothMesh = _clothesMeshes.Find(mesh => mesh.transform.name == cloth.Name);
            clothMesh.SetActive(true);

            // Now we hide the parts that need to be hidden
            foreach (PlayerBodyPart partToHideType in cloth.PartsToHide)
            {
                Material bodyPartMaterial = _bodyPartsMaterials.Find(foundBodyPartMaterial => foundBodyPartMaterial.PlayerBodyPart == partToHideType).Material;
                bodyPartMaterial.SetFloat("_Mode", 3);
                Color zeroOpacity = new Color(bodyPartMaterial.color.r, bodyPartMaterial.color.g, bodyPartMaterial.color.b, 0);
                bodyPartMaterial.SetColor("_Color", zeroOpacity);
            }

            if (cloth.Shoes != Shoes.None)
            {
                _player.PlayerState.Shoes = cloth.Shoes;
            }
        }
    }
}

