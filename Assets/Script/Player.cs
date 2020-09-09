using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;

    private float hAxis;
    private float xAxis;

    Vector3 MoveVec;

    void Update()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        hAxis = Input.GetAxisRaw("Vertical");

        MoveVec = new Vector3(xAxis, 0, hAxis).normalized; // normalized : 방향 값이 1로 보정된 백터(저걸 안하면 대각선 이동 시 평소보다 더 빠르게 이동함)

        transform.position += MoveVec * speed * Time.deltaTime; // transform이동은 Time.dalraTime을 넣어줘야 함
    }
}
