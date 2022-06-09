using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMatching : MonoBehaviour
{
    private GameObject operation = null;
    // Start is called before the first frame update
    void Start()
    {
        operation = GameObject.Find("OperationManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (operation.GetComponent<Operation>().GetIsLongTouch())
        {
            SceneManager.LoadScene("DemoMatchingScene");
            Debug.Log("To Standby Scene");
        }
    }
}
