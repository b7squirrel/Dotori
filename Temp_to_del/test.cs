using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    float gravity = 9.8f;
    float velocity;

    private void Update()
    {
        velocity += -gravity * Time.deltaTime;
        transform.Translate(new Vector3(0, velocity, 0) * Time.deltaTime);
    }
}
