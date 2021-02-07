using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public Player player;
    public enum WeaponsType { Melee, Range}; // enum 열거형 : 상수 숫자들을 보다 의미있는 단어로 표현하여 프로그램을 읽기 쉽게 해준 것
    public WeaponsType weaponsType;
    public int Damage;
    public float 공속;
    public int inBullet;
    public int maxBullet;

    public BoxCollider AttackRange;
    public TrailRenderer trailEffect;

    // 총 관련 변수
    public GameObject Bullet;
    public Transform shotBullet_transform;
    public GameObject bulletCase;
    public Transform shotCase_transform;
    public GameObject BulletBox;

    // 일반적인 함수( Use() ) : 메인루틴( Use() ) -> 서브루틴( Melee() ) ->메인루틴 -> 교차실행
    // 코루틴 : 메인루틴 + 서브루틴(같이 실행됨)   코루틴(co-op) : 함께라는 뜻
    // yield : 결과를 전달하는 키워드 코루틴 내에서 꼭 하나는 있어야함
    // 1
    //yield return null; ; // 1실행 후 1프레임 대기 후 2실행
    // 2

    public void Use()
    {
        if (weaponsType == WeaponsType.Melee)
            StartCoroutine("Melee");
        else if (weaponsType == WeaponsType.Range && 0 < inBullet && !player.isJump)
        {
            inBullet--;
            StartCoroutine("Shot");
        }
        // StopCoroutine("Melee"); : 코루틴 멈추는 함수
    }

    IEnumerator Melee()
    {
        player.isMelee = true;
        yield return new WaitForSeconds(0.1f); 
        trailEffect.enabled = true; // 공격 이펙트 활성화
        yield return new WaitForSeconds(0.25f); 
        AttackRange.enabled = true; // 충돌판정 활성화
        yield return new WaitForSeconds(0.15f);
        AttackRange.enabled = false;
        yield return new WaitForSeconds(0.25f);
        trailEffect.enabled = false;
        player.isMelee = false;
    }

    IEnumerator Shot()
    {
        // 1. 총알발사, Instantiate(생성할 오브젝트, 생성위치, 오브젝트각도) : 게임오브젝트생성
        GameObject shotBullet = InstantiateObject(Bullet, shotBullet_transform);
        Rigidbody BulletRigid = ReturnRigid(shotBullet);
        BulletRigid.velocity = shotBullet_transform.forward * 50; // forward : Z축  shotBullet_transform부터 z축으로 50의 속도로 총알이 날라가게 함

        // 2. 탄피배출
        GameObject shot_BulletCase = InstantiateObject(bulletCase, shotCase_transform);
        Rigidbody CaseRigid = ReturnRigid(shot_BulletCase);
        Vector3 caseVec = shotCase_transform.forward * Random.Range(-3f, -1.5f) + Vector3.up * Random.Range(3f, 1.5f); // 탄피가 생성위치에서 얼마나 튈지 설정
        // Z축의 반대쪽에 힘을주기위해 forward에 -값을 곱하고 탄피가 튀는것을 좀 더 느낌있게 보여주기 위해서 Vector.up의 양수값을 더함
        CaseRigid.AddForce(caseVec, ForceMode.Impulse); // 위에서 설정한 백터값만큼 탄피에 힘을 줌

        yield return new WaitForSeconds(5f); // 발사 후 5초가 지나도 총알이 있으면 삭제
        RemoveBullet(shotBullet);
    }

    GameObject InstantiateObject(GameObject gameobject, Transform transform)
    {
        GameObject Object = Instantiate(gameobject, transform.position, transform.rotation); // 오브젝트 생성
        Object.transform.SetParent(BulletBox.transform); // 생성된 GameObject를 하이라키 창에서 생성될 위치를 가지고있는 오브젝트에 상속시킴
        return Object;
    }

    Rigidbody ReturnRigid(GameObject gameObject)
    {
        return gameObject.GetComponent<Rigidbody>(); 
    }

    void RemoveBullet(GameObject Bullet)
    {
        if (Bullet != null)
            Destroy(Bullet);
    }
}
