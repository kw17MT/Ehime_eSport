using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressChecker : MonoBehaviour
{
    private bool[] m_checkPoint = new bool[3];

    // Start is called before the first frame update
    void Start()
    {
        //チェックポイントを初期化
        for (int i = 0; i < 3 ; i++)
		{
            m_checkPoint[i] = false;
		}
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

        return true;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
