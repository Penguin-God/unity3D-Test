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
    }
}
