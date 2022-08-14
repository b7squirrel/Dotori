using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    public static SlowMotionManager instance;
    [SerializeField] bool TriggerSlowMotion; // ���ο� ��� ���°� �Ǵ� ������ �������� ��
    [SerializeField] bool OnSlowMotion; // ���ο� ����� ���� ���� �� (���ο� ��� �ڷ�ƾ�� ��� �����Ű�� �ʱ� ����)

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
        //SlowMotion();
        //SlowMotionToggle();
    }
    void SlowMotion()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerSlowMotion = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            TriggerSlowMotion = false;
            OnSlowMotion = false;
            StopSlowMotion();
        }
        if (TriggerSlowMotion == true && OnSlowMotion == false) //���ο��� ������ �����ϰ� ���� ���ο� ����� �������� �ƴ϶��
        {
            StartSlowMotion(slowMotionTimeScale);
            OnSlowMotion = true;
        }
    }

    void SlowMotionToggle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerSlowMotion = !TriggerSlowMotion;
        }
        if (TriggerSlowMotion == false)
            OnSlowMotion = false;
        if (TriggerSlowMotion == true && OnSlowMotion == false) //���ο��� ������ �����ϰ� ���� ���ο� ����� �������� �ƴ϶��
        {
            StartSlowMotion(slowMotionTimeScale);
            OnSlowMotion = true;
        }
    }
    public void StartSlowMotion(float _slowMotionTimeScale)
    {
        Time.timeScale = _slowMotionTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime * _slowMotionTimeScale;
    }
    public void StopSlowMotion()
    {
        Time.timeScale = defaultTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
    }
}
