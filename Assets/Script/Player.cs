using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] 무기;
    public bool[] 무기보유;

    private float hAxis;
    private float xAxis;
    bool WalkKey;
    bool JumpKey;
    bool isJump;
    bool isDodje;
    bool GetItemKey;

    Vector3 MoveVec;
    Vector3 DodgeVector;
    Animator animator;
    new Rigidbody rigidbody;

    GameObject ItemObject;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>(); // Animator 컴포넌트가 Player의 자식에게 있기 때문에 GetComponentInChildren<>을 사용
    }

    void Update()
    {
        GetInput();
        PlayerMove();
        PlayerTurn();
        Jump();
        Dodge();
        GetItem();
    }
    
    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        hAxis = Input.GetAxisRaw("Vertical");
        WalkKey = Input.GetButton("Walk");
        JumpKey = Input.GetButtonDown("Jump");
        GetItemKey = Input.GetButtonDown("GetItem");
    }

    void PlayerMove()
    {
        // 이동
        if (isDodje)
            MoveVec = DodgeVector;
        else
            MoveVec = new Vector3(xAxis, 0, hAxis).normalized; // normalized : 방향 값이 1로 보정된 백터(저걸 안하면 대각선 이동 시 평소보다 더 빠르게 이동함)
        transform.position += MoveVec * speed * (WalkKey ? 0.3f : 1f) * Time.deltaTime; // transform이동은 Time.dalraTime을 넣어줘야 함

        // 애니메이션
        animator.SetBool("IsRun", MoveVec != Vector3.zero); // Vector3.zero = Vector3(0, 0, 0); 즉 모든 Vector값이 0이 아니면 "IsRun"은 true
        animator.SetBool("IsWalk", WalkKey);
    }

    void PlayerTurn() // 회전
    {
        // player가 나아가는 방향을 바라보는 코드
        transform.LookAt(transform.position + MoveVec); // LookAt() : 지정된 백터를 향해서 회전시켜주는 함수 
    }

    void Jump()
    {
        if (JumpKey && !isJump && MoveVec == Vector3.zero && !isDodje) // 가만히 있을때만 점프가능
        {
            // .AddForce(힘, 유형) : Rigidbody에 힘을 추가한다   Impulse : 질량을 사용하여 리지드 바디에 순간적인 힘 임펄스를 추가
            rigidbody.AddForce(Vector3.up * 13, ForceMode.Impulse); // 편집 -> 프로젝트 세팅-> 물리에가면 중력값 조정 가능
            isJump = true;
            animator.SetBool("IsJump", isJump);
            animator.SetTrigger("DoJump"); // Trigger : 트리거가 호출되는 순간에 한 번 켜지고, 트리거 조건이 있는 트랜지션을 통과하면 자동으로 꺼지기 때문에 별다른 세부 조건이 없다.
        }
    }

    void Dodge() // 회피(구르기)
    {
        if (JumpKey && !isJump && MoveVec != Vector3.zero && !isDodje) // 움직이고 있을떄만 구르기 사용
        {
            DodgeVector = MoveVec;
            speed *= 2;
            animator.SetTrigger("DoDodge");
            isDodje = true;
            
            Invoke("DodgeOut", 0.5f); // Invoke("함수명", time) : time후에 ""안에 함수가 실행됨, Invoke없이 바로 함수 쓰면 안 빨라지는 것처럼 보임
        }
    }

    void DodgeOut()
    {
        isDodje = false;
        speed *= 0.5f;
    }

    void GetItem()
    {
        if(GetItemKey && ItemObject != null)
        {
            if(ItemObject.tag == "무기")
            {
                Item item = ItemObject.GetComponent<Item>();
                int weaponIndex = item.value;
                무기보유[weaponIndex] = true;
                Destroy(ItemObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("바닥"))
        {
            isJump = false;
            animator.SetBool("IsJump", isJump);
        }
    }

    private void OnTriggerStay(Collider other) // OnTriggerStay : 트리거가 다른(이 프로젝트는 Player)Collider 에 계속 닿아있는 동안 "거의"매 프레임 호출됨
    {
        if(other.tag == "무기")
        {
            ItemObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) // OnTriggerExit : Collider가 /other/의 트리거에 닿는 것을 중지했을 때 호출됩니다.
    {
        if (other.tag == "무기")
        {
            ItemObject = null;
        }
    }
}
