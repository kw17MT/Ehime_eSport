using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AIProgressChecker : MonoBehaviourPunCallbacks
{
    private int m_lapCount = 0;
    private bool m_canGoaled = false;

    public int MAX_LAP_NUM = 3;

    public void SetCanGoaled()
	{
        m_canGoaled = true;
	}

    public int GetLapCount()
	{
        return m_lapCount;
	}
    public void CheckCanGoal()
	{
        if(m_canGoaled)
		{
            //ラップ数を加算
            m_lapCount++;

            //Debug.Log(this.gameObject.GetComponent<AICommunicator>().GetAIName() + "  Count =  " + m_lapCount);
            if (!PhotonNetwork.OfflineMode)
            {
                //次のウェイポイントの番号をルームプロパティに保存
                var hashtable = new ExitGames.Client.Photon.Hashtable();
                //プレイヤー名＋LapCountという名前を作成 ex.)Player2LapCount
                string name = this.gameObject.GetComponent<AICommunicator>().GetAIName() + "LapCount";
                //ウェイポイント番号を設定
                hashtable[name] = m_lapCount;
                //ルームプロパティの更新
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }

            if (m_lapCount >= MAX_LAP_NUM)
			{
                this.GetComponent<AICommunicator>().SetGoaled();
			}

            m_canGoaled = false;

            //Debug.Log(m_lapCount);
        }
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
