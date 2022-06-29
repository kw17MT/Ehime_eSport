using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム画面の現在のラップ数の表示を更新するクラス
/// </summary>
public class LapChange : MonoBehaviour
{
    //ラップ数ラベル
    [SerializeField]Text m_lapLabel = null;
    //ラップ数
    [SerializeField]int m_lapNo = 1;

    public void SetLapNum(int num)
	{
        m_lapNo = num;
	}

    public int GetLapNum()
	{
        return m_lapNo;
	}

    //アップデート関数
    void Update()
    {
        //現在のラップ数によってラップ数ラベルを変更
        m_lapLabel.text = m_lapNo + "/3";
    }
}
