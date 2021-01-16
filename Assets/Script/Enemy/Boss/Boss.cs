using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject bossMissile;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookVec;
    Vector3 tauntVec;
    bool isLook = true;

    private void Awake() // 상속 시 Awake함수는 자식의 함수만 작동함
    {
        StartCoroutine(Pattern());
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>(); // Material은 MeshRenderer에서 가져와야 됨
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
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
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(Pattern());
    }
    IEnumerator RockShot()
    {
        animator.SetTrigger("doBigShot");
        yield return new WaitForSeconds(3f);
        StartCoroutine(Pattern());
    }
    IEnumerator JumpAttack()
    {
        animator.SetTrigger("doTaunt");
        yield return new WaitForSeconds(3f);
        StartCoroutine(Pattern());
    }
}
