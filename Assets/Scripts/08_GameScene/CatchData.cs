using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatchData : MonoBehaviour
{
    //ユーザーが設定した情報を格納して置く保管場所
    UserSettingData m_userSettingData = null;
    //ブラインダーの黒画像
    Image m_blinderPanelImage = null;

    void Start()
    {
        //ユーザー設定データのゲームオブジェクトを検索し、
        //ゲームコンポーネントを取得する
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //今まで保存されたデータをコンソールにデバック表示
        m_userSettingData.IndicateDebugLog();

        //ブラインドモードがONのとき、
        if(m_userSettingData.GetSetBlindMode)
        {
            //ブラインダーのゲームオブジェクトを取得
            m_blinderPanelImage = GameObject.Find("BlinderPanel").GetComponent<Image>();
            //ブラインダーを表示させる
            m_blinderPanelImage.enabled = true;
        }
    }
}
