using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Rotation Setting")]
    [Range(1f, 100f)]
    public float speed;
    public bool ranDir;

    private void Start()
    {
        if (ranDir)
        {
            int dir = Random.Range(0, 2);
            if (dir == 1)
            {
                speed *= -1f;
            }
        }
    }

    private void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
