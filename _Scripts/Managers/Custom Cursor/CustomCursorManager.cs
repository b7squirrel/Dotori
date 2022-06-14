using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursorManager : MonoBehaviour
{
    Vector2 targetPos;
    [SerializeField] Camera mainCam;

    private void Start()
    {
        Cursor.visible = false; // ���� ȭ��ǥ Ŀ�� �����
    }

    private void Update()
    {
        targetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = targetPos;
    }
}
