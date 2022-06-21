using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//デモシーンのオンラインかオフラインかで分岐するシーン
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
        //長押しならば
        if (operation.GetComponent<OperationOld>().GetIsLongTouch())
        {
            //マッチングシーンへ
            SceneManager.LoadScene("DemoMatchingScene");
        }

        //右フリックならば
        if (operation.GetComponent<OperationOld>().GetDirection() == "right")
        {
            //インゲームへ直行（シングルプレイ）
            SceneManager.LoadScene("DemoInGame");
            //シングルプレイモードに設定する
            GameObject pm = GameObject.Find("ParamManager");
            pm.GetComponent<ParamManage>().SetOfflineMode();
        }
    }
}
