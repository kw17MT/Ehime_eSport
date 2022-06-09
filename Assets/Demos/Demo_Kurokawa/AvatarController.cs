using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacksを継承して、photonViewプロパティを使えるようにする
public class AvatarController : MonoBehaviourPunCallbacks
{
    Rigidbody m_rb = null;                              //割り当てられたリジッドボディ
    Vector3 m_moveDir = Vector3.zero;                   //移動する方向
    Vector3 m_rot = Vector3.zero;                       //どちらに回転するかの向き
    private GameObject m_paramManager = null;           //パラメータを保存するインスタンス（シーン跨ぎ）
    private bool m_canMove = false;                     //移動が制限されていないか
    private float m_runningTime = 0.0f;                 //走行時間
    private bool m_isGoaled = false;                    //自分はゴールしたか
    private bool m_isToldRecord = false;                //自分の走破レコードをホストクライアントに送ったかどうかのフラグ
    private bool m_isToldReady = false;                 //ルームに参加して準備ができたことを一度だけ通信するためのフラグ

    public float MOVE_POWER = 100.0f;                   //リジッドボディにかける移動の倍率
    public float ROT_POWER = 1.0f;                      //ハンドリング

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
        //自分が生成されたインスタンスであれば
        if(photonView.IsMine)
		{
            //探しやすい名前を付ける。（ヒエラルキーにも適用される）
            gameObject.name = "OwnPlayer";
            //タグをつける
            gameObject.tag = "OwnPlayer";
		}

        //1秒間に何回通信するか
        PhotonNetwork.SendRate = 3;
        //1秒間に何回同期を行うか
        PhotonNetwork.SerializationRate = 3;
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

    //ゲーム上へクリアタイムを送る
    [PunRPC]
    void TellRecordTime(float time)
	{
        GameObject.Find("SceneDirector").GetComponent<InGameScript>().AddGoaledPlayerNameAndRecordTime(PhotonNetwork.NickName, time);
    }

    //自身がレースの参加の用意ができたかホストに送る
    [PunRPC]
    private void TellReadyOK()
	{
        if(!m_isToldReady)
		{
            GameObject.Find("SceneDirector").GetComponent<InGameScript>().AddReadyPlayerNum();
            m_isToldReady = true;
        }
    }

    private void Update()
	{
        //現在のシーンがインゲームでカウントダウンが終了して動ける状態ならば
        if (SceneManager.GetActiveScene().name == "DemoInGame" && m_canMove)
        {
            // 自身が生成したオブジェクトだけに移動処理を行う
            if (photonView.IsMine)
            {
                //前方向に移動
                m_moveDir = this.transform.forward * (Input.GetAxis("Vertical") * MOVE_POWER);
                //回転
                m_rot = new Vector3(0.0f, Input.GetAxis("Horizontal") * ROT_POWER, 0.0f);

                //テストでボタンを押したらバナナが出るようにする。
                if (Input.GetKeyDown(KeyCode.K))
                {
                    Vector3 orangePeelPos = this.transform.position + (this.transform.forward * -2.0f);
                    photonView.RPC(nameof(InstantiateOrangePeel), RpcTarget.All, orangePeelPos);
                }
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
                photonView.RPC(nameof(TellRecordTime), RpcTarget.MasterClient, m_runningTime);

                m_isToldRecord = true;
            }

            //Yキル
            if (this.transform.position.y <= -2.0f)
            {
                this.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            }
        }
    }

    //オレンジの皮を自分の後ろに置く
    [PunRPC]
    public void InstantiateOrangePeel(Vector3 popPos)
    {
        //ゲーム全体で生成したオレンジの皮の数を把握できるように、ローカルのインスタンスでもメモする。
        m_paramManager.GetComponent<ParamManage>().AddOrangePeelNum();
        
        //ローカルでオレンジの皮を指定された座標に生成
        var orange = PhotonNetwork.Instantiate("OrangePeel", popPos, Quaternion.identity);
        //こちら側でも名前に総合生成数を付与する。
        orange.name = "OrangePeel" + m_paramManager.GetComponent<ParamManage>().GetOrangePeelNumOnField();
    }

    //環境に依存されない、一定期間のUpdate関数（移動はここにかくこと）
    private void FixedUpdate()
    {
        m_rb.AddForce(m_moveDir);
        transform.Rotate(m_rot);
    }
}