using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polling : MonoBehaviour
{
    public static Polling Instance;
    public GameObject pollingObjectPrefab;

    Queue<PollingBullet> pollingObjectQueue = new Queue<PollingBullet>();

    private void Start()
    {
        Instance = this;
        AddBulletQueue(10);
    }

    private PollingBullet CreateBulletObject()
    {
        var newObject = Instantiate(pollingObjectPrefab, transform).GetComponent<PollingBullet>();
        newObject.gameObject.SetActive(false);
        return newObject;
    }

    void AddBulletQueue(int count)
    {
        for(int i = 0; i < count; i++)
        {
            pollingObjectQueue.Enqueue(CreateBulletObject());
        }
    }

    public static PollingBullet GetObjcet()
    {
        if(Instance.pollingObjectQueue.Count > 0)
        {
            var poolObject = Instance.pollingObjectQueue.Dequeue();
            poolObject.transform.SetParent(null);
            poolObject.gameObject.SetActive(true);
            return poolObject;
        }
        else
        {
            var newObject = Instance.CreateBulletObject();
            newObject.transform.SetParent(null);
            newObject.gameObject.SetActive(true);
            return newObject;
        }
    }

    public static void ReturnObject(PollingBullet pollingBullet)
    {
        pollingBullet.gameObject.SetActive(false);
        pollingBullet.transform.SetParent(Instance.transform);
        Instance.pollingObjectQueue.Enqueue(pollingBullet);
    }
}
