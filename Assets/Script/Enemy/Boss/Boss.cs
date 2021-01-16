using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject bossMissile;
    public Transform missilePortA;
    public Transform missilePortB;

    BoxCollider boxCollider;
    Vector3 lookVec;
    Vector3 tauntVec;
    bool isLook = true;

    private void Awake() // 상속 시 Awake함수는 자식의 함수만 작동함
    {
        boxCollider = GetComponent<BoxCollider>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>(); // Material은 MeshRenderer에서 가져와야 됨
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Pattern());
    }

    private void Update()
    {
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5;
            transform.LookAt(Target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec);
    }

    IEnumerator Pattern()
    {
        yield return new WaitForSeconds(0.1f);
        int patternNumber = Random.Range(0, 5);
        switch (patternNumber)
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(JumpAttack());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        animator.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(this.bossMissile, missilePortA.position, missilePortA.rotation);
        BossMissile bossmissileA = instantMissileA.GetComponent<BossMissile>();
        bossmissileA.target = Target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(this.bossMissile, missilePortB.position, missilePortB.rotation);
        BossMissile bossmissileB = instantMissileB.GetComponent<BossMissile>();
        bossmissileB.target = Target;

        yield return new WaitForSeconds(2f);
        StartCoroutine(Pattern());
    }
    IEnumerator RockShot()
    {
        animator.SetTrigger("doBigShot");
        isLook = false;
        Instantiate(Missile, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Pattern());
    }
    IEnumerator JumpAttack()
    {
        tauntVec = Target.position + lookVec;
        animator.SetTrigger("doTaunt");
        boxCollider.enabled = false;
        isLook = false;
        nav.isStopped = false;

        yield return new WaitForSeconds(1.5f);
        meleeCollider.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeCollider.enabled = false;

        yield return new WaitForSeconds(1f);
        boxCollider.enabled = true;
        isLook = true;
        nav.isStopped = true;
        StartCoroutine(Pattern());
    }
}
