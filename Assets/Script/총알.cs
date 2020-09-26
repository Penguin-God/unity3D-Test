using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 총알 : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision) // 탄피가 벽 혹은 바닥과 닿았을 때
    {
        if (collision.gameObject.tag == "바닥")
            Destroy(gameObject, 3); // 바닥과 닿을경우 3초뒤에 삭제
        else if (collision.gameObject.tag == "벽")
            Destroy(gameObject);
    }
}
