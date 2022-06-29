using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Photon.Pun;

//このスクリプトが割り当てられたインスタンスのチェックポイント管理インスタンス
public class ProgressChecker : MonoBehaviour
{
    private GameObject m_rapCountText = null;               //周回回数を提示するテキストインスタンス
    private int m_rapCount = 0;                             //ゲーム中の周回回数
    private List<bool> m_checkPoint = new List<bool>();     //通過したチェックポイントの保存配列

    public int MAX_CHECKPOINT_NUM = 3;                      //ステージに配置されるチェックポイントの数
    public int MAX_RAP_NUM = 3;                             //何周するか

    void Start()
    {
        //チェックポイントを初期化
        for (int i = 0; i < MAX_CHECKPOINT_NUM; i++)
		{
            //指定したチェックポイント分配列を伸ばしていく
            m_checkPoint.Add(false);
		}

        GameObject.Find("LapLabel").GetComponent<LapChange>().SetLapNum(m_rapCount);
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
        m_rapCount++;

        GameObject.Find("LapLabel").GetComponent<LapChange>().SetLapNum(m_rapCount);

        //次のウェイポイントの番号をルームプロパティに保存
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        //プレイヤー名＋WayPointNumberという名前を作成 ex.)Player2WayPointNumber
        string name = PhotonNetwork.NickName + "RapCount";
        //ウェイポイント番号を設定
        hashtable[name] = m_rapCount;
        //ルームプロパティの更新
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

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
        if(m_rapCount >= MAX_RAP_NUM)
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
}
