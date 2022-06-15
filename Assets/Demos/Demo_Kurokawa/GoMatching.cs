using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMatching : MonoBehaviour
{
    GameObject operation = null;

    //スタート関数
    void Start()
    {
        operation = GameObject.Find("OperationManager");
    }

    //更新関数
    void Update()
    {
        if (operation.GetComponent<OperationOld>().GetIsLongTouch())
        {
            //マッチングシーンへ
            SceneManager.LoadScene("DemoMatchingScene");
            Debug.Log("To Standby Scene");
        }

        if (operation.GetComponent<OperationOld>().GetDirection() == "right")
        {
            //インゲームへ直行（シングルプレイ）
            SceneManager.LoadScene("DemoInGame");
            GameObject pm = GameObject.Find("ParamManager");
            pm.GetComponent<ParamManage>().SetOfflineMode();
            Debug.Log("To Standby Scene");
        }
    }
}
