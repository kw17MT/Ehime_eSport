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
    Vector3 m_corseDir = Vector3.zero;                  //現在走っているコースの大まかな方向
    Vector3 m_alongWallDir = Vector3.zero;
    private GameObject m_paramManager = null;           //パラメータを保存するインスタンス（シーン跨ぎ）
    private bool m_canMove = false;                     //移動が制限されていないか
    private float m_runningTime = 0.0f;                 //走行時間
    private float m_stiffenTime = 0.0f;                 //攻撃された時の硬直している時間
    private float m_starTime = 0.0f;                    //スター時間の使用している時間
    private float m_dashTime = 0.0f;                    //キノコを使ってダッシュしている時間
    private float m_killerTime = 0.0f;                  //キラーを使用している時間
    private float m_spinedAngle = 0.0f;                 //被弾して回転した総量
    private bool m_isGoaled = false;                    //自分はゴールしたか
    private bool m_isToldRecord = false;                //自分の走破レコードをホストクライアントに送ったかどうかのフラグ
    private bool m_isToldReady = false;                 //ルームに参加して準備ができたことを一度だけ通信するためのフラグ
    private bool m_isUsingStar = false;                 //現在、スターを使用しているか
    private bool m_isUsingKiller = false;               //現在、キラーを使用しているか
    private bool m_isUsingJet = false;                  //現在、ジェットを使用しているか
    private bool m_isAttacked = false;                  //攻撃されたか
    private bool m_hittedWall = false;                  //壁に当たっているか
    private Quaternion m_prevTrasnform;                 //前回の回転の度合い
    

    public float MOVE_POWER = 25.0f;                  　//リジッドボディにかける移動の倍率
    public float MOVE_POWER_USING_STAR = 35.0f;         //スター使用時のリジッドボディにかける移動の倍率
    public float MOVE_POWER_USING_JET = 50.0f;          //ジェット使用時のリジッドボディにかける移動の倍率
    public float MOVE_POWER_USING_KILLER = 60.0f;       //キラー使用時のリジッドボディにかける移動の倍率
    public float ROT_POWER = 0.5f;                      //ハンドリング
    public float MAX_STAR_REMAIN_TIME = 10.5f;          //スターの最大継続時間
    public float MAX_KILLER_REMAIN_TIME = 3.0f;         //キラーの最大継続時間
    public float MAX_DASH_TIME = 1.0f;                  //ダッシュの最大継続時間
    public float MAX_STIFFIN_TIME = 1.5f;               //攻撃が当たった時の最大硬直時間
    public float KILLER_HANDLING_RATE = 5.0f;           //キラーを使用した際のカメラの追従速度
    public float SPIN_AMOUNT = 0.5f;                    //被弾時の回転率

    private AlongWall m_alongWall = null;

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

        //前回の回転を初期化
        m_prevTrasnform = this.transform.rotation;

        m_alongWall = new AlongWall();


        //1秒間に何回通信するか
        PhotonNetwork.SendRate = 3;
        //1秒間に何回同期を行うか
        PhotonNetwork.SerializationRate = 3;
    }

    //ルームプロパティの何かが更新された時の関数
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //更新されたルームのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in propertiesThatChanged)
        {
            //各プレイヤーの無敵状態を示すキー部分を作成
            string name = PhotonNetwork.NickName + "Invincible";
            //更新された部分のキー部分をStringで取得
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
        //スターもキラーも使用していない状態ならば
        if(!m_isUsingKiller && !m_isUsingStar)
		{
            //攻撃された
            m_isAttacked = true;
        }
    }

    //スターを使用している状態にする
    public void SetIsUsingStar()
	{
        //ルームプロパティの自分の無敵状態を名前を使って検索、変更を行う
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        string name = PhotonNetwork.NickName + "Invincible";
        hashtable[name] = 1;
        //ルームプロパティを更新
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
    }

    //ジェットを使用している状態にする
    public void SetIsUsingJet()
	{
        m_isUsingJet = true;
	}

    //キラーを使用している状態にする
    public void SetIsUsingKiller()
	{
        m_isUsingKiller = true;
	}

    //スターを使用しているかを取得する
    public bool GetIsUsingStar()
	{
        return m_isUsingStar;
	}

    public bool GetIsAttacked()
	{
        return m_isAttacked;
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
        //何回も送らないようにする（通信の関係上フラグで見極める）
        if (!m_isToldReady)
        {
            GameObject.Find("SceneDirector").GetComponent<InGameScript>().AddReadyPlayerNum();
            m_isToldReady = true;
        }
    }

    //何かが衝突したら
    private void OnCollisionEnter(Collision col)
	{
        //衝突対象が他のプレイヤーならば
        if(col.gameObject.tag == "Player")
        {
            //コリジョンの持ち主のPhotonNetworkに関する変数を取得
            Player pl = col.gameObject.GetComponent<PhotonView>().Owner;
            //そのプレイヤーの無敵状態をルームプロパティから持ってくる
            string plName = pl.NickName + "Invincible";
            int stat = (PhotonNetwork.CurrentRoom.CustomProperties[plName] is int value) ? value : 0;

            //攻撃を受けたか
            bool isCrash = false;
            //そのプレイヤーが無敵で、自分がスターもキラーも使っていなけば
            if (stat == 1 && !m_isUsingStar && !m_isUsingKiller)
			{
                //攻撃を受けた
                isCrash = true;
			}

            if (isCrash)
			{
                //自分のプレイヤーは攻撃を受けた
                m_isAttacked = true;
			}
		}
        //プレイヤーでなく、タイならば
		else if(col.gameObject.name == "Snapper")
		{
            //キラーもスターも使っていなければ
            if(!m_isUsingKiller && !m_isUsingStar)
			{
                //攻撃された
                m_isAttacked = true;
			}
		}

        if(col.gameObject.tag == "Wall")
		{
            m_hittedWall = true;
            Debug.Log("WALL");
            m_alongWall.CollisionEnter(col, m_rb, ref m_moveDir);
            m_alongWallDir = m_moveDir;
            Debug.Log("AvatarController : m_moveDir = " + m_alongWallDir);
        }
    }

	private void OnCollisionStay(Collision col)
	{
        if (col.gameObject.tag == "Wall")
        {
            m_hittedWall = true;
            Debug.Log("WALL");
            m_alongWall.CollisionEnter(col, m_rb, ref m_moveDir);
            m_alongWallDir = m_moveDir;
            Debug.Log("AvatarController : m_moveDir = " + m_alongWallDir);
        }
    }

	private void Update()
	{
        //コースの向きを現在のウェイポイント通過状況から調べる
        m_corseDir = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.GetComponent<WayPointChecker>().GetCurrentWayPoint();
        //正規化
        m_corseDir.Normalize();
        //高さの方向はいらない
        m_corseDir.y = 0.0f;

        //現在のシーンがインゲームでカウントダウンが終了して動ける状態ならば
        if (SceneManager.GetActiveScene().name == "DemoInGame" && m_canMove)
        {
            // 自身が生成したオブジェクトだけに移動処理を行う
            if (photonView.IsMine)
            {
                //前方向に移動
                m_moveDir = this.transform.forward * (Input.GetAxis("Vertical"));
                //キラーを使っている時の移動スピードを計算する。
                if (m_isUsingKiller)
                {
                    m_moveSpeed = m_moveDir * MOVE_POWER_USING_KILLER;
                }
                //スターを使っている時の移動スピードを計算する。
                else if (m_isUsingStar)
				{
                    m_moveSpeed = m_moveDir * MOVE_POWER_USING_STAR;
                }
                //キノコを使っている時の移動スピードを計算する。
                else if (m_isUsingJet)
				{
                    m_moveSpeed = m_moveDir * MOVE_POWER_USING_JET;
                }
                //通常時の移動スピードを計算する。
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

	private void LateUpdate()
	{
        if (m_isUsingKiller)
        {
            //回転について、FixedUpdateでやると呼び出し回数が少なすぎてガクつくためここで更新
            Quaternion rot;
            //自分の前方向からコースの向きへの回転を計算
            Vector3 newForward = (this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position) - this.transform.forward;
            newForward.y = 0.0f;
            rot = Quaternion.LookRotation(newForward);
            //緩やかにして適用
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * KILLER_HANDLING_RATE);
            //現在の回転を保存
            m_prevTrasnform = this.transform.rotation;
            return;
        }

        //入力による回転処理をさせない
        if (!m_isAttacked)
        { 
            //現在入力している回転を適用したTransformを適宜
            Transform appliedTrasnform = this.transform;
            appliedTrasnform.Rotate(m_rot);

            //コースの向きとプレイヤーの前方向が45度以内であれば
            if (Vector3.Dot(m_corseDir, appliedTrasnform.forward) >= 0.7f)
            {
                //回転を実際に適用する
                transform.Rotate(m_rot);
                //適切な回転を保存
                m_prevTrasnform = this.transform.rotation;
            }
            //横に向きすぎているならば
            else
            {

                //前回適用した、適切な回転で補正
                this.transform.rotation = m_prevTrasnform;

                //よこに向きすぎている
                if (Vector3.Dot(m_corseDir, this.transform.forward) < 0.7f)
                {
                    Quaternion rot;
                    //コースの向きに戻すような回転を計算して適用する
                    rot = Quaternion.LookRotation(m_corseDir - this.transform.forward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);

                    m_prevTrasnform = this.transform.rotation;
                }
            }
        }

        if(m_isAttacked)
		{
            m_spinedAngle += SPIN_AMOUNT;

            if(m_spinedAngle < 360.0f)
			{
                this.transform.Rotate(0.0f, SPIN_AMOUNT, 0.0f, Space.World); // 回転角度を設定            
            }
        }
    }

	//環境に依存されない、一定期間のUpdate関数（移動はここにかくこと）
	private void FixedUpdate()
    {
        //キラーを使っていたら
        if (m_isUsingKiller)
        {
            //次のウェイポイントへの向きを計算
			Vector3 direction = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;
            //正規化
            direction.Normalize();
            //高さはいらない
            direction.y = 0.0f;
            //TRASNFORMで位置を更新（Rigidbodyを使うと速さも出ないし、速くしたらコースアウトする
            this.transform.position += direction * 1.5f;

            //キラーを使用している時間をゲームタイムでインクリメント
            m_killerTime += Time.deltaTime;
            //最大継続時間を超えたら
            if(m_killerTime >= MAX_KILLER_REMAIN_TIME)
			{
                //時間をリセット
                m_killerTime = 0.0f;
                //キラーを使っていない状態にする
                m_isUsingKiller = false;
            }

            //アイテムの重ね掛けをさせないように、使用中だったアイテムの継続時間も減らしていく。
            if (m_isUsingJet)
            {
                m_dashTime += Time.deltaTime;
                if (m_dashTime >= MAX_DASH_TIME)
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

            //キラー使用中は以降の処理を実行しない
            return;
        }

        if(!m_isAttacked)
		{
            if(m_hittedWall)
			{
                m_moveSpeed = m_alongWallDir * MOVE_POWER;
                Debug.Log("moveSpeed : " + m_moveSpeed + " m_alongWallDir : " + m_alongWallDir);
                //前方へ加速
                
                m_hittedWall = false;
            }
            m_rb.AddForce(m_moveSpeed - m_rb.velocity);
        }
		//攻撃されていたら
		else
		{
            //硬直時間をゲーム時間で増やす
            m_stiffenTime += Time.deltaTime;
            //設定した最大硬直時間を超えたら

            if (m_stiffenTime >= MAX_STIFFIN_TIME)
			{
                //計測した硬直時間をリセット
                m_stiffenTime = 0.0f;
                //被弾時の回転リアクションの総回転量をリセット
                m_spinedAngle = 0.0f;
                //攻撃フラグを直す
                m_isAttacked = false;
            }
		}

        //ジェットを使用しているならば
        if(m_isUsingJet)
		{
            //使用時間を増やして
            m_dashTime += Time.deltaTime;
            //最大継続時間を超えたら
            if(m_dashTime >= MAX_DASH_TIME)
			{
                //使っていないことにして
                m_isUsingJet = false;
                //タイムもリセット
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

                //ルームプロパティの無敵状態も戻しておく
				var hashtable = new ExitGames.Client.Photon.Hashtable();
				string name = PhotonNetwork.NickName + "Invincible";
				hashtable[name] = 0;
				PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }
		}
	}
}