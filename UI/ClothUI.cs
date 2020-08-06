using UnityEngine;
using UnityEngine.UI;

public class ClothUI : MonoBehaviour
{
    [SerializeField] private Image _clothImage = null;
    [SerializeField] private Text _clothNameText = null;


    private Cloth _cloth = null;
    public Cloth Cloth { set { _cloth = value; } }

    public void Initialize(Cloth cloth)
    {
        _cloth = cloth;
        _clothImage.sprite = _cloth.Sprite;
        _clothNameText.text = _cloth.Name;
    }

    public void WearButton()
    {
        GameManager.Instance.Player.PlayerClothes.Wear(_cloth);
    }
}
