using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    private PlayerHurtBox playerHurtBox;

    public bool isDead;

    public GameObject playerDieEffect;
    

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        isDead = false;
        playerHurtBox = GetComponentInChildren<PlayerHurtBox>();
    }

    private void Update()
    {
        if(isDead)
        {
            GameObject dieEffect = Instantiate(playerDieEffect, transform.position, transform.rotation);
            dieEffect.transform.eulerAngles = new Vector3(0, playerHurtBox.whichSideToBeHit, 0);
            gameObject.SetActive(false);
        }
    }
}
