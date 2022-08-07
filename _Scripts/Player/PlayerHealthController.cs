using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;
    PlayerHurtBox playerHurtBox;
    PanManager panManager;
    PlayerData playerData;
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
        panManager = GetComponentInChildren<PanManager>();
        playerData = GetComponentInParent<PlayerData>();
    }

    private void Update()
    {
        if (isDead)
        {
            GameObject dieEffect = Instantiate(playerDieEffect, transform.position, transform.rotation);
            dieEffect.transform.eulerAngles = new Vector3(0, playerHurtBox.Angle_Y, 0);
            panManager.DropAllRolls();
            gameObject.SetActive(false);
        }
    }

    public void KillPlayer()
    {
        if (GameManager.instance.IsInvincible)
            return;
        if (PlayerController.instance.IsDodging)
            return;
        playerData.Play(PlayerData.soundType.die);
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
