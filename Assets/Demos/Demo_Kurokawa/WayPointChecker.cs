using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ウェイポイント経過処理と次のウェイポイントを提示するクラス
public class WayPointChecker : MonoBehaviour
{
    private Vector3 m_currentWayPointPos = Vector3.zero;        //現在通過済みのウェイポイントの座標
    private Vector3 m_nextWayPointPos = Vector3.zero;           //次のウェイポイントの座標
    private int m_nextWayPointNumber = 0;                       //次のウェイポイントの番号

    // Start is called before the first frame update
    void Start()
    {
        //このクラスを持っているのが自分のプレイヤーだったら
        if(this.gameObject.tag == "OwnPlayer")
		{
            //カメラ回転抑制の都合上最初はウェイポイント1目指す。
            GameObject nextWayPoint = GameObject.Find("WayPoint1");
            //次のウェイポイントの座標を取得
            m_nextWayPointPos = nextWayPoint.transform.position;
        }
    }

    //他のクラス（主にプレイヤー）から直接現在直近で通過したウェイポイントの座標と番号を設定
    public void SetCurrentWayPointDirectly(Vector3 pos, int wayPointNumber)
	{
        //下関数で次のウェイポイントを更新するため1つ前のポイントで初期化
        m_nextWayPointNumber = wayPointNumber;
        //次の目的地を更新する。
        SetNextWayPoint(pos, wayPointNumber);
    }

    //次のウェイポイントに向かうため、変数を更新する
    public void SetNextWayPoint(Vector3 currentPos, int throughNumber)
    {
        //既に通過済みのポイントと再度接触して不要な更新が行われないようにする。
        if (m_nextWayPointNumber != throughNumber)
        {
            return;
        }

        //通過済みポイントの座標を保存
        m_currentWayPointPos = currentPos;
        //次のポイントへインクリメント
        m_nextWayPointNumber++;
        //次のポイントの名前を定義
        string nextWayPointName = "WayPoint" + m_nextWayPointNumber;
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
            //自分の次のウェイポイントはどこか更新させるように伝える
            this.GetComponent<SnapperController>().SetCheckNextWayPoint();
        }
    }

    //次のウェイポイントの座標を返す
    public Vector3 GetNextWayPoint()
	{
        return m_nextWayPointPos;
	}

    //直近で通過したウェイポイントの座標を返す
    public Vector3 GetCurrentWayPoint()
    {
        return m_currentWayPointPos;
    }

    //直近で通過したウェイポイント番号を返す
    public int GetCurrentWayPointNumber()
    {
        return m_nextWayPointNumber;
    }
}
