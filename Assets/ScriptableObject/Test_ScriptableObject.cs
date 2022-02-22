using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Test Data", menuName = "Scriptable Object/TestData")]
public class Test_ScriptableObject : ScriptableObject
{
    [SerializeField] int a;
}
