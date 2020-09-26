using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public Player player;
    public enum Type { Melee, Range};
    public Type type;
    public int Damge;
    public float 공속;
    public BoxCollider AttackRange;
    public TrailRenderer trailEffect;

    // 총 관련 변수
    public Transform 총알발사위치;
    public GameObject 총알;
    public Transform 탄피배출위치;
    public GameObject 탄피;

    // 일반적인 함수( Use() ) : 메인루틴( Use() ) -> 서브루틴( Melee() ) ->메인루틴 -> 교차실행
    // 코루틴 : 메인루틴 + 서브루틴(같이 실행됨)   코루틴(co-op) : 함께라는 뜻
    // yield : 결과를 전달하는 키워드 코루틴 내에서 꼭 하나는 있어야함
    // 1
    //yield return null; ; // 1실행 후 1프레임 대기 후 2실행
    // 2
    public void Use()
    {
        if (type == Type.Melee)
            StartCoroutine("Melee");
        else if (type == Type.Range)
            StartCoroutine("Shot");
        // StopCoroutine("Melee"); : 코루틴 멈추는 함수
    }

    IEnumerator Melee()
    {
        player.isAttack = true;
        player.speed *= 0.5f;
        yield return new WaitForSeconds(0.1f);
        AttackRange.enabled = true; // 콜라이더 두개 활성화
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        AttackRange.enabled = false;
        yield return new WaitForSeconds(0.3f);
        player.isAttack = false;
        player.speed *= 2f;
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        // 1. 총알발사, Instantiate(생성할 오브젝트, 생성위치, 오브젝트각도) : 게임오브젝트생성
        GameObject 발사할총알 = Instantiate(총알, 총알발사위치.position, 총알발사위치.rotation);
        Rigidbody BulletRigid = 발사할총알.GetComponent<Rigidbody>();
        BulletRigid.velocity = 총알발사위치.forward * 50; // forward : Z축
        yield return null;

        // 2. 탄피배출
        GameObject 배출할탄피 = Instantiate(탄피, 탄피배출위치.position, 탄피배출위치.rotation);
        Rigidbody BulletCaseRigid = 배출할탄피.GetComponent<Rigidbody>();
        Vector3 CaseVec = 탄피배출위치.forward * Random.Range(-3f, -1.5f) + Vector3.up * Random.Range(3f, 1.5f);
        // Z축의 반대쪽에 힘을주기위해 forward에 -값을 곱하고 탄피가 튀는것을 좀 더 느낌있게 보여주기 위해서 Vector.up의 양수값을 더함
        BulletCaseRigid.AddForce(CaseVec, ForceMode.Impulse);
        //BulletCaseRigid.AddTorque(Vector3.up * Random.Range(1, 3)); // AddTorque : 회전함수
    }
}
