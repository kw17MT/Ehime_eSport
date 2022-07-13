using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI用に作ったカメラスクリプト
/// </summary>
public class AICameraScript : MonoBehaviour
{
    const int m_kElementCount = 10;             //配列の要素数

    private Vector3[] m_prevPlayerForward;      //過去のプレイヤーの前方向

    public GameObject m_AIPlayer;               //カメラで追うプレイヤー

    public float m_cameraBackLength = 5.0f;     //カメラの位置をプレイヤーの位置から後ろ向きにずらす距離

    public float m_cameraHeight = 10.0f;        //カメラのプレイヤーの位置からの高さ

    private void Start()
    {
        //配列の要素数を初期化
        m_prevPlayerForward = new Vector3[m_kElementCount];

        //スタート時の向きで埋める
        Vector3 camPos = m_AIPlayer.transform.forward;
        for(int i = 0;i< m_kElementCount; i++)
        {
            m_prevPlayerForward[i] = camPos;
        }
        
    }

    void FixedUpdate()
    {
        //現在の向きを取得
        Vector3 camPos = m_AIPlayer.transform.forward;

        //更新
        for(int i = 1;i< m_kElementCount; i++)
        {
            m_prevPlayerForward[i - 1] = m_prevPlayerForward[i];
        }

        //最新の向きを挿入
        m_prevPlayerForward[m_kElementCount - 1] = camPos;

        //平均を計算
        Vector3 posSum = Vector3.zero;
        for(int i = 0;i< m_kElementCount; i++)
        {
            posSum += m_prevPlayerForward[i];
        }

        posSum /= m_kElementCount;

        //平均の向きをもとにカメラの座標をセット
        this.gameObject.transform.position = m_AIPlayer.transform.position + posSum * -m_cameraBackLength + new Vector3(0.0f, m_cameraHeight, 0.0f);
        
        //注視点をセット
        this.gameObject.transform.LookAt(m_AIPlayer.transform);
    }
}
