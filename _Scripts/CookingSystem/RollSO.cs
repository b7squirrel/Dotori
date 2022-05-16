using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Roll", menuName = "RollSO")]
public class RollSO : ScriptableObject
{
    public Roll.rollType rollType;
    public Sprite rollSprite_UI;
    public GameObject[] rollPrefab;
    public AnimatorOverrideController[] roll_OverrideController;
    public float height;
}
