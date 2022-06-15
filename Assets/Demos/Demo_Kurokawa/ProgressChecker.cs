using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ProgressChecker : MonoBehaviour
{
    public int MAX_CHECKPOINT_NUM = 3;                  //ステージに配置されるチェックポイントの数
    public int MAX_RAP_NUM = 1;                         //何周するか

    private List<bool> m_checkPoint = new List<bool>();
    private int m_rapCount = 0;
    private GameObject m_rapCountText = null;


    void Start()
    {
        //チェックポイントを初期化
        for (int i = 0; i < MAX_CHECKPOINT_NUM; i++)
		{
            m_checkPoint.Add(false);
		}

        //ラップカウントのテキストを追加
        m_rapCountText = GameObject.Find("RapCount");
        //現在のラップ数と最大ラップ数を表示
        m_rapCountText.GetComponent<Text>().text = "Rap : " + m_rapCount + " / " + MAX_RAP_NUM;
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

        //フラグを元に戻す
        for(int i = 0; i < MAX_CHECKPOINT_NUM; i++)
		{
            m_checkPoint[i] = false;
		}

        //ラップ数の更新
        m_rapCountText.GetComponent<Text>().text = "Rap : " + m_rapCount + " / " + MAX_RAP_NUM;

        //ゴールできる
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
