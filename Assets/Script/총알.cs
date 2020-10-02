using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 총알 : MonoBehaviour
{
    public int Damage;

    private void OnCollisionEnter(Collision collision) // 탄피가 바닥과 닿았을 때
    {
        if (collision.gameObject.tag == "바닥")
            Destroy(gameObject, 3); // 바닥과 닿을경우 3초뒤에 삭제
    }

    private void OnTriggerEnter(Collider other) // 총알이 벽과 닿았을 때
    {
        if (other.gameObject.tag == "벽")
            Destroy(gameObject); // 벽에 닿을경우 바로 삭제
    }
}
