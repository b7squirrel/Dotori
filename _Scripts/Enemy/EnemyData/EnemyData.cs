using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    enum enemyType 
    {
        goul,
        mageFire,
        goblin,
        cornFly
    }
    [SerializeField] enemyType currentEnemyType;

    public void PlayDieSound()
    {
        switch (currentEnemyType)
        {
            case enemyType.goul:
                PlaySound("Goul_Die_01");
                PlaySound("GoulBreak01");
                break;
            case enemyType.mageFire:
                PlaySound("Goul_Die_01");
                break;
            case enemyType.cornFly:
                PlaySound("InsectCrush_01");
                PlaySound("InsectCrush_02");
                break;
            case enemyType.goblin:
                PlaySound("Goul_Die_01");
                break;
        }
    }
    void PlaySound(string _sound)
    {
        AudioManager.instance.Play(_sound);
    }

}
