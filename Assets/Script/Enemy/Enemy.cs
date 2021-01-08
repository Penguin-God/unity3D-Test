using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent를 사용하기 위함 

public class Enemy : MonoBehaviour
{
    public int MaxHP;
    public int CurrentHp;
    public Transform Target;
    public bool isChase; // chase : 추적, 추적기능과 물리설정의 조건 역할

    BoxCollider box;
    Rigidbody rigid;
    Material Mat;
    NavMeshAgent nav;
    Animator animator;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
        rigid = GetComponent<Rigidbody>();
        Mat = GetComponentInChildren<MeshRenderer>().material; // Material은 MeshRenderer에서 가져와야 됨
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        Invoke("SetChase", 2); 
    }
    void SetChase()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    private void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(Target.position); // 도착할 목표의 위치를 지정하는 함수
            nav.isStopped = !isChase; // true일 때 정지
        }
    }

    private void OnTriggerEnter(Collider other) // 몬스터가 플레이어한터 피격 당함
    {
        if(other.tag == "Melee")
        {
            Weapons weapon = other.GetComponent<Weapons>();
            CurrentHp -= weapon.Damage;

            Vector3 DamageVec = this.transform.position - other.transform.position; // 몬스터 포지션 - 입장에서 맞은방향 계산
            DamageEffect(DamageVec);
        }
        else if(other.tag == "총알")
        {
            총알삭제 총알 = other.GetComponent<총알삭제>();
            CurrentHp -= 총알.Damage;
            Vector3 DamageVec = this.transform.position - other.transform.position;
            Destroy(other.gameObject); 
            DamageEffect(DamageVec);
        }
    }

    public void HitByGrenade(Vector3 BoomVec) // 수류탄 공격 받을 시
    {
        CurrentHp -= 50;
        Vector3 DamageVec = this.transform.position - BoomVec;
        DamageEffect(DamageVec, true);
    }


    // 이 코루틴 나중에 나눠야 함
    void DamageEffect(Vector3 DamageVec, bool Grenade = false) // 피격시 넉백 및 색깔변화
    {
        Mat.color = Color.red;
        DamageVec = DamageVec.normalized; // normalized : 백터의 방향은 같지만 크기는 1.0을 반환함
        if (Grenade) // 수류탄 피격 시 넉백
        {
            StartCoroutine(HitByGrenade_Effect(DamageVec));
        }
        else // 근접 혹은 원거리 공격 피격 시 넉백
        {
            StartCoroutine(NormalAttack_Effect(DamageVec));
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
            Mat.color = Color.white;
        }
        else
        {
            EnemyDie();
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
            Mat.color = Color.white;
        }
        else // 피격으로 사망 시 넉백
        {
            EnemyDie();
            rigid.AddForce(DamageVec * Random.Range(7f, 11f), ForceMode.Impulse);
        }
    }


    void EnemyDie()
    {
        rigid.freezeRotation = false; // 회전방지 해제
        isChase = false;
        nav.enabled = false; // 사망 시 리액션을 위해 nav를 false로 변경
        animator.SetTrigger("doDie");
        Mat.color = Color.gray;
        gameObject.layer = 14; // 죽으면 EnemyDead로 레이어 변경 
        Destroy(gameObject, 3);
    }


    private void FixedUpdate()
    {
        FreezeVelocity();
    }

    void FreezeVelocity() // 물리 충돌로 nav에 영향이 가지 않도록 하는 함수
    {
        if (isChase)
        {
            rigid.angularVelocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
        }
    }
}
