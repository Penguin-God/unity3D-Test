using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    public float speed;
    // 아이템 관련 변수
    public GameObject[] 무기;
    public bool[] 무기보유;
    public GameObject[] 보유수류탄;
    public Camera followCamera;

    public int 보유총알;
    public int Max총알;
    public int coin;
    public int MaxCoin;
    public int playerhp;
    public int MaxPlayerhp;
    public int 수류탄;
    public int Max수류탄;

    int EquipObjcetIndex = -1; // 보유중인 무기 숫자

    // 상태확인
    public bool isJump;
    bool isDodje;
    bool isSwap;
    bool isReload;
    public bool isMelee;
    bool isBorder; // 벽에 닿으면 true


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
    bool ReloadDown;

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
        Reload();
    }

    void GetInput() // 마우스, 키보드 등 입력받기
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        hAxis = Input.GetAxisRaw("Vertical");
        WalkKey = Input.GetButton("Walk"); // 느리게 걷기(left shift)
        JumpKey = Input.GetButtonDown("Jump");
        GetItemKey = Input.GetButtonDown("GetItem"); // 아이템 줍기(e)
        // 무기 교체입력(1, 2, 3)
        SwapWeapon1 = Input.GetButtonDown("Swap1");
        SwapWeapon2 = Input.GetButtonDown("Swap2");
        SwapWeapon3 = Input.GetButtonDown("Swap3");
        
        AttackDown = Input.GetButton("Fire1"); // 공격 입력
        ReloadDown = Input.GetButtonDown("Reload"); // 장전
    }

    void PlayerMove() // 이동
    {
        if (isDodje || isMelee) // 구르기나 공격중에는 행동 전 백터값으로 직진함
            MoveVec = DodgeVector;
        else
            MoveVec = new Vector3(xAxis, 0, hAxis).normalized; // normalized : 방향 값이 1로 보정된 백터(저걸 안하면 대각선 이동 시 평소보다 더 빠르게 이동함)

        if ((!MeleeReady && EquipObjcetIndex == 1) || (EquipObjcetIndex == 2 && (AttackDown || !MeleeReady)))  // 원거리 공격중일 때는 이동 못함
            MoveVec = Vector3.zero;

        if(!isBorder) // 벽과 닿을때 Vector3.zero로 만들어 버리면 회전도 못해서 아예정지해 버리기 때문에 트랜스폼에 백터를 더해서 이동하는 것만 제한함
            transform.position += MoveVec * speed * (WalkKey ? 0.3f : 1f) * Time.deltaTime; // Time.dataTime : 안넣으면 프레임당 움직임 넣으면 초당 움직임

        // 애니메이션
        animator.SetBool("IsRun", MoveVec != Vector3.zero); // Vector3.zero = Vector3(0, 0, 0); 즉 모든 Vector값이 0이 아니면 "IsRun"은 true
        animator.SetBool("IsWalk", WalkKey);
    }

    void PlayerTurn() // 회전
    {
        // player가 나아가는 방향을 바라보는 코드
        transform.LookAt(transform.position + MoveVec); // LookAt() :  백터가 지정한 방향으로 회전시켜주는 함수 

        // 마우스에 의한 회전
        if (AttackDown && !isDodje && EquipObject != null && EquipObject.type == Weapons.Type.Range) // 마우스 클릭시에만 마우스 포인터를 바라봄
        {
            Ray CameraRay = followCamera.ScreenPointToRay(Input.mousePosition); // 카메라에서 마우스 누른곳에 Ray를 쏨
            RaycastHit rayHit;
            if (Physics.Raycast(CameraRay, out rayHit, 100)) // out : ray에 닿은 물체를 리턴함
            {
                Vector3 nextVec = rayHit.point - transform.position; // 마우스를 클릭한 지점에서 현재 플레이어 위치를 뺀 값을 넣음
                nextVec.y = 0; // y축으로도 도는거 방지
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (JumpKey && !isJump && MoveVec == Vector3.zero && MeleeReady) // 가만히 있을때만 점프가능
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
        if (JumpKey && !isJump && !isDodje && MoveVec != Vector3.zero && !isReload) // 움직이고 있을떄만 구르기 사용
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

        if ((SwapWeapon1 || SwapWeapon2 || SwapWeapon3) && !isDodje && !isMelee && !isReload) // 공격중일 때 스왑하면 들고있는 무기관련함수가 캔슬되서 공격중에 스왑막음
        {
            if(EquipObject != null)
                EquipObject.gameObject.SetActive(false);

            EquipObjcetIndex = WeaponIndex; // 조건에 맞을때 작동을 안하기 위해 값을 넣어줌
            EquipObject = 무기[WeaponIndex].GetComponent<Weapons>(); // EquipObject에 현재 장착중인 무기를 넣음
            EquipObject.gameObject.SetActive(true); // 장착한 무기 보여줌
            animator.SetTrigger("WeaponSwap");

            isSwap = true;
            Invoke("SwapOut", 0.4f); // 머신건 쏠 때는 애니메이션 빠르게 동작하는거 구현하자
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
        if(AttackDown && MeleeReady && !isSwap && !isDodje && !isReload)
        {
            DodgeVector = MoveVec; // DodgeVector에 공격하기 전 백터값을 넣음
            EquipObject.Use();
            if (EquipObject.type == Weapons.Type.Melee) // 장착한 무기에 따라 다른 애니메이션 실행
                animator.SetTrigger("DoSwing");
            else
                animator.SetTrigger(EquipObjcetIndex == 1 ? "DoShot" : "DoMachineGunShot");
            MeleeDelay = 0; // 공격 후 바로 공격 못하게 딜레이를 공속보다 낮게 0으로 만듬
        }
    }

    void Reload()
    {
        if (보유총알 == 0 || EquipObject == null || EquipObject.type == Weapons.Type.Melee || isReload)
            return;

        if(ReloadDown && !isDodje && !isSwap && MeleeReady)
        {
            animator.SetTrigger("DoReload");
            speed *= 0.5f;
            isReload = true;
            
            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int ReloadAmmo = 보유총알 > EquipObject.Max총알 ? EquipObject.Max총알 : 보유총알; // 총알 보유상태에 따라 충전할 총알의 수를 정함
        EquipObject.장전된총알 = ReloadAmmo; // Weapons script의 변수에 총알을 더함
        보유총알 -= ReloadAmmo;
        isReload = false;
        speed *= 2;
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



    private void OnCollisionEnter(Collision collision) // 점프 후 바닥에 닿을시 애니미이션
    {
        if (collision.gameObject.CompareTag("바닥"))
        {
            isJump = false;
            animator.SetBool("IsJump", isJump);
        }
    }

    private void OnTriggerEnter(Collider other) // 아이템하고 닿으면 먹는 로직, other : 닿은 오브젝트
    {
        if(other.tag == "아이템")
        {
            Item item = other.GetComponent<Item>(); // item에 닿은 아이템의 Item스크립트를 넣음
            switch (item.type)
            {
                case Item.Type.Ammo:
                    보유총알 += item.value;
                    if (보유총알 > Max총알)
                        보유총알 = Max총알;
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
