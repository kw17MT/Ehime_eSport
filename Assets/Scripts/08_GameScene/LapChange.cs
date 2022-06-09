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
    [SerializeField]Text m_lapLabel;

    //ラップ数
    [SerializeField]int m_lapNo = 1;

    //アップデート関数
    void Update()
    {
        //現在のラップ数によってラップ数ラベルを変更
        m_lapLabel.text = m_lapNo + "/3";
    }
}
