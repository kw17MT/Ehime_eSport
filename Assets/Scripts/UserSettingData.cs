using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユーザーが選んだアウトゲームのデータ(モード、設定、ステージ、キャラクター、CPUの強さ等)
/// を保存しインゲームに送るためのクラス
/// </summary>
public class UserSettingData : MonoBehaviour
{
    //モード
    int m_modeType = 0;
    //BGMボリューム
    int m_bgmBolume = 0;
    //SEボリューム
    int m_seBolume = 0;
    //キャラクターボイスボリューム
    int m_charaVoiceBolume = 0;
    //ナレーションボイスボリューム
    int m_narrationVoiceBolume = 0;
    //バイブレーション機能のオンオフ
    bool m_isVibration = true;
    //ブラインドモードかどうか
    bool m_isBlindMode = false;
    //ステージ
    int m_stageType = 0;
    //キャラクター
    int m_character = 0;
    //CPUの強さ
    int m_cpuPower = 0;

    void Start()
    {
        //シーン遷移してもこの操作オブジェクトは削除しない
        DontDestroyOnLoad(this);
    }

    //モードタイプのプロパティ
    public int GetSetModeType
    {
        //アクセッサ

        //ゲッター
        get
        {
            return m_modeType;
        }
        //セッター
        set
        {
            m_modeType = value;
        }
    }

    //BGMボリュームのプロパティ
    public int GetSetBgmVolume
    {
        //アクセッサ

        //ゲッター
        get
        {
            return m_bgmBolume;
        }
        //セッター
        set
        {
            m_bgmBolume = value;
        }
    }

    //SEボリュームのプロパティ
    public int GetSetSeVolume
    {
        //アクセッサ

        //ゲッター
        get
        {
            return m_seBolume;
        }
        //セッター
        set
        {
            m_seBolume = value;
        }
    }

    //キャラクターボイスボリュームのプロパティ
    public int GetSetCharaVoiceVolume
    {
        //アクセッサ

        //ゲッター
        get
        {
            return m_charaVoiceBolume;
        }
        //セッター
        set
        {
            m_charaVoiceBolume = value;
        }
    }

    //ナレーションボイスボリュームのプロパティ
    public int GetSetNarrationVoiceVolume
    {
        //アクセッサ

        //ゲッター
        get
        {
            return m_narrationVoiceBolume;
        }
        //セッター
        set
        {
            m_narrationVoiceBolume = value;
        }
    }

    //バイブレーション機能のオンオフのプロパティ
    public bool GetSetVibration
    {
        //アクセッサ

        //ゲッター
        get
        {
            return m_isVibration;
        }
        //セッター
        set
        {
            m_isVibration = value;
        }
    }

    //ブラインドモードのプロパティ
    public bool GetSetBlindMode
    {
        //アクセッサ

        //ゲッター
        get
        {
            return m_isBlindMode;
        }
        //セッター
        set
        {
            m_isBlindMode = value;
        }
    }

    //ステージのプロパティ
    public int GetSetStageType
    {
        //アクセッサ

        //ゲッター
        get
        {
            return m_stageType;
        }
        //セッター
        set
        {
            m_stageType = value;
        }
    }

    //キャラクターのプロパティ
    public int GetSetCharacter
    {
        //アクセッサ

        //ゲッター
        get
        {
            return m_character;
        }
        //セッター
        set
        {
            m_character = value;
        }
    }

    //CPUの強さのプロパティ
    public int GetSetCpuPower
    {
        //アクセッサ

        //ゲッター
        get
        {
            return m_cpuPower;
        }
        //セッター
        set
        {
            m_cpuPower = value;
        }
    }

    //設定されたデータを確認するために
    //コンソールにデバック表示する関数
    public void IndicateDebugLog()
    {
        Debug.Log("モードタイプ：" + m_modeType);
        Debug.Log("ブラインドモードかどうか：" + m_isBlindMode);
        Debug.Log("ステージ：" + m_stageType);
        Debug.Log("キャラクター：" + m_character);
        Debug.Log("CPUの強さ：" + m_cpuPower);
    }
}