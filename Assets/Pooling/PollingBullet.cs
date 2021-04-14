using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollingBullet : MonoBehaviour
{
    Vector3 direction;
    public void Shot(Vector3 dir)
    {
        direction = dir;
        Invoke("DestroyBullet", 5);
    }

    private void Update()
    {
        transform.Translate(direction);
    }

    private void DestroyBullet()
    {
        Polling.ReturnObject(this);
    }
}
