using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorA : MonoBehaviour
{
    Animator anim;
    BoxCollider2D boxCol;
    public bool isOpen;

    [SerializeField] LayerMask PlayerMask;

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
        boxCol = GetComponent<BoxCollider2D>();
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
        isPlayerIn = Physics2D.OverlapBox(center, size, 0, PlayerMask);
    }
    public void OpenDoor()
    {
        anim.SetTrigger("Open");
        isOpen = true;
        boxCol.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(center, size);
    }
    
}
