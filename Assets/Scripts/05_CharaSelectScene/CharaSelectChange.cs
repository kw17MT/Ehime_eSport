using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャラクター選択画面クラス
/// </summary>
public class CharaSelectChange : MonoBehaviour
{
    //キャラクター名
    string[] m_charaName = { "みきゃん", "子みきゃん", "ダークみきゃん" };
    //キャラクター名ラベル
    [SerializeField] Text m_charaNameLabel;

    //キャラクターステータス
    string[] m_charaStatus = { "S\nS\nS\nS", "A\nA\nA\nA", "B\nB\nB\nB" };
    //キャラステータスラベル
    [SerializeField] Text m_charaStatusLabel;

    //キャラクター説明文
    string[] m_charaExplanationSentence =
    {
        //みきゃん
        "えひめのマスコットキャラクター。みきゃん。かわいい。かわいい。かわいい。かわいい。",
        //子みきゃん
        "えひめのマスコットキャラクター。子みきゃん。かわいい。かわいい。かわいい。かわいい。",
        //ダークみきゃん
        "えひめのマスコットキャラクター。ダークみきゃん。かわいい。かわいい。かわいい。かわいい。"
    };
    //キャラクター説明ラベル
    [SerializeField] Text m_charaExplanationLabel;

    enum EnCharaType
    {
        enMikyan,        //みきゃん
        enKomikyan,      //子みきゃん
        enDarkmikyan,    //ダークみきゃん
        enMaxCharaNum    //最大キャラクター数
    }
    //現在選択されているキャラクター
    EnCharaType m_nowSelectChara = EnCharaType.enMikyan;

    //アップデート関数
    void Update()
    {
        //画面がタップされたら、
        if (Input.GetButtonDown("Fire1"))
        {
            //次のキャラクターに選択を移動
            GoNextChara();
        }

        //キャラ選択シーンのテキストなどのデータを更新
        CharaSelectSceneDataUpdate();
    }

    //次のキャラクターに選択を移動する関数
    void GoNextChara()
    {
        //選択されているキャラクターを次のキャラクターにする
        m_nowSelectChara++;
        if (m_nowSelectChara >= EnCharaType.enMaxCharaNum)
        {
            m_nowSelectChara = EnCharaType.enMikyan;
        }
    }

    //キャラ選択シーンのテキストなどのデータを更新させる関数
    void CharaSelectSceneDataUpdate()
    {
        //キャラクター名ラベルを更新
        m_charaNameLabel.text = m_charaName[(int)m_nowSelectChara];
        //キャラクターステータスラベルを更新
        m_charaStatusLabel.text = m_charaStatus[(int)m_nowSelectChara];
        //キャラクター説明ラベルを更新
        m_charaExplanationLabel.text = m_charaExplanationSentence[(int)m_nowSelectChara];
    }
}
