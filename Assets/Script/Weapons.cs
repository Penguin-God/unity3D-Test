using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public Player player;
    public enum Type { Melee, Range};
    public Type type;
    public int Damage;
    public float 공속;
    public int Max총알;
    public int 장전된총알;

    public BoxCollider AttackRange;
    public TrailRenderer trailEffect;

    // 총 관련 변수
    public Transform 총알발사위치;
    public GameObject 총알;
    public Transform 탄피배출위치;
    public GameObject 탄피;
    public GameObject BulletBox;

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
        else if (type == Type.Range && 0 < 장전된총알 && !player.isJump)
        {
            장전된총알--;
            StartCoroutine("Shot");
        }
        // StopCoroutine("Melee"); : 코루틴 멈추는 함수
    }

    IEnumerator Melee()
    {
        player.isMelee = true;
        yield return new WaitForSeconds(0.1f); // 콜라이더,이펙트 활성화
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.25f);
        AttackRange.enabled = true; 
        yield return new WaitForSeconds(0.15f);
        AttackRange.enabled = false;
        yield return new WaitForSeconds(0.25f);
        trailEffect.enabled = false;
        player.isMelee = false;
    }

    IEnumerator Shot()
    {
        // 1. 총알발사, Instantiate(생성할 오브젝트, 생성위치, 오브젝트각도) : 게임오브젝트생성
        GameObject 발사할총알 = Instantiate(총알, 총알발사위치.position, 총알발사위치.rotation); // 총알 생성
        발사할총알.transform.SetParent(BulletBox.transform); // 생성된 GameObject를 하이라키 창에서 생성될 위치를 가지고있는 오브젝트에 상속시킴
        Rigidbody BulletRigid = 발사할총알.GetComponent<Rigidbody>(); // 총알의 리지드바디를 가져옴
        BulletRigid.velocity = 총알발사위치.forward * 50; // forward : Z축  총알발사위치부터 z축으로 50의 속도로 총알이 날라가게 함
        yield return null;

        // 2. 탄피배출
        GameObject 배출할탄피 = Instantiate(탄피, 탄피배출위치.position, 탄피배출위치.rotation); // 탄피 생성
        배출할탄피.transform.SetParent(BulletBox.transform);
        Rigidbody BulletCaseRigid = 배출할탄피.GetComponent<Rigidbody>(); 
        Vector3 CaseVec = 탄피배출위치.forward * Random.Range(-3f, -1.5f) + Vector3.up * Random.Range(3f, 1.5f); // 탄피가 생성위치에서 얼마나 튈지 설정
        // Z축의 반대쪽에 힘을주기위해 forward에 -값을 곱하고 탄피가 튀는것을 좀 더 느낌있게 보여주기 위해서 Vector.up의 양수값을 더함

        BulletCaseRigid.AddForce(CaseVec, ForceMode.Impulse); // 위에서 설정한 백터값만큼 탄피에 힘을 줌

        yield return new WaitForSeconds(5f); // 5초가 지나도 총알이 있으면 삭제
        if (발사할총알) 
            Destroy(발사할총알);
    }
}
