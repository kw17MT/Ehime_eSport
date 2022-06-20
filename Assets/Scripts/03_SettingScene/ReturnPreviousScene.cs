using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///設定シーンから前のシーン(モード選択シーン)に戻るクラス
/// </summary>
public class ReturnPreviousScene : MonoBehaviour
{
    //操作システム
    Operation m_operation = null;
    //トグルの値
    ToggleOnOff m_blindToggle = null;
    //ユーザーが設定した情報を格納して置く保管場所
    UserSettingData m_userSettingData = null;

    void Start()
    {
        //操作システムのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
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

        //ブラインド切り替えのトグルのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_blindToggle = GameObject.Find("BlindToggle").GetComponent<ToggleOnOff>();

        //ユーザー設定データのゲームオブジェクトを検索し、
        //ゲームコンポーネントを取得する
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //ブラインドモードかどうかのデータを保存
        m_userSettingData.GetSetBlindMode = m_blindToggle.GetToggleValue;

        //モード選択シーンに遷移
        SceneManager.LoadScene("02_ModeSelectScene");
    }
}
