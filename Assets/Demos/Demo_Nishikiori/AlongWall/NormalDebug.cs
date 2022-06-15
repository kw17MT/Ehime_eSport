using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デバッグ用　法線を描画するオブジェクト
/// </summary>
public class NormalDebug : MonoBehaviour
{
    //描画する法線
    private Vector3 m_normal = new Vector3(1.0f,0.0f,0.0f);

    //法線のプロパティ
    public Vector3 Normal
    { 
        get
        {
            return this.m_normal;
        }
        set
        {
            //正規化する
            m_normal = value.normalized;
        }
    }

    //描画する法線の長さ
    public float m_lineLength = 2.5f;

    // Update is called once per frame
    void Update()
    {
        //法線を描画する
        Debug.DrawRay(transform.position, m_normal * m_lineLength, Color.magenta);
    }
}
