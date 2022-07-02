using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeFlavor : MonoBehaviour
{
    public static RecipeFlavor instance;

    public FlavorSo[] recipeFlavor;

    private void Awake()
    {
        instance = this;
    }

    public FlavorSo GetFlavourSo(FlavorSo _flavorSo)
    {
        FlavorSo _temp = _flavorSo; // 값을 할당해야 하므로 아무 값이나 할당했음.
        foreach (var _recipe in recipeFlavor)
        {
            if (_flavorSo == _recipe)
            {
                _temp = _recipe;
            }
        }
        return _temp;
    }
}
