using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム画面の現在の順位の表示を更新するクラス
/// </summary>
public class NowRankingChange : MonoBehaviour
{
    //現在の順位ラベル
    [SerializeField] Text m_nowRankingLabel = null;
    //順位のランクラベル
    [SerializeField] Text m_rankLabel = null;
    //現在の順位
    [SerializeField] int m_ranking = 1;
    //ランク(fir"st",seco"nd",thi"rd",for"th")
    string[] m_rankStr = { "st","nd","rd","th"};
    //順位ごとの色
    Vector4[] m_rankColor =
    {
        new Vector4(1.0f,1.0f,0.0f,1.0f),           //金色
        new Vector4(0.745f,0.745f,0.745f,1.0f),     //銀色
        new Vector4(0.588f,0.274f,0.196f,1.0f),     //銅色
        new Vector4(0.0f,0.0f,0.0f,1.0f)            //黒色
    };

    //アップデート関数
    void Update()
    {
        //現在の順位の値や色などのデータを更新
        RankingDataUpdate();
    }

    //現在の順位の値や色などのデータを更新させる関数
    void RankingDataUpdate()
    {
        //現在の順位によって数値を変化
        m_nowRankingLabel.text = m_ranking + "";
        m_rankLabel.text = m_rankStr[m_ranking - 1];
        //現在の順位によって色を変化
        m_nowRankingLabel.color = m_rankColor[m_ranking - 1];
        m_rankLabel.color = m_rankColor[m_ranking - 1];
    }
}
