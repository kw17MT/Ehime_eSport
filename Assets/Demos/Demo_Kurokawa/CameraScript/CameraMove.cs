using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Camera m_camera;                                                  //使用するメインカメラ
    private GameObject m_ownPlayer = null;                                  //追随対象のゲームオブジェクト（プレイヤー）
    private bool m_isGetOwnPlayer = false;                                  //プレイヤーインスタンスが確保できたか
    private Vector3 m_prevPlayerForward = Vector3.zero;                     //前フレームのプレイヤーの前方向
    private Vector3 m_prevCameraPos = Vector3.zero;                         //前フレームのカメラの位置
    [SerializeField] float BEHIND_RATE_FROM_PLAYER = 5.0f;                  //カメラの位置をどのくらいプレイヤーの後ろにするか
    [SerializeField] float UPPER_RATE_FROM_PLAYER = 5.0f;                   //カメラの位置をどのくらいプレイヤーの上にするか

	private void Start()
	{
        //メインカメラを取得
        m_camera = Camera.main;
    }

    private void FindOwnPlayer()
	{
        //通信の関係上、ここでインスタンスが確保できるまで探す
        if (!m_isGetOwnPlayer)
        {
            //自分のインスタンスを取得
            m_ownPlayer = GameObject.Find("OwnPlayer");
            if (m_ownPlayer != null)
            {
                m_isGetOwnPlayer = true;
            }
        }
    }

    private void FollowPlayer()
	{
        Vector3 cameraPos;
        //プレイヤーが攻撃されていなければ
        if (!m_ownPlayer.GetComponent<AvatarController>().GetIsAttacked())
        {
            //カメラの位置はプレイヤーの少し後ろの位置で
            cameraPos = m_ownPlayer.transform.position + (m_ownPlayer.transform.forward * -1.0f) * BEHIND_RATE_FROM_PLAYER;
            //少し高く設定する。
            cameraPos.y += UPPER_RATE_FROM_PLAYER;
            //プレイヤーの前方向を記録
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
        m_camera.gameObject.transform.position = Vector3.Lerp(cameraPos, m_prevCameraPos, 0.1f);
        m_prevCameraPos = m_camera.gameObject.transform.position;

        if (!m_ownPlayer.GetComponent<AvatarController>().GetIsAttacked())
        {
            //注目対象はプレイヤーにする
            m_camera.gameObject.transform.LookAt(m_ownPlayer.transform);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //自分のプレイヤーインスタンスを探す
        FindOwnPlayer();

        //ゲームを終了した後、前のシーンに戻るときにヌルを参照しないようにするため
        if (m_ownPlayer == null)
		{
            return;
		}

        FollowPlayer();
    }
}
