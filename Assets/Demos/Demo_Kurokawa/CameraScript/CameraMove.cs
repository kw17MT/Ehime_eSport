using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Camera camera;
    private GameObject m_ownPlayer = null;        //追随対象のゲームオブジェクト（プレイヤー）
    private bool m_isGetOwnPlayer = false;        //プレイヤーインスタンスが確保できたか
    private Vector3 m_prevPlayerForward = Vector3.zero;
    private Vector3 m_prevCameraPos = Vector3.zero;

    public float BEHIND_RATE_FROM_PLAYER = 5.0f; //カメラの位置をどのくらいプレイヤーの後ろにするか
    public float UPPER_RATE_FROM_PLAYER = 5.0f;   //カメラの位置をどのくらいプレイヤーの上にするか

	private void Start()
	{
        //メインカメラを取得
        camera = Camera.main;
    }

	// Update is called once per frame
	void Update()
    {
        //通信の関係上、ここでインスタンスが確保できるまで探す
        if(!m_isGetOwnPlayer)
		{
            //自分のインスタンスを取得
            m_ownPlayer = GameObject.Find("OwnPlayer");
            if(m_ownPlayer != null)
			{
                m_isGetOwnPlayer = true;
            }
        }

        //ゲームを終了した後、前のシーンに戻るときにヌルを参照しないようにするため
        if(m_ownPlayer == null)
		{
            return;
		}

        Vector3 cameraPos;
        if (!m_ownPlayer.GetComponent<AvatarController>().GetIsAttacked())
		{
            //カメラの位置はプレイヤーの少し後ろの位置で
            cameraPos = m_ownPlayer.transform.position + (m_ownPlayer.transform.forward * -1.0f) * BEHIND_RATE_FROM_PLAYER;
            //少し高く設定する。
            cameraPos.y += UPPER_RATE_FROM_PLAYER;

            m_prevPlayerForward = m_ownPlayer.transform.forward;
        }
		else
		{
            //カメラの位置はプレイヤーの少し後ろの位置で
            cameraPos = m_ownPlayer.transform.position + (m_prevPlayerForward * -1.0f) * BEHIND_RATE_FROM_PLAYER;
            //少し高く設定する。
            cameraPos.y += UPPER_RATE_FROM_PLAYER;
        }

        //位置を設定し
        camera.gameObject.transform.position = Vector3.Lerp(cameraPos, m_prevCameraPos, 0.1f);
        m_prevCameraPos = camera.gameObject.transform.position;

        if (!m_ownPlayer.GetComponent<AvatarController>().GetIsAttacked())
		{
            //注目対象はプレイヤーにする
            camera.gameObject.transform.LookAt(m_ownPlayer.transform);
        }

    }
}
