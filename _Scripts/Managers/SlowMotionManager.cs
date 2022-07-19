using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    public static SlowMotionManager instance;
    [SerializeField] float slowMotionTimeScale;
    float defaultTimeScale = 1f;
    float defaultFixedDeltaTime = 0.02f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StopSlowMotion();
    }

    public void StartSlowMotion()
    {
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime * slowMotionTimeScale;
    }
    public void StopSlowMotion()
    {
        Time.timeScale = defaultTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
    }
}
