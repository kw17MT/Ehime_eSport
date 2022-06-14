using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapperController : MonoBehaviour
{
    private Vector3 m_targetPos = Vector3.zero;                 //次の目標地点
    private bool m_shouldCheckNextWayPoint = false;             //次のウェイポイントが更新すべきかどうか
    private float MOVE_POWER = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        //最初の目標地点
        m_targetPos = this.GetComponent<WayPointChecker>().GetNextWayPoint();
    }

    //次の目標地点を更新するように命令する
    public void SetCheckNextWayPoint()
	{
        m_shouldCheckNextWayPoint = true;
	}

    // Update is called once per frame
    void Update()
    {
        //次の目標地点を更新すべきであれば
        if(m_shouldCheckNextWayPoint)
		{
            //次の地点を更新
            m_targetPos = this.GetComponent<WayPointChecker>().GetNextWayPoint();
            m_shouldCheckNextWayPoint = false;
        }

        //目標地点に向かうベクトルを定義
        Vector3 moveDir = m_targetPos - this.transform.position;
        moveDir.Normalize();
        //リジッドボディに目標地点方向に力を加える
        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.AddForce((moveDir * MOVE_POWER) - rb.velocity);
    }
}
