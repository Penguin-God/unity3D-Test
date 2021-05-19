using UnityEngine;
using System.Collections;

public class Parabola : MonoBehaviour
{
    Transform makePos;

    bool m_bStartCheck = false;

    public float Angle = 45.0f; // ?? 앵글이 45도인데 왜 45도로 셋팅해서 하는지는 모르겠다.
    public float Power = 0.01f;// 기본 파워값 

    float Timedir; //스타트에서 중력값을 계산하려고 한번 받아놓는 듯
    float Gravity;//중력값

    Vector3 v1;
    public void SetArrowCheck(bool check, Vector3 pos, Quaternion q)
    { //화살을 불러오는 함수
        m_bStartCheck = check;
        transform.position = pos;
        transform.rotation = q;
    }
    public bool GetArrowCheck() { return m_bStartCheck; }

    void Start()
    {
        Timedir = Time.deltaTime; //스타트에서 중력값을 계산하려고 한번 받아놓는 듯
        //makePos = GameObject.Find("BulletManager").transform;
        Gravity = -(1.0f * Timedir * Timedir / 2.0f);//고정 중력값
                                                     //리얼한 중력값을 구하려고 이런식으로 하는 거 같다. 중력 계산값은
                                                     //0,98888로 하면 더 리얼한 중력값을 받게끔 할 수 있다.
    }

    void Update()
    {
        //if (m_bStartCheck == false) return;
        Timedir += Time.deltaTime;
        v1.z = Power = Mathf.Cos(Angle * Mathf.PI / 180.0f) * Timedir * Time.deltaTime;//핵심코드
        v1.y = Power = Mathf.Cos(Angle * Mathf.PI / 180.0f) * Timedir * Gravity * Time.deltaTime;//핵심코드
        transform.Translate(v1);

        transform.Rotate(new Vector3(Mathf.Cos(Angle * Mathf.PI / 180.0f), 0, 0));//핵심코드
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Debug.Log("헐");
            transform.position = makePos.position;
            v1 = new Vector3(0, 0, 0);//충돌하였으니 초기화
            Power = 0.0f;//충돌하였으니 초기화
            m_bStartCheck = false;
        }
        else if (col.tag != "Enemy")
        {
            transform.position = makePos.position;
            v1 = new Vector3(0, 0, 0);//충돌하였으니 초기화
            Power = 0.0f;//충돌하였으니 초기화
            m_bStartCheck = false;
            Debug.Log(col.tag);
        }
    }
}