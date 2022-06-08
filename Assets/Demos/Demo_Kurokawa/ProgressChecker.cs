using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressChecker : MonoBehaviour
{
    private bool[] m_checkPoint = new bool[3];
    private int m_goalCount = 0;
    private GameObject m_rapCountText = null;

    // Start is called before the first frame update
    void Start()
    {
        //チェックポイントを初期化
        for (int i = 0; i < 3 ; i++)
		{
            m_checkPoint[i] = false;
		}

        m_rapCountText = GameObject.Find("RapCount");
        m_rapCountText.GetComponent<Text>().text = "Rap : " + m_goalCount + " / 3";
    }

    public void SetThroughPointName(string name)
	{
        switch(name)
		{
            case "CheckPoint0":
                m_checkPoint[0] = true;
                break;
            case "CheckPoint1":
                m_checkPoint[1] = true;
                break;
            case "CheckPoint2":
                m_checkPoint[2] = true;
                break;
            default:
                break;
        }
	}

    public bool CheckCanGoal()
	{
        foreach(var isThrough in m_checkPoint)
        {
            if(!isThrough)
			{
                return false;
			}
		}
        //ラップ数を増やす
        m_goalCount++;

        m_rapCountText.GetComponent<Text>().text = "Rap : " + m_goalCount + " / 3";

        //ラップ数を返す
        return true;
    }

    public bool IsFinishRacing()
	{
        if(m_goalCount >= 1)
		{
            return true;
		}
        else
		{
            return false;
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
