using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntraStage : MonoBehaviour
{
    public Transform[] respawnEnemyPositions;
    public GameObject[] respawnEnemys;
    public Transform Player;
    public Vector3 enterPosition;

    void EnemyRespawn()
    {
        for(int i = 0; i < respawnEnemyPositions.Length; i++)
        {
            int random = Random.Range(0, respawnEnemys.Length);
            Instantiate(respawnEnemys[random], respawnEnemyPositions[i].position, respawnEnemyPositions[i].rotation);
        }
    }

    void ChangePosition()
    {
        Player.position = enterPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            EnemyRespawn();
            ChangePosition();
        }
    }
}
