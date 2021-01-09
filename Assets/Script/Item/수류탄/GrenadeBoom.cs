using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBoom : MonoBehaviour
{
    public GameObject MashObj;
    public GameObject EffectObj;
    public new Rigidbody rigidbody;

    private void Start()
    {
        StartCoroutine(Boom()); // Player Script에서 Instantiate()함수로 생성하는 거라서 Start에서 코루틴을 사용함
    }

    IEnumerator Boom()
    {
        yield return new WaitForSeconds(3f);
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        MashObj.SetActive(false);
        EffectObj.SetActive(true);

        RaycastHit[] rayHits = Physics.SphereCastAll(this.transform.position, 10, Vector3.up, 0f, LayerMask.GetMask("Enemy")); // 구 모양의 Ray를 쏨
        foreach(RaycastHit hitObjs in rayHits)
        {
            hitObjs.transform.GetComponent<Enemy>().HitByGrenade(this.transform.position);
        }
    }

    private void OnDrawGizmos() // Boom 코루틴에 있는 ray 씬에 그리는 함수
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, 10); // Boom 코루틴에 있는 rayHits Draw
    }
}
