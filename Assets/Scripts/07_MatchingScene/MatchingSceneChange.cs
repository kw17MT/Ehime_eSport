using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// マッチング完了した後のシーン遷移クラス
/// </summary>
public class MatchingSceneChange : MonoBehaviour
{
    //操作システム
    Operation m_operation = null;

    void Start()
    {
        //操作システムのゲームオブジェクトを検索しスクリプトを使用する
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
    }

    //アップデート関数
    void Update()
    {
        //画面が長押しされたら、
        if (m_operation.GetIsLongTouch())
        {
            //次のシーンに遷移させる
            GoNextScene();
        }
    }

    //次のシーンに遷移させる関数
    void GoNextScene()
    {
        //操作の判定を初期化させる
        m_operation.TachDataInit();

        //ゲームシーンに遷移
        SceneManager.LoadScene("08_GameScene");
    }
}