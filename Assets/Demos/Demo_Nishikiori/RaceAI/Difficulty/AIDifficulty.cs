using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AIDifficulty")]
public class SpawnManagerScriptableObject : ScriptableObject
{
    //内側の何割の幅まで目標地点になり得るか
    [Range(-1.0f,1.0f)]
    public float innerShiftMaxRatio;

    //外側の何割の幅まで目標地点になり得るか
    [Range(-1.0f, 1.0f)]
    public float outerShiftMaxRatio;
}
