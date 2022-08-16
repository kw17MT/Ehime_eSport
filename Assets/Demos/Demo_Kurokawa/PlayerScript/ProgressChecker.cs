using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

//このスクリプトが割り当てられたインスタンスのチェックポイント管理インスタンス
public class ProgressChecker : MonoBehaviour
{
    private GameObject m_lapLabel = null;
    private GameObject m_lapCountLabel = null;
    private float m_displayTime = 0.0f;
    private bool m_isDisplayingLapLabel = false;

    private int m_lapCount = 0;                             //ゲーム中の周回回数
    [SerializeField] int MAX_CHECKPOINT_NUM = 3;            //ステージに配置されるチェックポイントの数
    [SerializeField] int MAX_LAP_NUM = 3;                   //何周するか
    private List<bool> m_checkPoint = new List<bool>();     //通過したチェックポイントの保存配列


    void Start()
    {
        //チェックポイントを初期化
        for (int i = 0; i < MAX_CHECKPOINT_NUM; i++)
		{
            //指定したチェックポイント分配列を伸ばしていく
            m_checkPoint.Add(false);
		}

        //現在、ゲームシーンならば
        if(SceneManager.GetActiveScene().name[0..2] == "08")
		{
            //ラップ数を表示するオブジェクトを取得
            m_lapLabel = GameObject.Find("Lap");
            m_lapCountLabel = GameObject.Find("LapLabel");
            m_lapCountLabel.GetComponent<LapChange>().SetLapNum(m_lapCount);
            MAX_LAP_NUM = 3;
            m_lapLabel.SetActive(false);


            if(SceneManager.GetActiveScene().name == "08_EasyGameScene")
			{
                MAX_LAP_NUM = 1;
            }
        }
    }

    public int GetLapCount()
	{
        return m_lapCount;
	}

    //どの地点を通過したかを文字列で確認
    public void SetThroughPointName(string name)
	{
        //チェックポイントの数だけ見る
        for(int i = 0; i < MAX_CHECKPOINT_NUM; i++)
		{
            //チェックポイントと０～の数を組み合わせる
            string pointName = "CheckPoint" + i;
            //その文字列と通貨の連絡がきた文字列が同じならば
            if(name == pointName)
			{
                //通過したことを確認
                m_checkPoint[i] = true;
                //以降はあわないはずであるから、FOR文からでる。
                break;
			}
		}
	}

    //ゴールできるかチェックする
    public bool CheckCanGoal()
	{
        //ゴール済みならば、以下処理を行わない
        if(this.gameObject.GetComponent<AvatarController>().GetGoaled())
		{
            return false;
		}

        //全てのチェックポイントを
        foreach(var isThrough in m_checkPoint)
        {
            //通っていなければ
            if(!isThrough)
			{
                //ゴールできない
                return false;
			}
		}
        //完走したラップ数を増やす
        m_lapCount++;

        //m_lapLabel.GetComponent<LapChange>().SetLapNum(m_lapCount);
        m_lapLabel.SetActive(true);
        m_lapCountLabel.GetComponent<LapChange>().SetLapNum(m_lapCount);
        m_isDisplayingLapLabel = true;

        if (!PhotonNetwork.OfflineMode)
        {
            //次のウェイポイントの番号をルームプロパティに保存
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            //プレイヤー名＋LapCountという名前を作成 ex.)Player2LapCount
            string name = PhotonNetwork.NickName + "LapCount";
            //ウェイポイント番号を設定
            hashtable[name] = m_lapCount;
            //ルームプロパティの更新
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        }

        //フラグを元に戻す
        for (int i = 0; i < MAX_CHECKPOINT_NUM; i++)
		{
            m_checkPoint[i] = false;
		}

        //ゴールできたことを返す
        return true;
    }

    //レースを終えるか
    public bool IsFinishRacing()
	{
        //最大ラップ数を超えていたら
        if(m_lapCount >= MAX_LAP_NUM)
		{
            //終える
            return true;
		}
        else
		{
            //続ける
            return false;
		}
	}

    private void Update()
	{
        if(m_isDisplayingLapLabel)
		{
            m_displayTime += Time.deltaTime;
            if(m_displayTime >= 2.0f)
			{
                m_displayTime = 0.0f;
                m_isDisplayingLapLabel = false;

                //ゴール後周回ラベルを破棄するためヌルチェック
                if(m_lapLabel != null)
				{
                    m_lapLabel.SetActive(false);
                }
			}
		}
	}
}
