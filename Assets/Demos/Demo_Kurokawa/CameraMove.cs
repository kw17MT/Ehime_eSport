using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private GameObject m_ownPlayer = null;        //追随対象のゲームオブジェクト（プレイヤー）
    private bool m_isGetOwnPlayer = false;        //プレイヤーインスタンスが確保できたか

    public float BEHIND_RATE_FROM_PLAYER = 8.0f; //カメラの位置をどのくらいプレイヤーの後ろにするか
    public float UPPER_RATE_FROM_PLAYER = 5.0f;   //カメラの位置をどのくらいプレイヤーの上にするか
    
    // Update is called once per frame
    void Update()
    {
        //通信の関係上、ここでインスタンスが確保できるまで探す
        if(!m_isGetOwnPlayer)
		{
            m_ownPlayer = GameObject.Find("OwnPlayer");
            if(m_ownPlayer != null)
			{
                m_isGetOwnPlayer = true;
            }
        }
      
        //カメラの位置はプレイヤーの少し後ろの位置で
        Vector3 cameraPos = m_ownPlayer.transform.position + (m_ownPlayer.transform.forward * -1.0f) * BEHIND_RATE_FROM_PLAYER;
        //少し高く設定する。
        cameraPos.y += UPPER_RATE_FROM_PLAYER;

        //メインカメラを取得
        Camera camera = Camera.main;
        //位置を設定し
        camera.gameObject.transform.position = cameraPos;
        //注目対象はプレイヤーにする
        camera.gameObject.transform.LookAt(m_ownPlayer.transform);
    }
}
