using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorA : MonoBehaviour
{
    Animator anim;
    public bool isOpen;

    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] BoxCollider2D boxCol;
    [SerializeField] BoxCollider2D attackBox;

    [Range(-1.5f, 1f)]
    [SerializeField] float offsetX;
    [Range(-1f, 5f)]
    [SerializeField] float offsetY;
    [Range(-1.5f, 1.5f)]
    [SerializeField] float sizeX;
    [Range(-1f, 1f)]
    [SerializeField] float sizeY;

    Vector2 size, center;
    bool isPlayerIn;

    private void Start()
    {
        anim = GetComponent<Animator>();
        size = new Vector3(sizeX, sizeY);
        center = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY);
    }

    private void Update()
    {
        PlayerCheck();
        if (isPlayerIn)
        {
            DoorManager.instance.onDoorTrigger += OpenDoor;
        }
        else
        {
            DoorManager.instance.onDoorTrigger -= OpenDoor;
        }
    }

    void PlayerCheck()
    {
        isPlayerIn = Physics2D.OverlapBox(center, size, 0, playerLayer);
    }
    void AttackEnemy()
    {
        Collider2D _hit = Physics2D.OverlapBox((Vector2)transform.position + attackBox.offset, attackBox.size, 0, enemyLayer);
        if (_hit)
        {
            _hit.GetComponent<EnemyHealth>().TakeDamage();
        }
    }
    public void OpenDoor()
    {
        anim.SetTrigger("Open");
        isOpen = true;
        boxCol.enabled = false;
        AttackEnemy();
        PlaySound();
    }

    IEnumerator ActivateAttackBoxCo()
    {
        attackBox.enabled = true;
        yield return new WaitForSeconds(.3f);
        attackBox.enabled = false;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        //Gizmos.DrawCube(center, size);
        //Gizmos.DrawCube((Vector2)transform.position + attackBox.offset, attackBox.size);
    }
    void PlaySound()
    {
        AudioManager.instance.Play("Door01");
        AudioManager.instance.Play("Door02");
        AudioManager.instance.Play("Door03");
        AudioManager.instance.Play("Door04");

    }

}
