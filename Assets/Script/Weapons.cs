using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public enum Type { Melee, Range};
    public Type type;
    public int Damge;
    public float 공속;
    public BoxCollider2D AttackRange;
    public TrailRenderer trailEffect;

}
