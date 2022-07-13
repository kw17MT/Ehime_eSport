using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//プレイヤーが取得しているアイテムの管理と使用するクラス。
public class ObtainItemController : MonoBehaviourPunCallbacks
{
    GameObject m_paramManager = null;           //パラメータを保存するインスタンス（シーン跨ぎ）
    EnItemType m_obtainItemType = EnItemType.enNothing;

    //アイテムシャッフル演出が終了したかフラグ
    bool m_isLotteryFinish = false;
    bool m_isUseItem = false;

    const float SPACE_BETWEEN_PLAYER_FRONT = 4.0f;
    const float SPACE_BETWEEN_PLAYER_BACK = -2.0f;

    //アイテムの種類
    enum EnItemType
	{
        enNothing = -1,
        enOrangePeel,                                   //オレンジの皮
        enOrangeJet,                                    //オレンジジュースジェット
        enTrain,                                        //坊ちゃん列車キラー
        enStar,                                         //スター
        enSnapperCannon,                                //タイ砲
        enItemTypeNum                                   //アイテムの種類の数
	}

    void Start()
	{
        //ゲーム中のパラメータを保存するインスタンスを取得
        m_paramManager = GameObject.Find("ParamManager");
	}

    [PunRPC]
    private void InstantiateItem(string prefabName, Vector3 popPos, int wayPointNumber = -1, int playerNumber = -1)
	{
        GameObject.Find("SceneDirector").GetComponent<ItemStateCommunicator>().PopItem(prefabName, popPos, wayPointNumber, playerNumber);
	}

    public void SetUseItem()
	{
        if(m_isLotteryFinish)
		{
            m_isUseItem = true;
        }
	}

    //ランダムなアイテムを抽選する
    public void GetRandomItem()
	{
        //何も持っていなければ
        if(m_obtainItemType == EnItemType.enNothing)
		{
            //アイテムのナンバーをランダムに取得
            int type = (int)Random.Range((float)EnItemType.enOrangePeel, (float)EnItemType.enItemTypeNum);
            m_obtainItemType = (EnItemType)type;

            Debug.Log("取得したアイテム番号　＝　" + m_obtainItemType);          
        }
    }

    //入手したアイテムのタイプを取得
    public int GetObtainItemType()
    {
        return (int)m_obtainItemType;
    }

    //抽選演出が終了したかどうかのフラグのセッター
    public void SetIsLotteryFinish(bool isLotteryFinish)
    {
        m_isLotteryFinish = isLotteryFinish;
    }

    ///////////////////////////////////////////////////////////////
    //抽選演出が終了したかどうかのフラグを取得
    public bool GetLotteryFinish()
    {
        return m_isLotteryFinish;
    }
    ///////////////////////////////////////////////////////////////

    public void UseItem()
	{
        if (m_isUseItem)
        {
            //自分が生成したインスタンスならば、
            //かつ、抽選演出が終了していたならば、
            if (photonView.IsMine && m_isLotteryFinish)
            {
                switch (m_obtainItemType)
                {
                    case EnItemType.enOrangePeel:
                        //オレンジの皮のポップ位置を自機の後ろにする
                        Vector3 orangePeelPos = this.gameObject.transform.position + (this.gameObject.transform.forward * SPACE_BETWEEN_PLAYER_BACK);
                        //オレンジの皮をネットワークオブジェクトとしてインスタンス化
                        var orange = PhotonNetwork.Instantiate("OrangePeel", orangePeelPos, Quaternion.identity);

                        ////////////////////////////////////////////////
                        //皮を落とす音の再生
                        nsSound.SoundSource dropOrangePeelSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                        dropOrangePeelSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                        dropOrangePeelSS.Be3DSound();
                        dropOrangePeelSS.PlayStart(nsSound.SENames.m_dropOrangePeel);
                        ////////////////////////////////////////////////
                        break;
                    case EnItemType.enOrangeJet:
                        this.GetComponent<AvatarController>().SetIsUsingJet();
                        ////////////////////////////////////////////////
                        //ダッシュSEの再生
                        nsSound.SoundSource dashSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                        dashSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                        dashSS.Be3DSound();
                        dashSS.PlayStart(nsSound.SENames.m_dash);
                        ////////////////////////////////////////////////
                        break;
                    case EnItemType.enTrain:
                        this.GetComponent<AvatarController>().SetIsUsingKiller();
                        break;
                    case EnItemType.enStar:
                        this.GetComponent<AvatarController>().SetIsUsingStar();
                        break;
                    case EnItemType.enSnapperCannon:
                        //タイのポップ位置を自機の前にする
                        Vector3 snapperPos = this.gameObject.transform.position + (this.gameObject.transform.forward * SPACE_BETWEEN_PLAYER_FRONT);
                        //ローカルでオレンジの皮を指定された座標に生成
                        var snapper = PhotonNetwork.Instantiate("Snapper", snapperPos, Quaternion.identity);
                        //プレイヤーが直近で通過したウェイポイントの番号、座標を与える
                        snapper.GetComponent<WayPointChecker>().SetCurrentWayPointDirectly(this.gameObject.transform.position, this.gameObject.GetComponent<WayPointChecker>().GetNextWayPointNumber());
                        // Player1 とか
                        string idStr = PhotonNetwork.NickName;
                        int id = int.Parse(idStr[6].ToString());
                        snapper.GetComponent<SnapperController>().SetOwnerID(id);
                        break;
                    default:
                        return;
                }
                //何も持っていない状態にする
                m_obtainItemType = EnItemType.enNothing;
            }
            m_isUseItem = false;
        }
    }

