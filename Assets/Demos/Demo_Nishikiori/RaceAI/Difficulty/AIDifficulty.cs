using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AIDifficulty")]
public class SpawnManagerScriptableObject : ScriptableObject
{
    //�����̉����̕��܂ŖڕW�n�_�ɂȂ蓾�邩
    [Range(-1.0f,1.0f)]
    public float innerShiftMaxRatio;

    //�O���̉����̕��܂ŖڕW�n�_�ɂȂ蓾�邩
    [Range(-1.0f, 1.0f)]
    public float outerShiftMaxRatio;
}
