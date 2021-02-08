using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2f; // 회전파워 
    float scaleValue = 0.1f;
    bool isShot;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue; // Vector3.one = Vector3(1.0, 1.0, 1.0)
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null; // 이 문장을 쓰지 않으면 while문이 너무 빨리 돌아서 게임이 아예 멈춤
        }
    }
}
