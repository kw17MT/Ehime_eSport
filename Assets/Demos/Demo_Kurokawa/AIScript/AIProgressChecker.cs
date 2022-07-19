using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AIProgressChecker : MonoBehaviourPunCallbacks
{
    private int m_lapCount = 0;                     //AIの周回数
    private bool m_canGoaled = false;               //AIはゴールできるか
    [SerializeField] int MAX_LAP_NUM = 3;                     //AIのゴールラップ数

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
        //ゴールできるならば
        if(m_canGoaled)
		{
            //ラップ数を加算
            m_lapCount++;
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
            //次のゴール判定のためにフラグをオフ
            m_canGoaled = false;
        }
	}
}
