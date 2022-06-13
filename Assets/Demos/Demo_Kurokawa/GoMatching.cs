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
            SceneManager.LoadScene("DemoMatchingScene");
            Debug.Log("To Standby Scene");
        }
    }
}
