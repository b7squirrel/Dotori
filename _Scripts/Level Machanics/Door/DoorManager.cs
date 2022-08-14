using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoorManager : MonoBehaviour
{
    public static DoorManager instance;
    DoorA[] doors;
    public event Action onDoorTrigger;
    public void DoorTrigger()
    {
        onDoorTrigger?.Invoke();
    }

    private void Awake()
    {
        instance = this;
        doors = FindObjectsOfType<DoorA>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoorTrigger();
        }
    }
}
