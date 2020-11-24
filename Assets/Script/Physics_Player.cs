using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics_Player : MonoBehaviour
{
    public float speed = 15;
    private float maxSpeed = 30;
    public Camera followCamera;

    // 상태확인 변수
    public bool isJump;
    bool isDodje;
    public bool isMelee;
    bool isBorder; // 벽에 닿으면 true


    // 키입력
    private float hAxis;
    private float xAxis;
    bool WalkKey;
    bool JumpKey;

    Vector3 MoveVec;
    Vector3 DodgeVector;

    Animator animator;
    new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>(); // Animator 컴포넌트가 Player의 자식에게 있기 때문에 GetComponentInChildren<>을 사용
    }

    void Update()
    {
        GetInput();
        Dodge();
        PlayerMove();
        PlayerTurn();
        Jump();
    }

    void GetInput() // 마우스, 키보드 등 입력받기
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        hAxis = Input.GetAxisRaw("Vertical");
        WalkKey = Input.GetButton("Walk"); // 느리게 걷기(left shift)
        JumpKey = Input.GetButtonDown("Jump");
    }

    void PlayerMove() // 이동
    {
        if (isDodje) // 구르기나 공격중에는 행동 전 백터값으로 직진함
            MoveVec = DodgeVector;
        else
            MoveVec = new Vector3(xAxis, 0, hAxis).normalized; // normalized : 방향 값이 1로 보정된 백터(저걸 안하면 대각선 이동 시 평소보다 더 빠르게 이동함)

        if (!isBorder)
        {
            transform.position += MoveVec * speed * (WalkKey ? 0.3f : 1f) * Time.deltaTime; // Time.dataTime : 안넣으면 프레임당 움직임 넣으면 초당 움직임
            // 가속도
            if (speed < maxSpeed && MoveVec != Vector3.zero)
            {
                speed += 0.05f;
            }
            else if (MoveVec == Vector3.zero)
                speed = 15;
        }

        // 애니메이션
        animator.SetBool("IsRun", MoveVec != Vector3.zero); // Vector3.zero = Vector3(0, 0, 0); 즉 모든 Vector값이 0이 아니면 "IsRun"은 true
        animator.SetBool("IsWalk", WalkKey);
    }

    void PlayerTurn() // 회전
    {
        // player가 나아가는 방향을 바라보는 코드
        transform.LookAt(transform.position + MoveVec); // LookAt() :  백터가 지정한 방향으로 회전시켜주는 함수 
    }

    void Dodge() // 회피(구르기)
    {
        if (JumpKey && !isJump && !isDodje && MoveVec != Vector3.zero) // 움직이고 있을떄만 구르기 사용
        {
            DodgeVector = MoveVec;
            speed *= 2;
            animator.SetTrigger("DoDodge");
            isDodje = true;

            Invoke("DodgeOut", 0.5f); // Invoke("함수명", time) : time후에 ""안에 함수가 실행됨, Invoke없이 바로 함수 쓰면 안 빨라지는 것처럼 보임
        }
    }

    void DodgeOut() // 구르기 끝내기 함수
    {
        isDodje = false;
        speed *= 0.5f;
    }

    private void FixedUpdate()
    {
        회전방지();
        StopToWall();
    }

    void 회전방지()
    {
        rigidbody.angularVelocity = Vector3.zero; // angularVelocity : 물리회전속도
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 3, Color.green); // Debug.DrawRay(시작점, 쏘는방향 * 길이, 색깔); : Scene 내에서 Ray를 보여주는 함수
        isBorder = Physics.Raycast(transform.position, transform.forward, 3, LayerMask.GetMask("벽")); // Raycast(시작점, 쏘는방향, 길이, 가져올오브젝트레이어) : Ray를 쏘아 닿는 오브젝트를 감지하는 함수  
    }

    void Jump()
    {
        if (JumpKey && !isJump && MoveVec == Vector3.zero) // 가만히 있을때만 점프가능
        {
            // .AddForce(힘, 유형) : Rigidbody에 힘을 추가한다   Impulse : 질량을 사용하여 리지드 바디에 순간적인 힘 임펄스를 추가
            rigidbody.AddForce(Vector3.up * 13, ForceMode.Impulse); // 편집 -> 프로젝트 세팅-> 물리에가면 중력값 조정 가능
            isJump = true;
            animator.SetBool("IsJump", isJump);
            animator.SetTrigger("DoJump"); // Trigger : 트리거가 호출되는 순간에 한 번 켜지고, 트리거 조건이 있는 트랜지션을 통과하면 자동으로 꺼지기 때문에 별다른 세부 조건이 없다.
        }
    }

    private void OnCollisionEnter(Collision collision) // 점프 후 바닥에 닿을시 애니미이션
    {
        if (collision.gameObject.CompareTag("바닥"))
        {
            isJump = false;
            animator.SetBool("IsJump", isJump);
        }
    }
}
