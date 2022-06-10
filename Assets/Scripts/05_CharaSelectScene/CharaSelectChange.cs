using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// キャラクター選択画面クラス
/// </summary>
public class CharaSelectChange : MonoBehaviour
{
    //キャラクター名
    string[] m_charaName = { "みきゃん", "子みきゃん", "ダークみきゃん" };
    //キャラクター名ラベル
    [SerializeField] Text m_charaNameLabel = null;

    //キャラクターステータス
    string[] m_charaStatus = { "S\nS\nS\nS", "A\nA\nA\nA", "B\nB\nB\nB" };
    //キャラステータスラベル
    [SerializeField] Text m_charaStatusLabel = null;

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
    [SerializeField] Text m_charaExplanationLabel = null;

    enum EnCharaType
    {
        enMikyan,        //みきゃん
        enKomikyan,      //子みきゃん
        enDarkmikyan,    //ダークみきゃん
        enMaxCharaNum    //最大キャラクター数
    }
    //現在選択されているキャラクター
    EnCharaType m_nowSelectChara = EnCharaType.enMikyan;

    //操作システム
    OperationNew m_operation = null;

    void Start()
    {
        //操作システムのゲームオブジェクトを検索しスクリプトを使用する
        m_operation = GameObject.Find("OperationSystem").GetComponent<OperationNew>();
    }

    //アップデート関数
    void Update()
    {
        //画面が右フリックされたら、
        if (m_operation.GetNowOperation() == "right")
        {
            //次のキャラクターに選択を移動
            GoNextChara();
        }
        //画面が左フリックされたら、
        if (m_operation.GetNowOperation() == "left")
        {
            //前のキャラクターに選択を移動
            GoBackChara();
        }

        //画面が長押しされたら、
        if (m_operation.GetIsLongTouch())
        {
            //次のシーンに遷移させる
            GoNextScene();
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
    //前のキャラクターに選択を移動する関数
    void GoBackChara()
    {
        //選択されているモードを前のモードにする
        m_nowSelectChara--;
        if (m_nowSelectChara < EnCharaType.enMikyan)
        {
            m_nowSelectChara = EnCharaType.enMaxCharaNum - 1;
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

    //次のシーンに遷移させる関数
    void GoNextScene()
    {
        //操作の判定を初期化させる
        m_operation.TachDataInit();

        //ステージ選択シーンに遷移
        SceneManager.LoadScene("05_StageSelectScene");
    }
}