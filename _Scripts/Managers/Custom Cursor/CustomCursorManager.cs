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
        Vector2 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);
        targetPos = mainCam.ScreenToWorldPoint(mouseScreenPosition);
        transform.position = targetPos;
    }
}
