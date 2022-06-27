using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
///設定シーンのメインクラス
/// </summary>
public class SettingSceneManager : MonoBehaviour
{
    //操作システム
    Operation m_operation = null;

    //バイブレーショントグルの値
    ToggleOnOff m_vibrationToggle = null;

    //BGMのスライダー
    Slider m_bgmSlider = null;
    //SEのスライダー
    Slider m_seSlider = null;
    //キャラクターボイスのスライダー
    Slider m_charaVoiceSlider = null;
    //ナレーションボイスのスライダー
    Slider m_narrationVoiceSlider = null;

    //ブラインダートグルの値
    ToggleOnOff m_blindToggle = null;
    //ユーザーが設定した情報を格納して置く保管場所
    UserSettingData m_userSettingData = null;

    void Start()
    {
        //インスタンスの初期化
        InitInstance();
        //各値を初期化
        InitValue();
    }

    void Update()
    {
        //画面が長押しされたら、
        if (m_operation.GetIsLongTouch)
        {
            //前のシーンに戻る
            GoPreviousScene();
        }
    }

    //前のシーンに戻る関数
    void GoPreviousScene()
    {
        //操作の判定を初期化させる
        m_operation.TachDataInit();

        //各値を保管する
        StorageData();

        //モード選択シーンに遷移
        SceneManager.LoadScene("02_ModeSelectScene");
    }

    //インスタンスの初期化
    void InitInstance()
    {
        //操作システムのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();

        //ユーザー設定データのゲームオブジェクトを検索し、
        //ゲームコンポーネントを取得する
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();

        //BGMスライダーのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_bgmSlider = GameObject.Find("BgmVolumeSlider").GetComponent <Slider>();
        //SEスライダーのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_seSlider = GameObject.Find("SeVolumeSlider").GetComponent<Slider>();
        //キャラクターボイススライダーのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_charaVoiceSlider = GameObject.Find("CharaVoiceVolumeSlider").GetComponent<Slider>();
        //ナレーションボイススライダーのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_narrationVoiceSlider = GameObject.Find("NarrationVoiceVolumeSlider").GetComponent<Slider>();

        //バイブレーションのオンオフのトグルのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_vibrationToggle = GameObject.Find("VibrationToggle").GetComponent<ToggleOnOff>();
        //ブラインド切り替えのトグルのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_blindToggle = GameObject.Find("BlindToggle").GetComponent<ToggleOnOff>();
    }

    //各値の初期化関数
    void InitValue()
    {
        //BGMスライダーの値を初期化
        m_bgmSlider.value = m_userSettingData.GetSetBgmVolume;
        //SEスライダーの値を初期化
        m_seSlider.value = m_userSettingData.GetSetSeVolume;
        //キャラクターボイススライダーの値を初期化
        m_charaVoiceSlider.value = m_userSettingData.GetSetCharaVoiceVolume;
        //ナレーションボイススライダーの値を初期化
        m_narrationVoiceSlider.value = m_userSettingData.GetSetNarrationVoiceVolume;

        //ブラインドモードのトグルを初期化
        m_vibrationToggle.GetSetToggleValue = m_userSettingData.GetSetVibration;
        //ブラインドモードのトグルを初期化
        m_blindToggle.GetSetToggleValue = m_userSettingData.GetSetBlindMode;
    }

    //各値を保管する関数
    void StorageData()
    {
        //BGMの値を保存
        m_userSettingData.GetSetBgmVolume = (int)m_bgmSlider.value;
        //SEの値を保存
        m_userSettingData.GetSetSeVolume = (int)m_seSlider.value;
        //キャラボイスの値を保存
        m_userSettingData.GetSetCharaVoiceVolume = (int)m_charaVoiceSlider.value;
        //ナレーションボイスの値を保存
        m_userSettingData.GetSetNarrationVoiceVolume = (int)m_narrationVoiceSlider.value;
        //バイブレーションのオンオフのデータを保存
        m_userSettingData.GetSetVibration = m_vibrationToggle.GetSetToggleValue;
        //ブラインドモードかどうかのデータを保存
        m_userSettingData.GetSetBlindMode = m_blindToggle.GetSetToggleValue;
    }
}
