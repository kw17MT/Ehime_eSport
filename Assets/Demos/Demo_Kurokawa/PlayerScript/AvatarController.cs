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
    Vector3 m_prevMoveDir = Vector3.zero;               //前フレームの移動方向
    Vector3 m_moveSpeed = Vector3.zero;                 //移動スピード
    Vector3 m_rot = Vector3.zero;                       //どちらに回転するかの向き
    Vector3 m_corseDir = Vector3.zero;                  //現在走っているコースの大まかな方向
    Vector3 m_alongWallDir = Vector3.zero;              //壁ずりした時の修正済み移動方向          
    float m_distanceToNextWayPoint = 0.0f;              //次のウェイポイントへの距離
    private GameObject m_paramManager = null;           //パラメータを保存するインスタンス（シーン跨ぎ）
    private GameObject m_orepation = null;              //プレイヤーの操作状態をもつインスタンス
    private RaceAIScript m_aiScript = null;             //プレイヤーにつけているAIScript（ゴールした時にキャラの操作をAIに移管する用）
    private AvatarController m_avaCon = null;           //操作をAIに移管した時にプレイヤーの操作をOFFにするためのインスタンス（このスクリプト）
    private AlongWall m_alongWall = null;               //壁ずり時の移動方向を更新するインスタンス
    private bool m_canMove = false;                     //移動が制限されていないか
    private float m_runningTime = 0.0f;                 //走行時間
    private float m_stiffenTime = 0.0f;                 //攻撃された時の硬直している時間
    private float m_starTime = 0.0f;                    //スター時間の使用している時間
    private float m_dashTime = 0.0f;                    //キノコを使ってダッシュしている時間
    private float m_killerTime = 0.0f;                  //キラーを使用している時間
    private float m_spinedAngle = 0.0f;                 //被弾して回転した総量
    private float m_frameCounter = 0.0f;                //ゲームタイムを用いてどのくらい時間がたったかを記録する変数
    private bool m_isGoaled = false;                    //自分はゴールしたか
    private bool m_isToldRecord = false;                //自分の走破レコードをホストクライアントに送ったかどうかのフラグ
    private bool m_isToldReady = false;                 //ルームに参加して準備ができたことを一度だけ通信するためのフラグ
    private bool m_isUsingStar = false;                 //現在、スターを使用しているか
    private bool m_isUsingKiller = false;               //現在、キラーを使用しているか
    private bool m_isUsingJet = false;                  //現在、ジェットを使用しているか
    private bool m_isAttacked = false;                  //攻撃されたか
    private bool m_hittedWall = false;                  //壁に当たっているか
    private bool m_isInvincible = false;                //自分のプレイヤーは無敵化
    private Quaternion m_prevTrasnform;                 //前回の回転の度合い
    
   
    private float MOVE_POWER_USING_STAR = 35.0f;         //スター使用時のリジッドボディにかける移動の倍率
    private float MOVE_POWER_USING_JET = 50.0f;          //ジェット使用時のリジッドボディにかける移動の倍率
    private float MOVE_POWER_USING_KILLER = 10.0f;       //キラー使用時のリジッドボディにかける移動の倍率
    
    public float MAX_STAR_REMAIN_TIME = 10.5f;          //スターの最大継続時間
    public float MAX_KILLER_REMAIN_TIME = 2.0f;         //キラーの最大継続時間
    public float MAX_DASH_TIME = 1.0f;                  //ダッシュの最大継続時間

    private float KILLER_HANDLING_RATE = 5.0f;           //キラーを使用した際のカメラの追従速度
    public float SPIN_AMOUNT = 6.0f;                    //被弾時の回転率
    private float FIX_MOVESPEED_POWER_AFTER_KILLER = 10.0f; //キラー終了時に通常スピードに戻すための変数          
    private float UPDATE_DISTANCE_TIMING = 0.1f;        //次のウェイポイントとの距離を更新するタイミング

    //キャラごとに性能差をつけるパラメータ
    public float ROT_POWER = 50.0f;                      //ハンドリング
    public float MOVE_POWER = 25.0f;                   //リジッドボディにかける移動の倍率
    public float MAX_STIFFIN_TIME = 1.5f;               //攻撃が当たった時の最大硬直時間

    void Start()
    {
        //リジッドボディを取得
        m_rb = GetComponent<Rigidbody>();
        //インゲーム中であれば
        if (SceneManager.GetActiveScene().name[0..2] == "08")
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
        //壁ずりインスタンスを生成
        m_alongWall = new AlongWall();

        m_orepation = GameObject.Find("OperationSystem");


        //1秒間に何回通信するか
        PhotonNetwork.SendRate = 30;
        //1秒間に何回同期を行うか
        PhotonNetwork.SerializationRate = 30;

        //プレイヤーについているスクリプトの取得
        m_aiScript = this.GetComponent<RaceAIScript>();
        //プレイヤーについているスクリプトの取得
        m_avaCon = this.GetComponent<AvatarController>();
    }

    //ルームプロパティの何かが更新された時の関数
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //更新されたルームのカスタムプロパティのペアを取得していく
        foreach (var prop in propertiesThatChanged)
        {
            //各プレイヤーの無敵状態を示すキー部分を作成
            string name = PhotonNetwork.NickName + "Invincible";
            //更新された部分のキー部分をStringで取得
            string key = prop.Key.ToString();
            //自分自身の無敵状態をルームプロパティ上の無敵状態に同期させる
            if (name == key)
			{
                bool isPlayerInvincible = false;
                //バリューをint型で取得
                int isInvincivle = (PhotonNetwork.CurrentRoom.CustomProperties[prop.Key] is int value) ? value : 0;
                if (isInvincivle == 1)
				{
                    //スターを使用している状態にする
                    isPlayerInvincible = true;
				}
                m_isInvincible = isPlayerInvincible;
            }
            //Debug.Log($"{prop.Key}: {prop.Value}");
        }
    }

    //プレイヤーのインプットを受けいれて移動可能にする
    public void SetMovable()
    {
        m_canMove = true;
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        //freeze使うとRotationまでチェック外れるぽいのでチェック入れる
        rb.freezeRotation = true;
    }

    //プレイヤーがゴールしたかを設定する
    public void SetGoaled()
    {
        m_isGoaled = true;
    }

    //プレイヤーがゴールしたかを取得する
    public bool GetGoaled()
    {
        return m_isGoaled;
    }
    
    //自分が攻撃されたことを設定する
    public void SetIsAttacked()
    {
        //無敵状態（スターもキラーも使用していない状態）ならば
        if(!m_isInvincible)
		{
            //攻撃された
            m_isAttacked = true;
        }
    }

    //スターを使用している状態にする
    public void SetIsUsingStar()
	{
        //スターを使用している状態
        m_isUsingStar = true;
        m_isInvincible = true;
        //オンラインモードならば
        if (!PhotonNetwork.OfflineMode)
        {
            //ルームプロパティの自分の無敵状態を名前を使って検索、変更を行う
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            string name = PhotonNetwork.NickName + "Invincible";
            hashtable[name] = 1;
            //ルームプロパティを更新
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        }
    }

    //ジェットを使用している状態にする
    public void SetIsUsingJet()
	{
        m_isUsingJet = true;
    }

    //キラーを使用している状態にする
    public void SetIsUsingKiller()
	{
        //キラーを使用している
        m_isUsingKiller = true;
        m_isInvincible = true;
        if (!PhotonNetwork.OfflineMode)
        {
            //ルームプロパティの自分の無敵状態を名前を使って検索、変更を行う
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            string name = PhotonNetwork.NickName + "Invincible";
            hashtable[name] = 1;
            //ルームプロパティを更新
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        }
    }

    //スターを使用しているかを取得する
    public bool GetIsUsingStar()
	{
        return m_isUsingStar;
	}

    public bool GetIsUsingKiller()
	{
        return m_isUsingKiller;
	}

    //プレイヤーは攻撃されているかを取得する
    public bool GetIsAttacked()
	{
        return m_isAttacked;
	}

    //プレイヤーは無敵状態化を取得する
    public bool GetIsInvincible()
	{
        return m_isInvincible;
	}

    //次のウェイポイントへの距離を取得する
    public float GetDistanceToNextWayPoint()
	{
        return m_distanceToNextWayPoint;
	}

    //ホストへ自分の名前とクリアタイムを送る
    [PunRPC]
    public void TellRecordTime(string name, float time)
    {
        GameObject.Find("SceneDirector").GetComponent<InGameScript>().AddGoaledPlayerNameAndRecordTime(name, time, true);
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

    [PunRPC]
    private void DestroyItemWithName(string name)
	{
        GameObject.Find("SceneDirector").GetComponent<ItemStateCommunicator>().DestroyItemWithName(name);
    }

    private void JudgeIsAttackedBySnapper(Collision col)
	{
        if (col.gameObject.name.Length >= 7 && col.gameObject.name[0..7] == "Snapper")
        {
            //自分のニックネームの番号部分だけを取得
            string idStr = PhotonNetwork.NickName;
            int id = int.Parse(idStr[6].ToString());
            //タイを発射した人と自分が違うならば
            if (col.gameObject.GetComponent<SnapperController>().GetOwnerID() != id)
            {
                //自分のゲーム内のタイインスタンスの削除
                photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
                //キラーもスターも使っていなければ
                if (!m_isInvincible)
                {
                    //攻撃された
                    m_isAttacked = true;
                }
                //Debug.Log("Attacked By Snapper(Clone)");
            }
        }
    }

    private void JudgeIsStepOnOrangePeel(Collision col)
    {
        if (col.gameObject.name.Length >= 10 && col.gameObject.name[0..10] == "OrangePeel")
        {
            //そのインスタンスを削除
            photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
        }
    }

    private void JudgeIsAttackedByOtherPlayer(Collision col)
	{
        if (col.gameObject.tag == "Player")
        {
            //オンラインモードならば
            if (!PhotonNetwork.OfflineMode)
            {
                //コリジョンの持ち主のPhotonNetworkに関する変数を取得
                Player pl = col.gameObject.GetComponent<PhotonView>().Owner;
                //そのプレイヤーの無敵状態をルームプロパティから持ってくる
                string plName = pl.NickName + "Invincible";
                int stat = (PhotonNetwork.CurrentRoom.CustomProperties[plName] is int value) ? value : 0;
                //そのプレイヤーが無敵で、自分が無敵状態でなければ
                if (stat == 1 && !m_isInvincible)
                {
                    //自分のプレイヤーは攻撃を受けた
                    m_isAttacked = true;
                }
            }
            //オフラインプレイならば
            else
            {
                //あたったプレイヤーが無敵状態かを取得
                bool isInvinciblePlayer = GameObject.Find(col.gameObject.name).GetComponent<AICommunicator>().GetIsInvincible();

                //無敵プレイヤーならば
                if (isInvinciblePlayer)
                {
                    //攻撃された
                    m_isAttacked = true;
                }
            }
        }
    }

    private void JudgeIsHitWall(Collision col)
	{
        if (col.gameObject.tag == "Wall")
        {
            //壁に当たった
            m_hittedWall = true;
            //現在の移動方向をもとに壁ずり移動の方向を計算
            m_alongWall.CollisionEnter(col, m_rb, ref m_moveDir);
            //移動方向を更新
            m_alongWallDir = m_moveDir;
        }
    }

    private void MoveByUsingKiller()
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
            this.transform.position += direction / 2.0f /** 1.5f*/;

            //キラーを使用している時間をゲームタイムでインクリメント
            m_killerTime += Time.deltaTime;
            //最大継続時間を超えたら
            if (m_killerTime >= MAX_KILLER_REMAIN_TIME)
            {
                //時間をリセット
                m_killerTime = 0.0f;
                //キラーを使っていない状態にする
                m_isUsingKiller = false;
                this.gameObject.GetComponent<ObtainItemController>().SetItemNothing();

                m_rb.velocity = Vector3.zero;
                m_moveSpeed = direction * MOVE_POWER_USING_KILLER * FIX_MOVESPEED_POWER_AFTER_KILLER;
                m_rb.AddForce(m_moveSpeed);
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
                }
            }

            //キラー使用中は以降の処理を実行しない
            return;
        }
    }

    private void BreakInvincible()
    {
        if (!m_isUsingStar && !m_isUsingKiller)
        {
            //ルームプロパティを毎フレーム更新させないように、無敵を解除した時だけルームプロパティ側もFALSEに
            if (m_isInvincible)
            {
                m_isInvincible = false;
                if (!PhotonNetwork.OfflineMode)
                {
                    var hashtable = new ExitGames.Client.Photon.Hashtable();
                    string name = PhotonNetwork.NickName + "Invincible";
                    hashtable[name] = 0;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
                }
            }
        }
    }

    //ゴールした際に使用するコントローラーのスクリプトを変える
    private void SwitchUsingControllerScript()
    {
        if (SceneManager.GetActiveScene().name == "08_GameScene")
        {
            if (m_isGoaled && !m_aiScript.enabled)
            {
                //AIスクリプトをONにする
                m_aiScript.enabled = true;
                //AIで動けるようにする
                m_aiScript.SetCanMove(true);

                Destroy(Camera.main.GetComponent<CameraMove>());
                Camera.main.gameObject.AddComponent<AICameraScript>();
                Camera.main.gameObject.GetComponent<AICameraScript>().SetPlayer(gameObject);
            }
            //AIモードがONであれば
            if (m_aiScript.enabled)
            {
                //プレイヤーの操作スクリプトをOFFにする
                this.GetComponent<AvatarController>().enabled = false;
            }
        }
        else if(SceneManager.GetActiveScene().name == "08_EasyGameScene")
		{
            if (m_isGoaled)
            {
                this.GetComponent<AvatarController>().enabled = false;
            }
        }
    }

    private void CalcRotation()
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

    private void SpinAndStiffenWhenAttacked()
	{
        m_spinedAngle += SPIN_AMOUNT;

        if (m_spinedAngle <= 360.0f)
        {
            this.transform.Rotate(0.0f, SPIN_AMOUNT, 0.0f, Space.World); // 回転角度を設定            
        }

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

    private void MeasureJetTime()
	{
        //ジェットを使用しているならば
        if (m_isUsingJet)
        {
            //使用時間を増やして
            m_dashTime += Time.deltaTime;
            //最大継続時間を超えたら
            if (m_dashTime >= MAX_DASH_TIME)
            {
                //使っていないことにして
                m_isUsingJet = false;
                //タイムもリセット
                m_dashTime = 0.0f;
            }
        }
    }

    private void MeasureStarTime()
	{
        if (m_isUsingStar)
        {
            m_starTime += Time.deltaTime;
            if (m_starTime >= MAX_STAR_REMAIN_TIME)
            {
                m_starTime = 0.0f;
                m_isUsingStar = false;

                if (!PhotonNetwork.OfflineMode)
                {
                    //ルームプロパティの無敵状態も戻しておく
                    var hashtable = new ExitGames.Client.Photon.Hashtable();
                    string name = PhotonNetwork.NickName + "Invincible";
                    hashtable[name] = 0;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
                }
            }
        }
    }

    //何かが衝突したら
    private void OnCollisionEnter(Collision col)
	{
        //タイと当たったら
        JudgeIsAttackedBySnapper(col);

        //オレンジの皮をひいたら
        JudgeIsStepOnOrangePeel(col);

        //衝突対象が他のプレイヤーならば
        JudgeIsAttackedByOtherPlayer(col);


        //衝突先が壁の時
        JudgeIsHitWall(col);
    }

    private void YKill()
	{
        if (this.gameObject.transform.position.y <= -2.0f)
        {
            Vector3 pos = this.gameObject.transform.position;
            pos.y = 2.0f;
            this.gameObject.transform.position = pos;
        }
    }

	private void OnCollisionStay(Collision col)
	{
        if (col.gameObject.tag == "Wall")
        {
            //壁に当たった
            m_hittedWall = true;
            //壁に対して自分の前方向がめり込む方向ならば
            if(Vector3.Dot(col.contacts[0].normal, this.transform.forward) <= 0.0f)
			{
                m_alongWall.CollisionEnter(col, m_rb, ref m_moveDir);
            }
            m_alongWallDir = m_moveDir;
        }
    }

    //環境に依存されない、一定期間のUpdate関数（移動はここにかくこと）
    private void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name[0..2] != "08")
		{
            return;
		}

        SwitchUsingControllerScript();

        //キラー使用時の移動。キラー使用時はこの関数以降の処理は行わない
        MoveByUsingKiller();

        BreakInvincible();

        
        //攻撃されていれば
        if (m_isAttacked)
        {
            SpinAndStiffenWhenAttacked();
        }
        //攻撃されていなければ
        else
        {
            CalcRotation();

            //壁ずり状態ならば
            if (m_hittedWall)
            {
                //コースの進行方向と壁ずり移動方向が正反対ならば
				if (Vector3.Dot(m_alongWallDir, m_corseDir) < 0.0f)
				{
                    //コースの進行方向に直す
					m_alongWallDir *= -1.0f;
				}

				//壁に沿う移動方向を用いてスピードを計算
				m_moveSpeed = m_alongWallDir * MOVE_POWER;
                m_hittedWall = false;
            }


            //そうでないなら通常通り移動
            m_rb.AddForce(m_moveSpeed - m_rb.velocity);
        }

        MeasureJetTime();

        MeasureStarTime();
       

        //インゲームならば
        if (SceneManager.GetActiveScene().name[0..2] == "08")
        {
            //オフラインモードならば
            if (PhotonNetwork.OfflineMode)
            {
                //次のウェイポイントへの距離
                Vector3 distanceToNextWayPoint = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;
                //自分も持っておく
                m_distanceToNextWayPoint = distanceToNextWayPoint.magnitude;
            }
            else
            {
                //経過時間を計測する
                m_frameCounter += Time.deltaTime;
                //ゲームない時間が一定時間たったら
                if (m_frameCounter >= UPDATE_DISTANCE_TIMING)
                {
                    //次のウェイポイントへの距離
                    Vector3 distanceToNextWayPoint = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;
                    //自分も持っておく
                    m_distanceToNextWayPoint = distanceToNextWayPoint.magnitude;


                    string key = PhotonNetwork.NickName + "Distance";

                    //オンラインで取得できるようにカスタムプロパティを更新
                    var hashtable = new ExitGames.Client.Photon.Hashtable();
                    hashtable[key] = distanceToNextWayPoint;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

                    //リセット
                    m_frameCounter = 0.0f;
                }
            }
        }

        YKill();

        if (m_isGoaled && !m_isToldRecord)
        {
            //クリアタイムをホストだけに送る
            photonView.RPC(nameof(TellRecordTime), RpcTarget.MasterClient, PhotonNetwork.NickName, m_runningTime);
            //報告済み
            m_isToldRecord = true;

            if (PhotonNetwork.OfflineMode)
            {
                GameObject.Find("SceneDirector").GetComponent<InGameScript>().SetStopToTellAIRecord();
            }

            //ゴール後不要なUI群を削除
            Destroy(GameObject.Find("Lap"));
            Destroy(GameObject.Find("RankingImage"));
            Destroy(GameObject.Find("Item"));
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
        if (SceneManager.GetActiveScene().name[0..2] == "08" && m_canMove)
        {
            // 自身が生成したオブジェクトだけに移動処理を行う
            if (photonView.IsMine)
            {
                //移動方向
                Vector3 dir = Vector3.zero;
                //プレイヤーの操作状態を取得する
                switch (m_orepation.GetComponent<Operation>().GetNowOperationAndPower().dir)
                {
                    case "right":
                        //入力による回転量、フレームレートに回転量が依存しないようにゲームタイムを乗算
                        m_rot = new Vector3(0.0f, m_orepation.GetComponent<Operation>().GetNowOperationAndPower().power * ROT_POWER * Time.deltaTime, 0.0f);
                        break;
                    case "left":
                        //入力による回転量、フレームレートに回転量が依存しないようにゲームタイムを乗算
                        m_rot = new Vector3(0.0f, m_orepation.GetComponent<Operation>().GetNowOperationAndPower().power * ROT_POWER * Time.deltaTime, 0.0f);
                        break;
                    default:
                        m_rot = Vector3.zero;
                        break;
                }
                dir += this.transform.forward/* * Input.GetAxis("Vertical")*/;
                m_moveDir = dir;


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
            }

            //ゴールしていなったら
            if (!m_isGoaled)
            {
                //走行時間をゲームタイムで計測し続ける。
                m_runningTime += Time.deltaTime;
            }
        }

        if(m_orepation.GetComponent<Operation>().GetIsDoubleTouch())
		{
            this.GetComponent<ObtainItemController>().SetUseItem();
        }
    }

    private void LateUpdate()
    {
        //キラーを使用時にコースの方向を向くように回転させる
        if (m_isUsingKiller)
        {
            //回転について、FixedUpdateでやるとガクつくためここで更新
            Quaternion rot;
            //自分の前方向からコースの向きへの回転を計算
            Vector3 newForward = (this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position) - this.transform.forward;
            newForward.y = 0.0f;
            rot = Quaternion.LookRotation(newForward);
            //緩やかにして適用
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * KILLER_HANDLING_RATE);
            //m_rb.MoveRotation(Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * KILLER_HANDLING_RATE));
            //現在の回転を保存
            m_prevTrasnform = this.transform.rotation;
            return;
        }

        m_prevMoveDir = m_moveDir;
    }
}