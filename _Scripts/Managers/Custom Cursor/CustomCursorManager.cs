using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursorManager : MonoBehaviour
{
    Vector2 targetPos;
    [SerializeField] Camera mainCam;

    private void Start()
    {
        Cursor.visible = false; // 기존 화살표 커서 숨기기
    }

    private void Update()
    {
        targetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = targetPos;
    }
}
