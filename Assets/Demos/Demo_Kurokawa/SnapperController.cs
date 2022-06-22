using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//鯛の基本行動クラス
public class SnapperController : MonoBehaviour
{
    private GameObject m_targetPlayer = null;                   //ターゲットしたプレイヤーのオブジェクト
    private Vector3 m_targetPos = Vector3.zero;                 //次の目標地点
    private float MOVE_POWER = 20.0f;                           //移動速度に乗算する倍率
    private bool m_isChasePlayer = false;                       //プレイヤーを見つけて追跡しているか
    private bool m_shouldCheckNextWayPoint = false;             //次のウェイポイントが更新すべきかどうか

    //次の目標地点を更新するように命令する
    public void SetCheckNextWayPoint()
	{
        m_shouldCheckNextWayPoint = true;
	}

    //タイと何かが衝突したら
	private void OnCollisionEnter(Collision col)
	{
        //それが自分ではなく、地面ではない場合
        if(col.gameObject.tag != "OwnPlayer" && col.gameObject.tag != "Ground")
		{
            //消す。（壁であっても消えるように）
            Destroy(this.gameObject, 0.1f);
        }
	}

    //鯛の感知エリアに何かが入ったら
	private void OnTriggerEnter(Collider col)
	{
        //それがプレイヤーであって、自分が別のプレイヤーを追跡していなけば
        if(col.gameObject.tag == "Player" && !m_isChasePlayer)
		{
            //追跡対象として保存
            m_targetPlayer = col.gameObject;
            //自分は追跡している
            m_isChasePlayer = true;
        }
	}

    // Update is called once per frame
    void Update()
    {
        //ターゲットとなるプレイヤーを見つけたならば
        if(m_isChasePlayer)
		{
            //ターゲットしたプレイヤーの座標を更新し続ける
            m_targetPos = m_targetPlayer.transform.position;
        }
        //何もターゲットしていなければ
		else
		{
            //次の目標地点を更新すべきであれば
            if (m_shouldCheckNextWayPoint)
            {
                //次の地点を更新
                m_targetPos = this.GetComponent<WayPointChecker>().GetNextWayPoint();
                //更新が完了した
                m_shouldCheckNextWayPoint = false;
            }
        }

        //目標地点に向かうベクトルを定義
        Vector3 moveDir = m_targetPos - this.transform.position;
        //正規化
        moveDir.Normalize();
        //リジッドボディに目標地点方向に力を加える
        Rigidbody rb = this.GetComponent<Rigidbody>();
        //規定した移動速度より早くならないように
        rb.AddForce((moveDir * MOVE_POWER) - rb.velocity);
    }
}
