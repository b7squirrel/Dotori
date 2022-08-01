using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    public static SlowMotionManager instance;
    [SerializeField] bool IsSlowMotion; // ���ο� ��� ���°� �Ǵ� ������ �������� ��
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
        if (IsSlowMotion == true && OnSlowMotion == false) //���ο��� ������ �����ϰ� ���� ���ο� ����� �������� �ƴ϶��
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
