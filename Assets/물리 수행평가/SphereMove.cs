using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMove : MonoBehaviour
{
    Vector3 toCubeDirection;
    public GameObject Cube;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        toCubeDirection = (Cube.transform.position - transform.position).normalized;
        Vector3 LookDirection = (transform.position - Cube.transform.position).normalized;
        rigid.velocity = toCubeDirection + Vector3.forward;
        transform.LookAt(Vector3.Cross(Vector3.right, LookDirection));
        Debug.Log(Vector3.Cross(Vector3.right, LookDirection));
    }
}
