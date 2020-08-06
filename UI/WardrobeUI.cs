using System.Collections.Generic;
using UnityEngine;

public class WardrobeUI : MonoBehaviour
{
    [SerializeField] private GameObject _wardrobePanel = null;
    [SerializeField] private List<Cloth> _clothes = new List<Cloth>();
    [SerializeField] private ClothUI _clothUIPrefab = null;
    [SerializeField] private GameObject _content = null;

    private void Awake()
    {
        _wardrobePanel.gameObject.SetActive(false);

        foreach(Cloth cloth in _clothes)
        {
            ClothUI tmp = Instantiate(_clothUIPrefab, _content.transform);
            tmp.Initialize(cloth);
        }
    }

    public void Activate(bool activate)
    {
        _wardrobePanel.gameObject.SetActive(activate);
    }
}
