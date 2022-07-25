using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
///設定シーンから前のシーン(モード選択シーン)に戻るクラス
/// </summary>
public class ReturnPreviousScene : MonoBehaviourPunCallbacks
{
    //操作システム
    Operation m_operation = null;
    //前のシーン名
    [SerializeField] string m_previousSceneName = null;
    //設定シーンかどうか
    [SerializeField] bool m_isSettingScene = false;
    //マッチングシーンかどうか
    [SerializeField] bool m_isMatchingScene = false;

    void Start()
    {
        //操作システムのゲームオブジェクトを検索しゲームコンポーネントを取得する
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
    }

    //前のシーンに戻る関数
    public void GoPreviousScene()
    {
        //操作の判定を初期化させる
        m_operation.TachDataInit();

        //設定シーンの時のみ実行
        if (m_isSettingScene)
        {
            //ブラインドモードかどうかのデータを保存
            GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>().GetSetBlindMode = GameObject.Find("BlindToggle").GetComponent<ToggleOnOff>().GetToggleValue;
        }
        else if(m_isMatchingScene)
		{
            //ルームから出る
            PhotonNetwork.LeaveRoom();
            //サーバーから出る
            PhotonNetwork.Disconnect();
        }

        if(SceneManager.GetActiveScene().name == "02_ModeSelectScene")
		{
            Destroy(GameObject.Find("ParamManager"));
		}

        //モード選択シーンに遷移
        SceneManager.LoadScene(m_previousSceneName);

        //デバック
        Debug.Log(m_previousSceneName + "シーンに戻ります");
    }
}
