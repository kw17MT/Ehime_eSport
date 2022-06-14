using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointChecker : MonoBehaviour
{
    private Vector3 m_currentWayPointPos = Vector3.zero;
    private Vector3 m_nextWayPointPos = Vector3.zero;
    private int m_currentWayPointNum = 0;


    // Start is called before the first frame update
    void Start()
    {
        GameObject nextWayPoint = GameObject.Find("WayPoint0");
        m_nextWayPointPos = nextWayPoint.transform.position;
    }

    public void SetNextWayPoint(Vector3 currentPos, int throughNumber)
	{
        if(m_currentWayPointNum != throughNumber)
		{
            return;
		}

        m_currentWayPointPos = currentPos;

        m_currentWayPointNum++;
        string nextWayPointName = "WayPoint" + m_currentWayPointNum;

        Debug.Log(nextWayPointName);

        GameObject nextWayPoint = GameObject.Find(nextWayPointName);
        if (nextWayPoint == null)
        {
            nextWayPoint = GameObject.Find("WayPoint0");
            m_currentWayPointNum = 0;

            Debug.Log(nextWayPoint.name);
        }

        m_nextWayPointPos = nextWayPoint.transform.position;

        this.GetComponent<SnapperController>().SetCheckNextWayPoint();
    }

    public Vector3 GetNextWayPoint()
	{
        return m_nextWayPointPos;
	}

    public Vector3 GetCurrentWayPoint()
    {
        return m_currentWayPointPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
