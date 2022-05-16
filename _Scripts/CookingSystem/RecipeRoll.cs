using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeRoll : MonoBehaviour
{
    public static RecipeRoll instance;
    public RollSO[] recipeRoll;

    private void Awake()
    {
        instance = this;
    }

    public RollSO GetRollSo(Roll.rollType _rollType)
    {
        foreach (var _recipe in recipeRoll)
        {
            if (_rollType == _recipe.rollType)
            {
                return _recipe;
            }
        }
        return null;

    }

}
