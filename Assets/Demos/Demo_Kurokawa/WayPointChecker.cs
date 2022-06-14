using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointChecker : MonoBehaviour
{
    private Vector3 m_currentWayPointPos = Vector3.zero;
    private Vector3 m_nextWayPointPos = Vector3.zero;
    private int m_nextWayPointNumber = 0;


    // Start is called before the first frame update
    void Start()
    {
        if(this.gameObject.tag == "OwnPlayer")
		{
            GameObject nextWayPoint = GameObject.Find("WayPoint1");
            m_nextWayPointPos = nextWayPoint.transform.position;
        }
    }

    public void SetCurrentWayPointDirectly(Vector3 pos, int wayPointNumber)
	{
        Debug.Log("セットする通過済みナンバー　:　" + wayPointNumber);
        m_nextWayPointNumber = wayPointNumber;
        //次の目的地を更新する。
        SetNextWayPoint(pos, wayPointNumber);
    }

    public void SetNextWayPoint(Vector3 currentPos, int throughNumber)
    {
        //既に通過済みのポイントと再度接触して不要な更新が行われないようにする。
        if (m_nextWayPointNumber != throughNumber)
        {
            Debug.Log(this.gameObject.name + " Cant Update Next　次のポイント→" + m_nextWayPointNumber + " : 通過したポイント→" + throughNumber);
            return;
        }

        //通過済みポイントの座標を保存
        m_currentWayPointPos = currentPos;
        //次のポイントへインクリメント
        m_nextWayPointNumber++;
        //次のポイントの名前を定義
        string nextWayPointName = "WayPoint" + m_nextWayPointNumber;

        Debug.Log(this.gameObject.name + "の次のウェイポイントは　:  " + nextWayPointName);
        //次のポイントインスタンスを取得
        GameObject nextWayPoint = GameObject.Find(nextWayPointName);
        //なければ、0番に戻す
        if (nextWayPoint == null)
        {
            nextWayPoint = GameObject.Find("WayPoint0");
            m_nextWayPointNumber = 0;
        }
        //新しいポイントの座標を取得
        m_nextWayPointPos = nextWayPoint.transform.position;

        //このインスタンスの持ち主が鯛であれば
        if (gameObject.name == "Snapper")
        { 
            this.GetComponent<SnapperController>().SetCheckNextWayPoint();
        }
    }

    public Vector3 GetNextWayPoint()
	{
        return m_nextWayPointPos;
	}

    public Vector3 GetCurrentWayPoint()
    {
        return m_currentWayPointPos;
    }

    public int GetCurrentWayPointNumber()
    {
        Debug.Log(this.gameObject.name + "CurrentNumber = " + m_nextWayPointNumber);
        return m_nextWayPointNumber;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
