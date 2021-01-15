using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : 총알삭제
{
    public Transform target;
    NavMeshAgent nav;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        nav.SetDestination(target.position);
    }
}
