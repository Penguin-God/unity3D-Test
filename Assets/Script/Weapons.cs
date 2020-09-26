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
        trailEffect.enabled = false;
        player.isAttack = false;
        player.speed *= 2f;

    }
}
