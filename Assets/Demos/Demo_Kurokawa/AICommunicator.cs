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
    private float UPDATE_DISTANCE_TIMING = 0.1f;        //次のウェイポイントとの距離を更新するタイミング

    private float m_distanceToNextWayPoint = 0.0f;

    private bool m_isAttacked = false;
    private bool m_isInvincible = false;

    private float m_stiffinTime = 0.0f;
    public float MAX_STIFFIN_TIME = 1.5f;               //攻撃が当たった時の最大硬直時間

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool GetIsAttacked()
	{
        return m_isAttacked;
	}

    public void SetIsAttacked(bool isAttacked)
	{
        m_isAttacked = isAttacked;
	}

    public void SetIsInvincible(bool isInvicible)
	{
        m_isInvincible = isInvicible;
	}

    public bool GetIsInvincible()
	{
        return m_isInvincible;
	}

    public float GetDistanceToNextWayPoint()
	{
        return m_distanceToNextWayPoint;
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

    // Update is called once per frame
    void Update()
    {
        if (m_isAttacked)
		{
            m_stiffinTime += Time.deltaTime;
            if(m_stiffinTime > MAX_STIFFIN_TIME)
			{
                m_isAttacked = false;

                m_stiffinTime = 0.0f;
			}
		}


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
            //オフラインモードならば、毎フレーム次のウェイポイントまでの位置を記録する
            if (PhotonNetwork.OfflineMode)
            {
                Vector3 distance = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;
                m_distanceToNextWayPoint = distance.magnitude;
            }
            else
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


                    Vector3 distance = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;

                    m_distanceToNextWayPoint = distance.magnitude;

                    //次のウェイポイントへの距離をプレイヤーのカスタムプロパティに保存
                    hashtable[key] = distance.magnitude;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

                    //Vector3 a = (Vector3)hashtable[key];
                    //Debug.Log("SuccessSetDistance : " + hashtable[key] + "    " + a.magnitude);

                    //リセット
                    m_frameCounter = 0.0f;
                }
            }
        }
    }
}