    //デバックがおわったら消すこと
    void DebugUseItem()
	{
        //自分が生成したインスタンスならば、
        //かつ、抽選演出が終了していたならば、
        if (photonView.IsMine)
        {
            //テストでボタンを押したらバナナが出るようにする。
            if (Input.GetKeyDown(KeyCode.K))
            {
                //オレンジの皮のポップ位置を自機の後ろにする
                Vector3 orangePeelPos = this.gameObject.transform.position + (this.gameObject.transform.forward * SPACE_BETWEEN_PLAYER_BACK);
                //オレンジの皮をネットワークオブジェクトとしてインスタンス化
                //var orange = PhotonNetwork.Instantiate("OrangePeel", orangePeelPos, Quaternion.identity);

                //string prefabName, Transform trans, int wayPointNumber = 0, int playerNumber = 0
                photonView.RPC(nameof(InstantiateItem), RpcTarget.All, "OrangePeel", orangePeelPos, -1, -1);

            }
            //テストでボタンを押したらスター使用状態にする。
            if (Input.GetKeyDown(KeyCode.J))
            {
                this.GetComponent<AvatarController>().SetIsUsingStar();
            }
            //テストでボタンを押したらキノコ使用状態にする。
            if (Input.GetKeyDown(KeyCode.L))
            {
                this.GetComponent<AvatarController>().SetIsUsingJet();
            }
            //鯛を出す
            if (Input.GetKeyDown(KeyCode.I))
            {
                //タイのポップ位置を自機の前にする
                Vector3 snapperPos = this.gameObject.transform.position + (this.gameObject.transform.forward * SPACE_BETWEEN_PLAYER_FRONT);
                //ローカルでオレンジの皮を指定された座標に生成
                //var snapper = PhotonNetwork.InstantiateRoomObject("Snapper", snapperPos, Quaternion.identity);
                //プレイヤーが直近で通過したウェイポイントの番号、座標を与える
                //Debug.Log(this.gameObject.GetComponent<WayPointChecker>().GetNextWayPointNumber());
                //snapper.GetComponent<WayPointChecker>().SetCurrentWayPointDirectly(this.gameObject.transform, this.gameObject.GetComponent<WayPointChecker>().GetNextWayPointNumber());
                // Player1 とか
                string idStr = PhotonNetwork.NickName;
                int id = int.Parse(idStr[6].ToString());
                //snapper.GetComponent<SnapperController>().SetOwnerID(id);

                photonView.RPC(nameof(InstantiateItem), RpcTarget.All, "Snapper", snapperPos, this.gameObject.GetComponent<WayPointChecker>().GetNextWayPointNumber(), id);
            }
            //テストでボタンを押したらキラー使用状態にする。
            if (Input.GetKeyDown(KeyCode.P))
            {
                this.GetComponent<AvatarController>().SetIsUsingKiller();
            }
        }
    }

    void Update()
    {
        //デバッグ用-------------------------終わったら消すこと-------------------------
        DebugUseItem();



        UseItem();
    }
}
