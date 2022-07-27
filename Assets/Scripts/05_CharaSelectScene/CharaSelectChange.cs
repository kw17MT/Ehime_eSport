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
    [SerializeField] string[] m_charaName = null;
    //キャラクター名ラベル
    [SerializeField] Text m_charaNameLabel = null;
    //キャラクターステータス
    [SerializeField] string[] m_charaStatus = null;
    //キャラステータスラベル
    [SerializeField] Text m_charaStatusLabel = null;
    //キャラクター説明文
    [SerializeField] string[] m_charaExplanationSentence = null;
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
    Operation m_operation = null;
    //選択移動をしているか
    bool m_selectMove = false;
    //移動時間カウンター
    int m_selectMoveCount = 0;

    CircleCenterRotateAround m_circleCenterRotateAround = null;

    //ユーザーが設定した情報を格納して置く保管場所
    UserSettingData m_userSettingData = null;

    void Start()
    {
        //操作システムのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
        //円の中心を電車が回転する機能付きのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_circleCenterRotateAround = GameObject.Find("Train").GetComponent<CircleCenterRotateAround>();
    }

    //アップデート関数
    void Update()
    {
        //選択するキャラクターを移動する
        ChangeSelectChara();

        //画面が長押しされたら、
        if (m_operation.GetIsDoubleTouch())
        {
            //次のシーンに遷移させる
            GoNextScene();
        }

        //電車の移動に合わせて選択しているデータを合わせるカウンター
        Count();

        //キャラ選択シーンのテキストなどのデータを更新
        CharaSelectSceneDataUpdate();
    }

    //選択するキャラクターを移動する関数
    void ChangeSelectChara()
    {
        if (m_selectMove)
        {
            return;
        }
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
    }

    //次のキャラクターに選択を移動する関数
    void GoNextChara()
    {
        //選択移動状態にする
        m_selectMove = true;
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
        //選択移動状態にする
        m_selectMove = true;
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
        m_charaStatusLabel.text =
            m_charaStatus[(int)m_nowSelectChara][0] + "\n" +
            m_charaStatus[(int)m_nowSelectChara][1] + "\n" +
            m_charaStatus[(int)m_nowSelectChara][2] + "\n" +
            m_charaStatus[(int)m_nowSelectChara][3];
        //キャラクター説明ラベルを更新
        m_charaExplanationLabel.text = m_charaExplanationSentence[(int)m_nowSelectChara];
    }

    //次のシーンに遷移させる関数
    void GoNextScene()
    {
        //操作の判定を初期化させる
        m_operation.TachDataInit();

        //ユーザー設定データのゲームオブジェクトを検索し、
        //ゲームコンポーネントを取得する
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //選択されたキャラクターデータを保存
        m_userSettingData.GetSetCharacter = (int)m_nowSelectChara;

        //ステージ選択シーンに遷移
        SceneManager.LoadScene("05_StageSelectScene");
    }

    //電車の移動に合わせて選択しているデータを合わせるカウンター
    void Count()
    {
        //選択移動状態じゃないときは処理をしない。
        if (!m_selectMove) return;

        //カウント計測
        m_selectMoveCount++;

        //カウントが指定した数値より大きくなったら、
        if (m_selectMoveCount > m_circleCenterRotateAround.GetCountTime)
        {
            //選択移動していない状態に戻す
            m_selectMove = false;
            //カウントの初期化
            m_selectMoveCount = 0;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //現在の選択している状態を取得
    public int GetNowSelectState()
    {
        return (int)m_nowSelectChara;
    }
    ///////////////////////////////////////////////////////////////////////////////
}