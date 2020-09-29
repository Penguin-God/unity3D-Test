using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    public float speed;
    // 아이템 관련 변수
    //public Item[] 무기물리;
    public GameObject[] 무기;
    public bool[] 무기보유;
    public GameObject[] 보유수류탄;

    public int 총알;
    public int coin;
    public int playerhp;
    public int 수류탄;
    public int Max총알;
    public int MaxCoin;
    public int MaxPlayerhp;
    public int Max수류탄;

    int EquipObjcetIndex = -1; // 보유중인 무기

    // 상태확인
    bool isJump;
    bool isDodje;
    bool isSwap;
    public bool isMelee;

    // 키입력
    private float hAxis;
    private float xAxis;
    bool WalkKey;
    bool JumpKey;
    bool GetItemKey;
    bool SwapWeapon1;
    bool SwapWeapon3;
    bool SwapWeapon2;
    bool AttackDown;

    // 근접공격 관련 변수
    bool MeleeReady = true;
    float MeleeDelay = 0.5f; // 처음 무기를 장착한 순간부터 MeleeDelay += Time.datatime코드가 실행되서 순간적으로 MeleeReady가 false가 되어 이동을 못하게 되므로 기초값을 공속보다 높게 조정함 

    Vector3 MoveVec;
    Vector3 DodgeVector;

    Animator animator;
    new Rigidbody rigidbody;

    GameObject ItemObject;
    Weapons EquipObject;

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
        GetItem();
        WeaponSwap();
        Attack();
    }
    
    void GetInput() // 마우스, 키보드 등 입력받기
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        hAxis = Input.GetAxisRaw("Vertical");
        // 느리게 걷기(left shift)
        WalkKey = Input.GetButton("Walk");
        JumpKey = Input.GetButtonDown("Jump");
        // 아이템 줍기(e)
        GetItemKey = Input.GetButtonDown("GetItem");
        // 무기 변경입력(1, 2, 3)
        SwapWeapon1 = Input.GetButtonDown("Swap1");
        SwapWeapon2 = Input.GetButtonDown("Swap2");
        SwapWeapon3 = Input.GetButtonDown("Swap3");
        //근접공격 입력
        AttackDown = Input.GetButton("Fire1");
    }

    void PlayerMove() // 이동
    {
        if (isDodje || isMelee) // 구르기나 공격중에는 행동 전 백터값으로 직진함
            MoveVec = DodgeVector;
        else
            MoveVec = new Vector3(xAxis, 0, hAxis).normalized; // normalized : 방향 값이 1로 보정된 백터(저걸 안하면 대각선 이동 시 평소보다 더 빠르게 이동함)

        if ((!MeleeReady || AttackDown) && EquipObject != null && EquipObject.type == Weapons.Type.Range)  // 원거리 공격중일 때는 이동 못함
            MoveVec = Vector3.zero;
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
        if (JumpKey && !isJump && MoveVec == Vector3.zero) // 가만히 있을때만 점프가능
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

    void WeaponSwap() // 무기 변경 함수
    {
        // 무기미보유상태, 변경하려는 무기가 현재 장착중일 때 작동안함
        if (SwapWeapon1 && (!무기보유[0] || EquipObjcetIndex == 0))
            return;
        if (SwapWeapon2 && (!무기보유[1] || EquipObjcetIndex == 1))
            return;
        if (SwapWeapon3 && (!무기보유[2] || EquipObjcetIndex == 2))
            return;

        // WeaponIndex에 현재들고있는 무기 값을 부여
        int WeaponIndex = -1;
        if (SwapWeapon1) WeaponIndex = 0;
        if (SwapWeapon2) WeaponIndex = 1;
        if (SwapWeapon3) WeaponIndex = 2;

        if ((SwapWeapon1 || SwapWeapon2 || SwapWeapon3) && !isDodje && !isMelee) // 공격중일 때 스왑하면 들고있는 무기관련함수가 캔슬되서 공격중에 스왑막음
        {
            if(EquipObject != null)
                EquipObject.gameObject.SetActive(false);

            EquipObjcetIndex = WeaponIndex; // 조건에 맞을때 작동을 안하기 위해 값을 넣어줌
            EquipObject = 무기[WeaponIndex].GetComponent<Weapons>(); // EquipObject에 현재 장착중인 무기를 넣음
            EquipObject.gameObject.SetActive(true); // 장착한 무기 보여줌
            animator.SetTrigger("WeaponSwap");

            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }


    void GetItem()
    {
        if(GetItemKey && ItemObject != null) // Item오브젝트에 닿아있을 때 E키 입력시 아이템 획득
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


    void Attack()
    {
        if (EquipObject == null)
            return;
        // Time.datatime : 지난 프레임이 완료되는 데 까지 걸리는시간을 나타내며 단위는 초를사용(Update함수에서 1프레임이 아닌 1초당 어떤 행동을 하고 싶을 때 델타타임을 곱함)
        MeleeDelay += Time.deltaTime; // Melee에 매 프레임 소비한 시간을 더함
        MeleeReady = EquipObject.공속 < MeleeDelay; // 공격한 후 지정한 공속보다 시간이 더 지나면 다시 공격할 수 있음
        if(AttackDown && MeleeReady && !isSwap && !isDodje)
        {
            DodgeVector = MoveVec; // DodgeVector에 공격하기 전 백터값을 넣음
            EquipObject.Use();
            animator.SetTrigger(EquipObject.type == Weapons.Type.Melee ? "DoSwing" : "DoShot"); // 장착한 무기에 따라 다른 애니메이션 실행
            MeleeDelay = 0; // 공격 후 바로 공격 못하게 딜레이를 공속보다 낮게 0으로 만듬
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

    private void OnTriggerEnter(Collider other) // other : 닿은 오브젝트
    {
        if(other.tag == "아이템")
        {
            Item item = other.GetComponent<Item>(); // item에 닿은 아이템의 Item스크립트를 넣음
            switch (item.type)
            {
                case Item.Type.Ammo:
                    총알 += item.value;
                    if (총알 > Max총알)
                        총알 = Max총알;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > MaxCoin)
                        coin = MaxCoin;
                    break;
                case Item.Type.Heart:
                    playerhp += item.value;
                    if (playerhp > MaxPlayerhp)
                        playerhp = MaxPlayerhp;
                    break;
                case Item.Type.수류탄:
                    if (수류탄 == Max수류탄)
                        return;
                    보유수류탄[수류탄].SetActive(true); // 더하기 전에 활성화 시키는 거라서 -1 안해도 됨
                    수류탄 += item.value;
                    break;
            }
            Destroy(other.gameObject); // 아이템 먹고 보유량 올라간 후에 먹은 아이템 삭제
        }
    }

    private void OnTriggerStay(Collider other) // OnTriggerStay : 트리거가 다른(이 프로젝트는 Player)Collider 에 계속 닿아있는 동안 "거의"매 프레임 호출됨
    {
        if(other.tag == "무기") // 무기에 콜라이더에 닿아있을 때
        {
            ItemObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) // OnTriggerExit : Collider가 /other/의 트리거에 닿는 것을 중지했을 때 호출됩니다.
    {
        if (other.tag == "무기") // 무기 콜라이더에서 벗어났을때
        {
            ItemObject = null;
        }
    }
}
