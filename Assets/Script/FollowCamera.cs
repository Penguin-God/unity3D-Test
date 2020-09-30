using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // 카메라가 따라갈 목표
    public Vector3 offset; // 카메라의 현재 위치(보정값)

    private void Update()
    {
        // 카메라의 위치는 Player의 위치 + 카메라의 현재 Vector값 Player는 Vcetor이 0, 0, 0이고 카메라는 0, 21, -11이다.
        transform.position = target.position + offset;
    }
}
