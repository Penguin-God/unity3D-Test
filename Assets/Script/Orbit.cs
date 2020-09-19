using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform 공전축중심;
    public float 공전속도;
    Vector3 offset; // 공전목표와의 거리

    private void Start()
    {
        offset = transform.position - 공전축중심.position; // offset = 수류탄과 플레이어의거리
    }

    private void Update()
    {
        transform.position = 공전축중심.position + offset; // 플레이어의 현재위치에 offset을 더함

        // RotateAround(타겟, 회전 방향, 속도) : 타겟 주위를 회전(공전)하는 함수 다만, 타겟이 움직이면 타겟을 따라오지 못해 회전하는 물체의 포지션을 변경시켜주어야함 
        transform.RotateAround(공전축중심.position, Vector3.up, 공전속도 * Time.deltaTime);

        offset = transform.position - 공전축중심.position; // 물체가 계속 공전하므로 offset을 다시 설정
    }
}
