using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int MaxHP;
    public int CurrentHp;

    BoxCollider box;
    Rigidbody rigid;
    Material Mat;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
        rigid = GetComponent<Rigidbody>();
        Mat = GetComponent<MeshRenderer>().material; // Material은 MeshRenderer에서 가져와야 됨
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapons weapon = other.GetComponent<Weapons>();
            CurrentHp -= weapon.Damage;
            StartCoroutine(OnDamage(DamageVec));
        }
        else if(other.tag == "총알")
        {
            총알 총알 = other.GetComponent<총알>();
            CurrentHp -= 총알.Damage;
        DamageVec = DamageVec.normalized; // normalized : 백터의 방향은 같지만 크기는 1.0을 반환함
        DamageVec += Vector3.up * 0.5f; // Vector3.up : Vector(0, 1, 0) 
            rigid.AddForce(DamageVec * Random.Range(1f, 2f), ForceMode.Impulse);
            rigid.AddForce(DamageVec * Random.Range(7f, 15f), ForceMode.Impulse);
        }
    }
}
