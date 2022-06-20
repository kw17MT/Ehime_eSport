using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchData : MonoBehaviour
{
    //ユーザーが設定した情報を格納して置く保管場所
    UserSettingData m_userSettingData = null;
    //ブラインダーゲームオブジェクト
    GameObject m_blinderPanel = null;

    void Start()
    {
        //ユーザー設定データのゲームオブジェクトを検索し、
        //ゲームコンポーネントを取得する
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //今まで保存されたデータをコンソールにデバック表示
        m_userSettingData.IndicateDebugLog();

        //ブラインドモードがOFFのとき、
        if(!m_userSettingData.GetSetBlindMode)
        {
            //ブラインダーのゲームオブジェクトを取得
            m_blinderPanel = GameObject.Find("BlinderPanel");
            //真っ暗だった画面を元に戻す
            m_blinderPanel.SetActive(false);
        }
    }
}
