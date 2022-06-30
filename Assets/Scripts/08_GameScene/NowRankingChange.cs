using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム画面の現在の順位の表示を更新するクラス
/// </summary>
public class NowRankingChange : MonoBehaviour
{
    //順位画像
    [SerializeField] Sprite[] m_rankingSprite = null;

    public void ChangeRanking(int rank)
	{
        //順位に応じて画像を変更
        this.GetComponent<Image>().sprite = m_rankingSprite[rank];
    }
}