using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム画面のカウントダウンの表示を更新するクラス
/// </summary>
public class CountDownSpriteChange : MonoBehaviour
{
    //カウントダウン画像
    [SerializeField] Sprite[] m_countDownSprite = null;

    public void ChangeCountDownSprite(int countDownNum)
    {
        //順位に応じて画像を変更
        this.GetComponent<Image>().sprite = m_countDownSprite[countDownNum];
    }
}