using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    public static SlowMotionManager instance;
    [SerializeField] float slowMotionTimeScale;
    float defaultTimeScale;
    float defaultFixedDeltaTime;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        defaultTimeScale = Time.timeScale;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
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
