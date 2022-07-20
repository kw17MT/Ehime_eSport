using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//プレイヤーが取得しているアイテムの管理と使用するクラス。
public class ObtainItemController : MonoBehaviourPunCallbacks
{
    private bool m_isLotteryFinish = false;             //アイテムシャッフル演出が終了したかフラグ
    private bool m_isUseItem = false;                   //アイテムを使使うか（外部から設定）
    const float SPACE_BETWEEN_PLAYER_FRONT = 4.0f;      //プレイヤーから前方向へずらす幅
    const float SPACE_BETWEEN_PLAYER_BACK = -2.0f;      //プレイヤーから後ろ方向にずらす幅
    EnItemType m_obtainItemType = EnItemType.enNothing; //現在所持しているアイテム名

    //アイテムの種類
    enum EnItemType
	{
        enNothing = -1,
        enOrangePeel,                                   //オレンジの皮
        enOrangeJet,                                    //オレンジジュースジェット
        enSnapperCannon,                                //タイ砲
        enTrain,                                        //坊ちゃん列車キラー
        enStar,                                         //スター
        enItemTypeNum                                   //アイテムの種類の数
	}

    //アイテムを何も持っていない状態にする
    public void SetItemNothing()
	{
        m_obtainItemType = EnItemType.enNothing;
	}

    //アイテム名をもとにゲーム中にアイテムを生成する
    [PunRPC]
    private void InstantiateItem(string prefabName, Vector3 popPos, int wayPointNumber = -1, int playerNumber = -1)
	{
        GameObject.Find("SceneDirector").GetComponent<ItemStateCommunicator>().PopItem(prefabName, popPos, wayPointNumber, playerNumber);
	}

    //アイテムを使うように設定する
    public void SetUseItem()
	{
        //抽選が終わっていたら
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
            int currentPlace = GameObject.Find("ParamManager").GetComponent<ParamManage>().GetPlace();
            int itemType = 0;
            //現在の順位が一位の場合
            if(currentPlace == 1)
			{
                //0（オレンジの皮）から1（ジェット）を抽選
                itemType = (int)Random.Range((int)EnItemType.enOrangePeel, currentPlace);
            }
            //2,3,4位の場合
			else
			{
                //自分の順位に応じたアイテムを抽選
                //2位→0から2番のアイテムしか抽選されない
                itemType = (int)Random.Range(currentPlace - 2, currentPlace);
            }

            m_obtainItemType = (EnItemType)itemType;       
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

    //抽選演出が終了したかどうかのフラグを取得
    public bool GetLotteryFinish()
    {
        return m_isLotteryFinish;
    }

    public void UseItem()
	{
        //アイテムを使うならば
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
                        photonView.RPC(nameof(InstantiateItem), RpcTarget.All, "OrangePeel", orangePeelPos, -1, -1);

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
                        // Player1 とか
                        string idStr = PhotonNetwork.NickName;
                        int id = int.Parse(idStr[6].ToString());
                        photonView.RPC(nameof(InstantiateItem), RpcTarget.All, "Snapper", snapperPos, this.gameObject.GetComponent<WayPointChecker>().GetNextWayPointNumber(), id);
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
                // Player1 とか
                string idStr = PhotonNetwork.NickName;
                int id = int.Parse(idStr[6].ToString());
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

        if(this.gameObject.GetComponent<AvatarController>().GetIsUsingKiller())
		{
            m_obtainItemType = EnItemType.enTrain;
        }
    }
}
