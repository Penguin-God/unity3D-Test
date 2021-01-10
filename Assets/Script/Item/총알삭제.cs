using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 총알삭제 : MonoBehaviour
{
    public int Damage;
    public bool isMelee; // 근접 공격 범위는 삭제되는 버그 방지용 변수

    private void OnCollisionEnter(Collision collision) // 바닥과 닿았을 때 3초뒤에 삭제
    {
        if (collision.gameObject.tag == "바닥")
            Destroy(gameObject, 3);
    }

    private void OnTriggerEnter(Collider other) // 벽과 닿았을 때 바로 삭제
    {
        if (!isMelee && other.gameObject.tag == "벽") 
            Destroy(gameObject);
    }
}
