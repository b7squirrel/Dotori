using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;
    PlayerHurtBox playerHurtBox;
    [SerializeField] GameObject playerDieEffect;
    [SerializeField] GameObject playerCaptured;
    bool isDead;

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
        if (isDead)
        {
            GameObject dieEffect = Instantiate(playerDieEffect, transform.position, transform.rotation);
            dieEffect.transform.eulerAngles = new Vector3(0, playerHurtBox.Angle_Y, 0);
            gameObject.SetActive(false);
        }
    }

    public void KillPlayer()
    {
        isDead = true; 
    }

    public void CapturePlayer(Transform parentPosition)
    {
        GameObject capturedPlayer = Instantiate(playerCaptured, transform.position, transform.rotation);
        capturedPlayer.transform.position = parentPosition.position;
        capturedPlayer.transform.parent = parentPosition;
        KillPlayer();
    }
}
