using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject bulletPerfab;
    private Camera Camera;

    private void Awake()
    {
        Camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out raycastHit))
            {
                var dir = new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z);
                var bullet = Polling.GetObjcet(); //Instantiate(bulletPerfab, transform.position + dir.normalized, Quaternion.identity).GetComponent<PollingBullet>();
                bullet.transform.position = transform.position + dir.normalized;
                bullet.Shot(dir);
            }
        }
    }
}
