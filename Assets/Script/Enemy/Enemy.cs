using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent를 사용하기 위함 

public class Enemy : MonoBehaviour
{
    public int MaxHP;
    public int CurrentHp;
    public Transform Target;
    public bool isChase; // chase : 추적

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

    private void Update()
    {
        if(isChase)
            nav.SetDestination(Target.position); // 도착할 목표의 위치를 지정하는 함수
    }

    void SetChase()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapons weapon = other.GetComponent<Weapons>();
            CurrentHp -= weapon.Damage;

            Vector3 DamageVec = this.transform.position - other.transform.position; // 몬스터 포지션 - 입장에서 맞은방향 계산
            StartCoroutine(OnDamage(DamageVec));
        }
        else if(other.tag == "총알")
        {
            총알삭제 총알 = other.GetComponent<총알삭제>();
            CurrentHp -= 총알.Damage;
            Vector3 DamageVec = this.transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(DamageVec));
        }
    }

    public void HitByGrenade(Vector3 BoomVec)
    {
        CurrentHp -= 50;
        Vector3 DamageVec = this.transform.position - BoomVec;
        StartCoroutine(OnDamage(DamageVec, true));
    }

    IEnumerator OnDamage(Vector3 DamageVec, bool Grenade = false) // 받은 백터값을 조정해 넉백을 줌
    {
        Mat.color = Color.red;
        DamageVec = DamageVec.normalized; // normalized : 백터의 방향은 같지만 크기는 1.0을 반환함
        DamageVec += Vector3.up * 0.5f; // Vector3.up : Vector(0, 1, 0) 
        yield return new WaitForSeconds(0.15f);
        if (Grenade) // 수류탄 피격 시 넉백
        {
            if (CurrentHp <= 0) 
            {
                Mat.color = Color.gray;
                gameObject.layer = 14; // 죽으면 EnemyDead로 레이어 변경 
                Destroy(gameObject, 3);
            }
            else
                Mat.color = Color.white;
            DamageVec += Vector3.up * 2.5f;
            rigid.freezeRotation = false; // 회전방지 해제
            rigid.AddForce(DamageVec * 5, ForceMode.Impulse);
            rigid.AddTorque(DamageVec * 15, ForceMode.Impulse); // Torque : 회전력(회전하는 힘)
        }
        else // 피격 시 넉백
        {
            if(CurrentHp > 0)
            {
                Mat.color = Color.white;
                rigid.AddForce(DamageVec * Random.Range(1f, 2f), ForceMode.Impulse);
            }
            else // 피격으로 사망 시 넉백
            {
                isChase = false;
                animator.SetTrigger("doDie");
                Mat.color = Color.gray;
                gameObject.layer = 14; // 죽으면 EnemyDead로 레이어 변경 
                rigid.AddForce(DamageVec * Random.Range(7f, 11f), ForceMode.Impulse);

                Destroy(gameObject, 3);
            }
        }
    }
}
