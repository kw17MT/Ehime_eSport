using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class AICommunicator : MonoBehaviourPunCallbacks
{
    private string m_aiName = "";                       //設定するAIの名前ex.)Player2
    private bool m_isGoaled = false;                    //ゴールしたか
    private bool m_isToldRecord = false;                //ホストにゴールした時間を伝えたか
    private bool m_isMoving = false;                    //現在移動しているか（走行時間の計測用フラグ）
    private bool m_isAttacked = false;                  //このAIは攻撃されたか
    private bool m_isInvincible = false;                //このAIは無敵状態か
    private float m_runningTime = 0.0f;                 //走行時間＝ゴールタイム
    private float m_frameCounter = 0.0f;                //フレーム時間の計測用変数
    private float UPDATE_DISTANCE_TIMING = 0.1f;        //次のウェイポイントとの距離を更新するタイミング
    private float m_distanceToNextWayPoint = 0.0f;      //次のウェイポイントへの距離
    private float m_stiffinTime = 0.0f;                 //硬直している時間
    public float MAX_STIFFIN_TIME = 1.5f;               //攻撃が当たった時の最大硬直時間

    //このAIは攻撃されたかどうかを取得する
    public bool GetIsAttacked()
	{
        return m_isAttacked;
	}

    //このAIは攻撃されたかどうか設定する
    public void SetIsAttacked(bool isAttacked)
	{
        m_isAttacked = isAttacked;
	}

    //このAIの無敵状態をを取得する
    public bool GetIsInvincible()
	{
        return m_isInvincible;
	}

    //このAIを無敵状態かどうか設定する
    public void SetIsInvincible(bool isInvicible)
    {
        m_isInvincible = isInvicible;
    }

    //このAIの次のウェイポイントへの距離を取得する
    public float GetDistanceToNextWayPoint()
	{
        return m_distanceToNextWayPoint;
	}

    //このAIがゴールしたことを設定する
    public void SetGoaled()
	{
        m_isGoaled = true;
	}

    //このAIの名前を取得する
    public string GetAIName()
	{
        return m_aiName;
	}

    //このAIの名前を設定する
    public void SetAIName(string name)
    {
        m_aiName = name;
    }

    //このAIが移動中か設定する
    public void SetMoving(bool isMoving)
	{
        m_isMoving = isMoving;
	}

    //次のウェイポイントを設定する
    public void SetNextWayPoint(int number)
	{
        //オンラインモードならば
        if (!PhotonNetwork.OfflineMode)
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
    }

    //ホストに自分の名前とゴール時間を送信する
    [PunRPC]
    private void TellRecordTime(string name, float time, bool isPlayer)
	{
        GameObject.Find("SceneDirector").GetComponent<InGameScript>().AddGoaledPlayerNameAndRecordTime(name, time, isPlayer);
    }

	private void FixedUpdate()
	{
        //攻撃されていたら
        if (m_isAttacked)
        {
            //硬直時間を加算
            m_stiffinTime += Time.deltaTime;
            //硬直する最大時間がたったら
            if (m_stiffinTime > MAX_STIFFIN_TIME)
            {
                //攻撃されたことを解除
                m_isAttacked = false;
                //硬直時間リセット
                m_stiffinTime = 0.0f;
            }
        }
    }

	// Update is called once per frame
	void Update()
    {
        //現在AIが動いていて、ゴールしていないならば
        if (m_isMoving && !m_isGoaled)
        {
            //走行時間を加算
            m_runningTime += Time.deltaTime;
        }
        //ゴールしていて、タイムをホストに送信していなければ
        if (m_isGoaled && !m_isToldRecord)
		{
            //自分の名前が設定されていたら
            if(m_aiName != "")
			{
                //ホストに自分の名前と時間を送信
                photonView.RPC(nameof(TellRecordTime), RpcTarget.MasterClient, m_aiName, m_runningTime, false);
            }
            //2回以上送信しないようにフラグで制限（オンラインだと何回も呼ばれてしまう）
            m_isToldRecord = true;
            //Debug.Log("AI : " + m_aiName + " Clear : Time " + m_runningTime);
        }

        //インゲーム中で
        if (SceneManager.GetActiveScene().name == "08_GameScene")
        {
            //オフラインモードならば、毎フレーム次のウェイポイントまでの位置を記録する
            if (PhotonNetwork.OfflineMode)
            {
                //次のウェイポイントまでの距離を計算
                Vector3 distance = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;
                //メンバに距離を保存
                m_distanceToNextWayPoint = distance.magnitude;
            }
            //オンラインモードならば
            else
            {
                //経過時間を計測する
                m_frameCounter += Time.deltaTime;
                //ゲームない時間が一定時間たったら（毎フレームカスタムプロパティに保存すると通信料がえぐい）
                if (m_frameCounter >= UPDATE_DISTANCE_TIMING)
                {
                    //プロパティの名前
                    string key = m_aiName + "Distance";
                    //オンラインで取得できるようにカスタムプロパティを更新
                    var hashtable = new ExitGames.Client.Photon.Hashtable();
                    //次のウェイポイントまでの距離を計算
                    Vector3 distance = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;
                    //メンバに距離を保存
                    m_distanceToNextWayPoint = distance.magnitude;
                    //次のウェイポイントへの距離をプレイヤーのカスタムプロパティに保存
                    hashtable[key] = distance.magnitude;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
                    //リセット
                    m_frameCounter = 0.0f;
                }
            }
        }
    }
}