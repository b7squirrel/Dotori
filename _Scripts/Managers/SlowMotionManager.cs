using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    public static SlowMotionManager instance;
    [SerializeField] bool IsSlowMotion; // 슬로우 모션 상태가 되는 조건을 만족했을 때
    [SerializeField] bool OnSlowMotion; // 슬로우 모션이 진행 중일 떄 (슬로우 모션 코루틴을 계속 실행시키지 않기 위해)

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
    private void Update()
    {
        SlowMotion();
    }
    void SlowMotion()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsSlowMotion = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            IsSlowMotion = false;
            OnSlowMotion = false;
            StopSlowMotion();
        }
        if (IsSlowMotion == true && OnSlowMotion == false) //슬로우모션 조건을 만족하고 아직 슬로우 모션이 실행중이 아니라면
        {
            StartSlowMotion();
            OnSlowMotion = true;
        }
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
