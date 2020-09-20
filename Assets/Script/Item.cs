using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // enum : 열거형 타입 (타입 이름 지정 필요), 선언은 중괄호 안에 데이터를 열거하듯이 작성
    public enum Type {Ammo, 수류탄, Heart, Coin, Waepon };
    public Type type; // enum은 하나의 타입이므로 담을 변수가 따로 필요
    public int value;

    new Rigidbody rigidbody;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "바닥")
        {
            rigidbody.isKinematic = true;
            sphereCollider.enabled = false; // enabled : 활성화된 콜라이더는 다른 콜라이더와 충돌하고 비활성화된 콜라이더는 충돌하지 않음
        }
    }
}
