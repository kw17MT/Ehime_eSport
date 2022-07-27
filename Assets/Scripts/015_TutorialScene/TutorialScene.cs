using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScene : MonoBehaviour
{
    //操作システム
    Operation m_operation = null;

    // Start is called before the first frame update
    void Start()
    {
        //操作システムのゲームオブジェクトを検索しコンポーネントを取得
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
    }

    // Update is called once per frame
    void Update()
    {
        //画面がタップされたら、
        if (m_operation.GetNowOperation() == "touch")
        {
            //モードシーンに遷移
            SceneManager.LoadScene("02_ModeSelectScene");
        }
    }
}