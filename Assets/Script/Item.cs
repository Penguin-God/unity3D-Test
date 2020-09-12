using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // enum : 열거형 타입 (타입 이름 지정 필요), 선언은 중괄호 안에 데이터를 열거하듯이 작성
    public enum Type {Ammo, Hammer, Heart, Coin, Waepon };
    public Type type; // enum은 하나의 타입이므로 담을 변수가 따로 필요
    public int value;

    private void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
}
