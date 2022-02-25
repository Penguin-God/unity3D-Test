using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Player Data", menuName = "Scriptable Object/Player Data")]
public class PlayerData : ScriptableObject
{
    // 체력
    [SerializeField] int maxHp;
    public int MaxHp => maxHp;

    [SerializeField] int currentHp;
    public int CurrentHp => currentHp;

    public UnityEvent<bool> OnDamageEvent;
    public void OnDamage(int _damage, bool _isBoss)
    {
        currentHp -= _damage;
        if (OnDamageEvent != null) OnDamageEvent.Invoke(_isBoss);

        if (currentHp <= 0) OnDeath();
    }

    public void RestoreHeath(int _newHeath)
    {
        currentHp += _newHeath;
    }



    // 사망
    public UnityEvent DeathEvent;

    [SerializeField] bool isDead = false;
    public bool IsDead => isDead;
    void OnDeath()
    {
        isDead = true;
        if (DeathEvent != null) DeathEvent.Invoke();
    }



    // 경험치
    [SerializeField] int currentLV;
    public int CurrentLV => currentLV;
    [SerializeField] int maxXp;
    public int MaxXp => maxXp;
    [SerializeField] int currentXp;
    public int CurrnetXp => CurrnetXp;
}
