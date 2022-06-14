using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacksを継承して、photonViewプロパティを使えるようにする
public class AvatarController : MonoBehaviourPunCallbacks
{
    Rigidbody m_rb = null;                              //割り当てられたリジッドボディ
    Vector3 m_moveDir = Vector3.zero;                   //移動する方向
    Vector3 m_moveSpeed = Vector3.zero;                 //移動スピード
    Vector3 m_rot = Vector3.zero;                       //どちらに回転するかの向き
    Vector3 m_corseDir = Vector3.zero;
    private GameObject m_paramManager = null;           //パラメータを保存するインスタンス（シーン跨ぎ）
    private bool m_canMove = false;                     //移動が制限されていないか
    private float m_runningTime = 0.0f;                 //走行時間
    private float m_stiffenTime = 0.0f;
    private float m_starTime = 0.0f;
    private float m_dashTime = 0.0f;
    private bool m_isGoaled = false;                    //自分はゴールしたか
    private bool m_isToldRecord = false;                //自分の走破レコードをホストクライアントに送ったかどうかのフラグ
    private bool m_isToldReady = false;                 //ルームに参加して準備ができたことを一度だけ通信するためのフラグ
    private bool m_isUsingStar = false;
    private bool m_isUsingKiller = false;
    private bool m_isUsingJet = false;
    private bool m_isAttacked = false;
    private Quaternion m_prevTrasnform;
    

    public float MOVE_POWER = 25.0f;                   //リジッドボディにかける移動の倍率
    public float MOVE_POWER_USING_STAR = 35.0f;        //スター使用時のリジッドボディにかける移動の倍率
    public float MOVE_POWER_USING_JET = 50.0f;        //ジェット使用時のリジッドボディにかける移動の倍率
    public float MOVE_POWER_USING_KILLER = 40.0f;      //キラー使用時のリジッドボディにかける移動の倍率
    public float ROT_POWER = 1.0f;                      //ハンドリング
    public float MAX_STAR_REMAIN_TIME = 10.5f;           //スターの継続時間
    public float MAX_STIFFIN_TIME = 1.5f;               //攻撃が当たった時の最大硬直時間
    public float MAX_DASH_TIME = 1.0f;

    void Start()
    {
        //リジッドボディを取得
        m_rb = GetComponent<Rigidbody>();
        //インゲーム中であれば
        if (SceneManager.GetActiveScene().name == "DemoInGame")
        {
            //重力をオンにする
            m_rb.useGravity = true;
            //インゲームに移行できたことを通信
            photonView.RPC(nameof(TellReadyOK), RpcTarget.MasterClient);
        }
        //ゲーム中のパラメータ保存インスタンスを取得
        m_paramManager = GameObject.Find("ParamManager");
        //ネットワークで同期される名前を設定
        PhotonNetwork.NickName = "Player" + m_paramManager.GetComponent<ParamManage>().GetPlayerID();
        gameObject.tag = "Player";
        //自分が生成されたインスタンスであれば
        if (photonView.IsMine)
        {
            //探しやすい名前を付ける。（ヒエラルキーにも適用される）
            gameObject.name = "OwnPlayer";
            //タグをつける
            gameObject.tag = "OwnPlayer";
        }

        m_prevTrasnform = this.transform.rotation;

        //1秒間に何回通信するか
        PhotonNetwork.SendRate = 3;
        //1秒間に何回同期を行うか
        PhotonNetwork.SerializationRate = 3;
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //更新されたルームのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in propertiesThatChanged)
        {
            string name = PhotonNetwork.NickName + "Invincible";
            string key = prop.Key.ToString();
            if (name == key)
			{
                bool isUsingStar = false;
                int isUsing = (PhotonNetwork.CurrentRoom.CustomProperties[prop.Key] is int value) ? value : 0;
                if (isUsing == 1)
				{
                    isUsingStar = true;
				}
                m_isUsingStar = isUsingStar;
            }
            Debug.Log($"{prop.Key}: {prop.Value}");
        }
    }

    //プレイヤーのインプットを受けいれて移動可能にする
    public void SetMovable()
    {
        m_canMove = true;
    }

    //プレイヤーがゴールしたかを設定する
    public void SetGoaled()
    {
        m_isGoaled = true;
    }

    //自分が攻撃されたことを設定する
    public void SetIsAttacked()
    {
        if(!m_isUsingKiller && !m_isUsingStar)
		{
            m_isAttacked = true;
        }
    }

    public void SetIsUsingStar()
	{
        //ルームプロパティの自分の無敵状態を名前を使って検索、変更を行う
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        string name = PhotonNetwork.NickName + "Invincible";
        hashtable[name] = 1;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
    }

    public void SetIsUsingJet()
	{
        m_isUsingJet = true;
	}

    //スターを使用しているかを取得する
    public bool GetIsUsingStar()
	{
        return m_isUsingStar;
	}

    //ホストへクリアタイムを送る
    [PunRPC]
    void TellRecordTime(string name, float time)
    {
        GameObject.Find("SceneDirector").GetComponent<InGameScript>().AddGoaledPlayerNameAndRecordTime(name, time);
    }

