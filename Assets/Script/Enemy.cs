using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int MaxHP;
    public int CurrentHp;

    BoxCollider box;
    Rigidbody rigid;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
        rigid = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapons weapon = other.GetComponent<Weapons>();
            CurrentHp -= weapon.Damage;
        }
        else if(other.tag == "총알")
        {
            총알 총알 = other.GetComponent<총알>();
            CurrentHp -= 총알.Damage;
        }
    }
}
