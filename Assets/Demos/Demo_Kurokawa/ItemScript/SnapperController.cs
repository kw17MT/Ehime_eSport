using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//鯛の基本行動クラス
public class SnapperController : MonoBehaviourPunCallbacks
{
    private GameObject m_targetPlayer = null;                   //ターゲットしたプレイヤーのオブジェクト
    private bool m_isChasePlayer = false;                       //プレイヤーを見つけて追跡しているか
    private bool m_shouldCheckNextWayPoint = false;             //次のウェイポイントが更新すべきかどうか
    private bool m_isAddFirstVelocity = false;                  //初めてポップされた時に強い力で押して発射したプレイヤーとの接触を回避したか
    private int m_ownerID = 0;                                  //発射したプレイヤーの番号
    private float MOVE_POWER = 25.0f;                           //移動速度に乗算する倍率
    private Vector3 m_targetPos = Vector3.zero;                 //次の目標地点
    private Vector3 m_moveDir = Vector3.zero;                   //移動方向

    public void SetOwnerID(int id)
    {
        m_ownerID = id;
    }

    public int GetOwnerID()
    {
        return m_ownerID;
    }

    //次の目標地点を更新するように命令する
    public void SetCheckNextWayPoint()
	{
        m_shouldCheckNextWayPoint = true;
	}

    [PunRPC]
    private void DestroyItemWithName(string name)
    {
        GameObject.Find("SceneDirector").GetComponent<ItemStateCommunicator>().DestroyItemWithName(name);
    }

    //タイと何かが衝突したら
    private void OnCollisionEnter(Collision col)
	{
        //それが自分ではなく、地面ではない場合
        if (col.gameObject.tag != "OwnPlayer" && col.gameObject.tag != "Ground")
		{
            //消す。（壁であっても消えるように）
            photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, this.gameObject.name);
        }
    }

    //鯛の感知エリアに何かが入ったら
    private void OnTriggerEnter(Collider col)
	{
        //それがプレイヤーであって、自分が別のプレイヤーを追跡していなけば
        if ((col.gameObject.tag == "Player" || col.gameObject.tag == "OwnPlayer") && !m_isChasePlayer)
		{
            //そのオブジェクトはAIのみが所持するスクリプトを持っているか
            AICommunicator aiCommunicator = col.gameObject.GetComponent<AICommunicator>();
            int playerID = 0;
            //持っていなければ
            if (aiCommunicator == null)
			{
                //プレイヤーのもつオンライン上でのニックネームを取得
                Player pl = col.gameObject.GetComponent<PhotonView>().Owner;
                //取得できた＝プレイヤーならば
                if (pl != null)
                {
                    string strID = pl.NickName;
                    playerID = int.Parse(strID[6].ToString());
                }
            }
            //接触オブジェクトがAIならば
			else
			{
                string strID = aiCommunicator.GetAIName();
                playerID = int.Parse(strID[6].ToString());
			}

            //発射したプレイヤーが自分でなく、感知したプレイヤーが自分の進行方向前方にいたら
            if (m_ownerID != playerID && Vector3.Dot(m_moveDir, col.gameObject.transform.position - this.gameObject.transform.position) >= 1.0f)
            {
                //追跡対象として保存
                m_targetPlayer = col.gameObject;
                //自分は追跡している
                m_isChasePlayer = true;
            }
        }
	}

    // Update is called once per frame
    void FixedUpdate()
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
        m_moveDir = m_targetPos - this.transform.position;
        //正規化
        m_moveDir.Normalize();
        //リジッドボディに目標地点方向に力を加える
        Rigidbody rb = this.GetComponent<Rigidbody>();
        //規定した移動速度より早くならないように

        if(!m_isAddFirstVelocity)
        {
            rb.velocity = m_moveDir * MOVE_POWER;
            m_isAddFirstVelocity = true;
        }
        rb.AddForce((m_moveDir * MOVE_POWER) - rb.velocity);
    }
}
