using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTrigger : MonoBehaviour
{
    [SerializeField]
    Rigidbody rigid = null;

    private void OnTriggerEnter(Collider other)
    {
        rigid.useGravity = true;
    }
}
