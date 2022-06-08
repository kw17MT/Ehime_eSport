using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamManage : MonoBehaviour
{
    private int m_orangePeelNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddOrangePeelNum()
	{
        m_orangePeelNum++;
	}

    public int GetOrangePeelNumOnField()
	{
        return m_orangePeelNum;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
