using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntraStage : MonoBehaviour
{
    public Transform[] RespawnEnemyPositions;
    public GameObject[] RespawnEnemys;

    void EnemyRespawn()
    {
        for(int i = 0; i < RespawnEnemyPositions.Length; i++)
        {
            int random = Random.Range(0, RespawnEnemys.Length);
            Instantiate(RespawnEnemys[random], RespawnEnemyPositions[i].position, RespawnEnemyPositions[i].rotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            EnemyRespawn();
        }
    }
}
