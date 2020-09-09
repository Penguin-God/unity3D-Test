using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;

    private float hAxis;
    private float xAxis;
    bool Walk;

    Vector3 MoveVec;
    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>(); // Animator 컴포넌트가 Player의 자식에게 있기 때문에 GetComponentInChildren<>을 사용
    }

    void Update()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        hAxis = Input.GetAxisRaw("Vertical");
        Walk = Input.GetButton("Walk");

        // 이동
        MoveVec = new Vector3(xAxis, 0, hAxis).normalized; // normalized : 방향 값이 1로 보정된 백터(저걸 안하면 대각선 이동 시 평소보다 더 빠르게 이동함)
        transform.position += MoveVec * speed * (Walk ? 0.3f : 1f) * Time.deltaTime; // transform이동은 Time.dalraTime을 넣어줘야 함

        // 애니메이션
        animator.SetBool("IsRun", MoveVec != Vector3.zero); // Vector3.zero = Vector3(0, 0, 0); 즉 모든 Vector값이 0이 아니면 "IsRun"은 true
        animator.SetBool("IsWalk", Walk);

        // 회전
        // player가 나아가는 방향을 바라보는 코드
        transform.LookAt(transform.position + MoveVec); // LookAt() : 지정된 백터를 향해서 회전시켜주는 함수 
    }
}