    //自身がレースの参加の用意ができたかホストに送る
    [PunRPC]
    private void TellReadyOK()
    {
        if (!m_isToldReady)
        {
            GameObject.Find("SceneDirector").GetComponent<InGameScript>().AddReadyPlayerNum();
            m_isToldReady = true;
        }
    }

    private void OnCollisionEnter(Collision col)
	{
        //衝突対象が他のプレイヤーならば
        if(col.gameObject.tag == "Player")
        {
            Player pl = col.gameObject.GetComponent<PhotonView>().Owner;
            string plName = pl.NickName + "Invincible";

            bool isCrash = false;
            int stat = (PhotonNetwork.CurrentRoom.CustomProperties[plName] is int value) ? value : 0;

            if (stat == 1 && !m_isUsingStar)
			{
                isCrash = true;
			}

            //そのプレイヤーがスターを使用していたら
            if (isCrash)
			{
                m_isAttacked = true;
                Debug.Log("Hitted");
			}
		}
	}

    private void Update()
	{
        m_corseDir = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.GetComponent<WayPointChecker>().GetCurrentWayPoint();
        m_corseDir.Normalize();
        m_corseDir.y = 0.0f;

        //現在のシーンがインゲームでカウントダウンが終了して動ける状態ならば
        if (SceneManager.GetActiveScene().name == "DemoInGame" && m_canMove)
        {
            // 自身が生成したオブジェクトだけに移動処理を行う
            if (photonView.IsMine)
            {
                //前方向に移動
                m_moveDir = this.transform.forward * (Input.GetAxis("Vertical"));
                //移動スピードを計算する。
                if (m_isUsingKiller)
                {
                    m_moveSpeed = m_moveDir * MOVE_POWER_USING_KILLER;
                }
                else if (m_isUsingStar)
				{
                    m_moveSpeed = m_moveDir * MOVE_POWER_USING_STAR;
                }
                else if(m_isUsingJet)
				{
                    m_moveSpeed = m_moveDir * MOVE_POWER_USING_JET;
                    
                }
				else
				{
                    m_moveSpeed = m_moveDir * MOVE_POWER;
                }

                //入力による回転量
                m_rot = new Vector3(0.0f, Input.GetAxis("Horizontal") * ROT_POWER, 0.0f);
            }

            //ゴールしていなったら
            if(!m_isGoaled)
			{
                //走行時間をゲームタイムで計測し続ける。
                m_runningTime += Time.deltaTime;
            }
			else if(!m_isToldRecord)
			{
                //クリアタイムをホストだけに送る
                photonView.RPC(nameof(TellRecordTime), RpcTarget.MasterClient, PhotonNetwork.NickName, m_runningTime);

                m_isToldRecord = true;
            }

            //Yキル
            if (this.transform.position.y <= -2.0f)
            {
                this.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            }
        }
    }

    //環境に依存されない、一定期間のUpdate関数（移動はここにかくこと）
    private void FixedUpdate()
    {
        //自分が攻撃されていなければ
        if(!m_isAttacked)
		{
            //前方へ加速
            m_rb.AddForce(m_moveSpeed - m_rb.velocity);

            Transform appliedTrasnform = this.transform;
            appliedTrasnform.Rotate(m_rot);

            //コースの向きとプレイヤーの前方向が45度以内であれば
            if (Vector3.Dot(m_corseDir, appliedTrasnform.forward) >= 0.7f)
			{
                //通常通りの回転を適用する
                transform.Rotate(m_rot);
                m_prevTrasnform = this.transform.rotation;
            }
			else
			{
                this.transform.rotation = m_prevTrasnform;

                if (Vector3.Dot(m_corseDir, this.transform.forward) < 0.7f)
				{
                    Quaternion rot;
                   
                    rot = Quaternion.LookRotation(m_corseDir- this.transform.forward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);

                    m_prevTrasnform = this.transform.rotation;
                }
			}
        }
		//攻撃されていたら
		else
		{
            //硬直時間をゲーム時間で増やす
            m_stiffenTime += Time.deltaTime;
            //設定した最大硬直時間を超えたら
            if(m_stiffenTime >= MAX_STIFFIN_TIME)
			{
                //計測した硬直時間をリセット
                m_stiffenTime = 0.0f;
                //攻撃フラグを直す
                m_isAttacked = false;
            }
		}
        if(m_isUsingJet)
		{
            m_dashTime += Time.deltaTime;
            if(m_dashTime >= MAX_DASH_TIME)
			{
                m_isUsingJet = false;
                m_dashTime = 0.0f;
			}
		}

        

		if (m_isUsingStar)
		{
			m_starTime += Time.deltaTime;
			if (m_starTime >= MAX_STAR_REMAIN_TIME)
			{
				m_starTime = 0.0f;
				m_isUsingStar = false;

				var hashtable = new ExitGames.Client.Photon.Hashtable();
				string name = PhotonNetwork.NickName + "Invincible";
				hashtable[name] = 0;
				PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }
		}
	}
}