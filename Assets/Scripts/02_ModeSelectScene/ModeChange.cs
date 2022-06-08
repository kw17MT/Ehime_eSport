using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//モード選択画面クラス
public class ModeChange : MonoBehaviour
{
    //モード名
    string[] m_modeName = { "オンラインたいせん","CPUたいせん","タイムアタック","せってい" };
    //モードラベル
    [SerializeField] Text m_modeLabel;

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
    [SerializeField] Text m_modeExplanationLabel;

    //アップデート関数
    void Update()
    {
        //画面がタップされたら、
        if (Input.GetButtonDown("Fire1"))
        {
            //次のモードに選択を移動
            GoNextMode();
        }

        //次のシーンに遷移させる
        GoNextScene();

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
        //選択されているモードによって分岐
        switch (m_nowSelectMode)
        {
            //オンライン対戦モード
            case EnModeType.enOnlineMode:
                break;
            //CPU対戦モード
            case EnModeType.enCpuMode:
                break;
            //タイムアタックモード
            case EnModeType.enTimeAttackMode:
                break;
            //設定モード
            case EnModeType.enSettingMode:
                break;
        }
    }
}