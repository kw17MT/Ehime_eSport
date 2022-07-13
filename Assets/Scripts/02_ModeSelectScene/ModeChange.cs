using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//モード選択画面クラス
public class ModeChange : MonoBehaviour
{
    //モード名
    [SerializeField] string[] m_modeName = null;
    //モードラベル
    [SerializeField] Text m_modeLabel = null;
    //選択移動をしているか
    bool m_selectMove = false;
    //移動時間カウンター
    int m_selectMoveCount = 0;
    //モードタイプ
    enum EnModeType
    {
        enOnlineMode,       //オンライン対戦モード
        enCpuMode,          //CPU対戦モード
        enTimeAttackMode,   //タイムアタックモード
        enSettingMode,      //設定モード
        enMaxModeNum        //モード数
    }
    //現在選択されているモード
    EnModeType m_nowSelectMode = EnModeType.enOnlineMode;
    //モード説明文
    [SerializeField] string[] m_modeExplanationSentence = null;
    //モード説明文ラベル
    [SerializeField] Text m_modeExplanationLabel = null;
    //操作システム
    Operation m_operation = null;

    CircleCenterRotateAround m_circleCenterRotateAround = null;

    //ユーザーが設定した情報を格納して置く保管場所
    UserSettingData m_userSettingData = null;

    void Start()
    {
        //操作システムのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();

        //円の中心を電車が回転する機能付きのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_circleCenterRotateAround = GameObject.Find("Train").GetComponent<CircleCenterRotateAround>();

        ///////////////////////////
        //BGMの再生
        nsSound.BGM.Instance.SetPlayBGM(nsSound.BGMNames.m_menu);
        ///////////////////////////
    }

    //アップデート関数
    void Update()
    {
        //選択しているモードを移動させる関数
        //操作は左右フリック
        ChangeSelectMode();

        //画面が長押しされたら、
        if (m_operation.GetIsLongTouch)
        {
            //次のシーンに遷移させる
            GoNextScene();
        }

        //電車の移動に合わせて選択しているデータを合わせるカウンター
        Count();

        //モードシーンのテキストなどのデータを更新
        ModeSceneDataUpdate();
    }

    //選択しているモードを移動させる関数
    void ChangeSelectMode()
    {
        if (m_selectMove)
        {
            return;
        }

        //画面が右フリックされたら、
        if (m_operation.GetNowOperation() == "right")
        {
            //次のモードに選択を移動
            GoNextMode();
        }
        //画面が左フリックされたら、
        if (m_operation.GetNowOperation() == "left")
        {
            //前のモードに選択を移動
            GoBackMode();
        }
    }

    //次のモードに選択を移動する関数
    void GoNextMode()
    {
        //選択移動状態にする
        m_selectMove = true;
        //選択されているモードを次のモードにする
        m_nowSelectMode++;
        if (m_nowSelectMode >= EnModeType.enMaxModeNum)
        {
            m_nowSelectMode = EnModeType.enOnlineMode;
        }
    }
    //前のモードに選択を移動する関数
    void GoBackMode()
    {
        //選択移動状態にする
        m_selectMove = true;
        //選択されているモードを前のモードにする
        m_nowSelectMode--;
        if (m_nowSelectMode < EnModeType.enOnlineMode)
        {
            m_nowSelectMode = EnModeType.enMaxModeNum-1;
        }
    }

    //モードシーンのテキストなどのデータを更新させる関数
    void ModeSceneDataUpdate()
    {
        //モード名を更新
        m_modeLabel.text = m_modeName[(int)m_nowSelectMode];
        //モード説明文を更新
        m_modeExplanationLabel.text = m_modeExplanationSentence[(int)m_nowSelectMode];
    }

    //次のシーンに遷移させる関数
    void GoNextScene()
    {
        //操作の判定を初期化させる
        m_operation.TachDataInit();

        //ユーザー設定データのゲームオブジェクトを検索し、
        //ゲームコンポーネントを取得する
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //選択されたモードデータを保存
        m_userSettingData.GetSetModeType = (int)m_nowSelectMode;

        //選択されているモードによって分岐
        switch (m_nowSelectMode)
        {
            //オンライン対戦モード
            case EnModeType.enOnlineMode:
                //設定モードシーンに遷移
                SceneManager.LoadScene("04_CharaSelectScene");
                break;
            //CPU対戦モード
            case EnModeType.enCpuMode:
                //設定モードシーンに遷移
                SceneManager.LoadScene("04_CharaSelectScene");
                break;
            //タイムアタックモード
            case EnModeType.enTimeAttackMode:
                //設定モードシーンに遷移
                SceneManager.LoadScene("04_CharaSelectScene");
                break;
            //設定モード
            case EnModeType.enSettingMode:
                //設定モードシーンに遷移
                SceneManager.LoadScene("03_SettingScene");
                break;
        }
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
}