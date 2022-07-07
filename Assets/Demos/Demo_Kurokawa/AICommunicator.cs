using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class AICommunicator : MonoBehaviourPunCallbacks
{
    private string m_aiName = "";
    private float m_runningTime = 0.0f;
    private bool m_isGoaled = false;
    private bool m_isToldRecord = false;
    private bool m_isMoving = false;
    private float m_frameCounter = 0.0f;
    private float UPDATE_DISTANCE_TIMING = 0.5f;        //次のウェイポイントとの距離を更新するタイミング

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetGoaled()
	{
        m_isGoaled = true;
	}
    public void SetAIName(string name)
	{
        m_aiName = name;
	}

    public string GetAIName()
	{
        return m_aiName;
	}

    public void SetMoving(bool isMoving)
	{
        m_isMoving = isMoving;
	}

    public void SetNextWayPoint(int number)
	{
        //次のウェイポイントの番号をルームプロパティに保存
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        //プレイヤー名＋WayPointNumberという名前を作成 ex.)Player2WayPointNumber
        string name = m_aiName + "WayPointNumber";
        //ウェイポイント番号を設定
        hashtable[name] = number;
        //ルームプロパティの更新
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isMoving && !m_isGoaled)
        {
            m_runningTime += Time.deltaTime;
        }

        if (m_isGoaled && !m_isToldRecord)
		{
            m_isToldRecord = true;
            photonView.RPC(nameof(AvatarController.TellRecordTime), RpcTarget.MasterClient, m_aiName, m_runningTime);
            Debug.Log("AI : " + m_aiName + " Clear : Time " + m_runningTime);
        }

        //インゲームならば
        if (SceneManager.GetActiveScene().name == "08_GameScene")
        {
            //経過時間を計測する
            m_frameCounter += Time.deltaTime;
            //ゲームない時間が一定時間たったら
            if (m_frameCounter >= UPDATE_DISTANCE_TIMING)
            {
                //プロパティの名前
                string key = m_aiName + "Distance";
                //オンラインで取得できるようにカスタムプロパティを更新
                var hashtable = new ExitGames.Client.Photon.Hashtable();
                //次のウェイポイントへの距離をプレイヤーのカスタムプロパティに保存
                hashtable[key] = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

                //リセット
                m_frameCounter = 0.0f;
            }
        }
    }
}