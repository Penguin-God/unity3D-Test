using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent를 사용하기 위함 

public class Enemy : MonoBehaviour
{
    public enum Type { Normal, Charge, AD, Boss };
    public Type enemyType;
    public int MaxHP;
    public int CurrentHp;
    public Transform target;
    public bool isChase; // chase : 추적, 추적기능과 물리설정의 조건 역할
    public bool isAttack;
    public BoxCollider meleeCollider;
    public GameObject Missile;
    public int score;
    public GameObject[] coin;

    protected bool isDead;
    protected Rigidbody rigid;
    protected MeshRenderer[] meshs;
    protected NavMeshAgent nav;
    protected Animator animator;

    Coroutine coroutine; // 중지시킬 코루틴을 변수에 담음
    public GameManager gameManager;

    //public SphereCollider detectArea;
    //bool isFigth;
    //public GameObject Bang;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>(); 
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        Invoke("SetChase", 2);

        // 프리팹에 연결되는 객체는 무조건 프리팹 내부에 있는 오브젝트여야 하므로 그냥 Find로 넣음
        target = GameObject.Find("Player").GetComponent<Transform>(); 
    }

    void SetChase()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    private void Update()
    {
        if (nav.enabled && enemyType != Type.Boss)
        {
            nav.SetDestination(target.position); // 도착할 목표의 위치를 지정하는 함수
            nav.isStopped = !isChase; // isChase가 true일 때 정지
        }
    }

    private void OnTriggerEnter(Collider other) // 몬스터가 플레이어한터 피격 당함
    {
        if(!isDead)
        {
            if (other.tag == "Melee")
            {
                Weapons weapon = other.GetComponent<Weapons>();
                CurrentHp -= weapon.Damage;

                Vector3 DamageVec = this.transform.position - other.transform.position; // 몬스터 포지션 - 입장에서 맞은방향 계산
                DamageEffect(DamageVec);
            }
            else if (other.tag == "총알")
            {
                Bullet bullet = other.GetComponent<Bullet>();
                CurrentHp -= bullet.Damage;
                Vector3 DamageVec = this.transform.position - other.transform.position;
                Destroy(other.gameObject);
                DamageEffect(DamageVec);
            }
        }

        if(other.gameObject.name == "Player")
        {
            SetChase();
            //isFigth = true;
            //detectArea.enabled = false;
            //Bang.SetActive(true);
        }
    }

    public void HitByGrenade(Vector3 BoomVec) // 수류탄 공격 받을 시
    {
        CurrentHp -= 50;
        Vector3 DamageVec = this.transform.position - BoomVec;
        DamageEffect(DamageVec, true);
    }


    void DamageEffect(Vector3 DamageVec, bool Grenade = false) // 피격시 넉백 및 색깔변화
    {
        foreach(MeshRenderer mesh in meshs) // 모든 메테리얼의 색깔 변화
            mesh.material.color = Color.red;

        DamageVec = DamageVec.normalized; // normalized : 백터의 방향은 같지만 크기는 1.0을 반환함
        if (Grenade) // 수류탄 피격 시 넉백
        {
            StartCoroutine(HitByGrenade_Effect(DamageVec));
        }
        else // 근접 혹은 원거리 공격 피격 시 넉백
        {
            coroutine = StartCoroutine(NormalAttack_Effect(DamageVec)); // 나중에 코루틴을 멈추기 위해 변수에 담음
        }
    }

    IEnumerator HitByGrenade_Effect(Vector3 DamageVec) // 수류탄 이펙트
    {
        if (CurrentHp > 0)
        {
            isChase = false;
            rigid.AddForce(DamageVec * 8, ForceMode.Impulse);
            yield return new WaitForSeconds(0.2f);
            isChase = true;
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            Die();
            DamageVec += Vector3.up * 3f;
            rigid.AddForce(DamageVec * 5, ForceMode.Impulse);
            rigid.AddTorque(DamageVec * 15, ForceMode.Impulse); // Torque : 회전력(회전하는 힘)
        }
    }

    IEnumerator NormalAttack_Effect(Vector3 DamageVec) // 근접 혹은 원거리 공격 피격 시 이펙트
    {
        if (CurrentHp > 0)
        {
            rigid.AddForce(DamageVec * Random.Range(1f, 2f), ForceMode.Impulse);
            yield return new WaitForSeconds(0.2f);
            foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.white;
        }
        else // 피격으로 사망 시 넉백
        {
            StopCoroutine(coroutine); // else코드 실행 후 위에 if 코드에서 0.2f Wait후 Color가 White가 되는 버그가 있어서 coroutine중지로 버그 방지 
            Die();
            rigid.AddForce(DamageVec * Random.Range(7f, 11f), ForceMode.Impulse);
        }
    }


    void Die()
    {
        rigid.freezeRotation = false; // 회전방지 해제
        isChase = false;
        nav.enabled = false; // 사망 시 리액션을 위해 nav를 false로 변경
        animator.SetTrigger("doDie");
        gameObject.layer = 14; // 죽으면 EnemyDead로 레이어 변경
        isDead = true;
        foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.gray;

        // 점수 상승
        gameManager.score += score;
        // 코인 드랍
        int randomCoin = Random.Range(0, 3);

        SubtrackEnemyCount();
        // Quaternion.identity : 이 Quaternion은 회전 없음을 의미 완벽하게 월드 좌표 축 또는 부모의 축으로 정렬됨
        Instantiate(coin[randomCoin], transform.position + Vector3.up, Quaternion.identity);
        Destroy(gameObject, 3);
    }

    void SubtrackEnemyCount() // 적 카운트 빼기
    {
        switch (enemyType)
        {
            case Type.Normal:
                gameManager.current_NomalEnemy--;
                break;
            case Type.Charge:
                gameManager.current_ChargeEnemy--;
                break;
            case Type.AD:
                gameManager.current_ADEnemy--;
                break;
            case Type.Boss:
                gameManager.current_Boss--;
                break;
        }
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
        if(!isDead && enemyType != Type.Boss)
            Targeting();
    }

    void FreezeVelocity() // 물리 충돌로 nav에 영향이 가지 않도록 하는 함수
    {
        if (isChase)
        {
            rigid.angularVelocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        float targetRadius = 0f;
        float targetRange = 0f;

        switch (enemyType) // Enemy Type에 따라 다른 크기의 Ray부여
        {
            case Type.Normal:
                targetRadius = 1.5f;
                targetRange = 2.5f;
                break;
            case Type.Charge:
                targetRadius = 1f;
                targetRange = 14f;
                break;
            case Type.AD:
                targetRadius = 0.5f;
                targetRange = 25f;
                break;
        }

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack) StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        //Bang.SetActive(false); // 위에 느낌표 제거
        isChase = false; // 공격 시 정지
        isAttack = true;
        animator.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.Normal:
                yield return new WaitForSeconds(0.5f);
                meleeCollider.enabled = true;
                yield return new WaitForSeconds(0.7f);
                meleeCollider.enabled = false;
                yield return new WaitForSeconds(0.5f);
                break;
            case Type.Charge:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeCollider.enabled = true;
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeCollider.enabled = false;
                yield return new WaitForSeconds(2f);
                break;
            case Type.AD:
                yield return new WaitForSeconds(0.5f);
                // 자신의 위치로부터 target의 위치까지 가는 방향 백터를 구함
                Vector3 direction = (target.position - transform.position).normalized;
                // 총알 생성
                GameObject instant_Missile = Instantiate(Missile, transform.position, 
                    Quaternion.LookRotation(direction)); 
                // 총알의 방향과 속도 설정
                Rigidbody missile_Rigid = instant_Missile.GetComponent<Rigidbody>();
                missile_Rigid.velocity = direction * 20;
                yield return new WaitForSeconds(2f);
                break;
        }

        isChase = true;
        isAttack = false;
        animator.SetBool("isAttack", false);
    }
}
