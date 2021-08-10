using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Moveable : MonoBehaviour
{
    NavMeshAgent nav;
    [SerializeField]
    Transform target = null;
    Rigidbody rigid;
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.DrawRay(transform.position, target.position - transform.position, Color.green, 30f);
            //Physics.Raycast(transform.position, target.position - transform.position, out RaycastHit targetHit, 30f);
            //nav.SetDestination(targetHit.point);
            nav.SetDestination(target.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            //nav.isStopped = true;
        }
    }
}
