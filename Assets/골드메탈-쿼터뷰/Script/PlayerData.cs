using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Player Data", menuName = "Scriptable Object/Player Data")]
public class PlayerData : ScriptableObject
{
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

    public UnityEvent Death;
    void OnDeath()
    {
        if (Death != null) Death.Invoke();
    }

    // 경험치
    [SerializeField] int currentLV;
    public int CurrentLV => currentLV;
    [SerializeField] int maxXp;
    public int MaxXp => maxXp;
    [SerializeField] int currentXp;
    public int CurrnetXp => CurrnetXp;
}
