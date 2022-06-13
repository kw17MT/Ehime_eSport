using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 設定画面のスライダーの現在の数値をテキスト表示するクラス
/// </summary>
public class GetSliderValue : MonoBehaviour
{
    [SerializeField]Slider m_slider = null;
    [SerializeField] Text m_nowSliderValueText = null;

    //スライダーの値が変更されたら実行される関数
    public void ChangeValue()
    {
        //ハンドルの上の数値を更新
        m_nowSliderValueText.text = "" + (int)(100 - m_slider.value);
    }
}