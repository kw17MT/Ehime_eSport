using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デバッグ用　法線を描画するオブジェクトのマネージャーオブジェクト
/// </summary>
public class NormalDebugManager : MonoBehaviour
{
    //法線描画オブジェクトのインスタンス
    public GameObject m_nornalDebugInstance;

    /// <summary>
    /// 法線描画オブジェクトの生成
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <param name="normal">法線の向き</param>
    public void Spawn(Vector3 position,Vector3 normal)
    {
        //法線描画するオブジェクトを生成
        GameObject normalDebugObject = GameObject.Instantiate(m_nornalDebugInstance, position,Quaternion.identity);

        //法線の向きをセット
        normalDebugObject.GetComponent<NormalDebug>().Normal = normal;
    }
}
