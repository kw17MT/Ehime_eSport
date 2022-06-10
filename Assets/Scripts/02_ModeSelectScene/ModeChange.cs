using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//モード選択画面クラス
public class ModeChange : MonoBehaviour
{
    //モード名
    string[] m_modeName = { "オンラインたいせん","CPUたいせん","タイムアタック","せってい" };
    //モードラベル
    [SerializeField] Text m_modeLabel = null;

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
    string[] m_modeExplanationSentence =
    {
        //オンラインたいせんモード
        "オンラインたいせんモード。えへへでえへへなえへへである。えへへでえへへ。えへへへへへへへ。",
        //CUP対戦モード
        "CPUたいせんモード。えへへでえへへなえへへである。えへへでえへへ。えへへへへへへへ。",
        //タイムアタックモード
        "タイムアタックモード。えへへでえへへなえへへである。えへへでえへへ。えへへへへへへへ。",
        //せっていモード
        "せっていモード。えへへでえへへなえへへである。えへへでえへへ。えへへへへへへへ。"
    };
    //モード説明文ラベル
    [SerializeField] Text m_modeExplanationLabel = null;

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
            //次のモードに選択を移動
            GoNextMode();
        }
        //画面が左フリックされたら、
        if (m_operation.GetNowOperation() == "left")
        {
            //前のモードに選択を移動
            GoBackMode();
        }

        //画面が長押しされたら、
        if (m_operation.GetIsLongTouch())
        {
            //次のシーンに遷移させる
            GoNextScene();
        }

        //モードシーンのテキストなどのデータを更新
        ModeSceneDataUpdate();
    }

    //次のモードに選択を移動する関数
    void GoNextMode()
    {
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
}