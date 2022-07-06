using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム画面の現在の順位の表示を更新するクラス
/// </summary>
public class NowRankingChange : MonoBehaviour
{
    [SerializeField] Sprite[] m_rankingImage = { null };

    public void ChangeRanking(int rank)
	{
        this.GetComponent<Image>().sprite = m_rankingImage[rank];
	}
}
