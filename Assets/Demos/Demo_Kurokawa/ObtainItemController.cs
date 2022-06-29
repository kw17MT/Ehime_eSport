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

    void Update()
    {
        //自分が生成したインスタンスならば、
        //かつ、抽選演出が終了していたならば、
        if (photonView.IsMine/* && m_isLotteryFinish*/)
        {
			//テストでボタンを押したらバナナが出るようにする。
			if (Input.GetKeyDown(KeyCode.K))
			{
                //オレンジの皮のポップ位置を自機の後ろにする
				Vector3 orangePeelPos = this.gameObject.transform.position + (this.gameObject.transform.forward * -2.0f);
                //オレンジの皮をネットワークオブジェクトとしてインスタンス化
                var orange = PhotonNetwork.Instantiate("OrangePeel", orangePeelPos, Quaternion.identity);
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
                Vector3 snapperPos = this.gameObject.transform.position + (this.gameObject.transform.forward * 3.0f);
                //ローカルでオレンジの皮を指定された座標に生成
                var snapper = PhotonNetwork.Instantiate("Snapper", snapperPos, Quaternion.identity);
                //プレイヤーが直近で通過したウェイポイントの番号、座標を与える
                Debug.Log(this.gameObject.GetComponent<WayPointChecker>().GetCurrentWayPointNumber());
                snapper.GetComponent<WayPointChecker>().SetCurrentWayPointDirectly(snapperPos, this.gameObject.GetComponent<WayPointChecker>().GetCurrentWayPointNumber());
            }
			//テストでボタンを押したらキラー使用状態にする。
			if (Input.GetKeyDown(KeyCode.P))
			{
				this.GetComponent<AvatarController>().SetIsUsingKiller();
			}

			if (Input.GetKeyDown(KeyCode.K))
			{
                switch(m_obtainItemType)
				{
                    case EnItemType.enOrangePeel :
                        //オレンジの皮のポップ位置を自機の後ろにする
                        Vector3 orangePeelPos = this.gameObject.transform.position + (this.gameObject.transform.forward * -2.0f);
                        //オレンジの皮をネットワークオブジェクトとしてインスタンス化
                        var orange = PhotonNetwork.Instantiate("OrangePeel", orangePeelPos, Quaternion.identity);
                        break;
                    case EnItemType.enOrangeJet:
                        this.GetComponent<AvatarController>().SetIsUsingJet();
                        break;
                    case EnItemType.enTrain:
                        this.GetComponent<AvatarController>().SetIsUsingKiller();
                        break;
                    case EnItemType.enStar:
                        this.GetComponent<AvatarController>().SetIsUsingStar();
                        break;
                    case EnItemType.enSnapperCannon:
                        //タイのポップ位置を自機の前にする
                        Vector3 snapperPos = this.gameObject.transform.position + (this.gameObject.transform.forward * 3.0f);
                        //ローカルでオレンジの皮を指定された座標に生成
                        var snapper = PhotonNetwork.Instantiate("Snapper", snapperPos, Quaternion.identity);
                        //プレイヤーが直近で通過したウェイポイントの番号、座標を与える
                        snapper.GetComponent<WayPointChecker>().SetCurrentWayPointDirectly(snapperPos, this.gameObject.GetComponent<WayPointChecker>().GetCurrentWayPointNumber());
                        break;
                    default:
                        return;
                }
                //何も持っていない状態にする
                m_obtainItemType = EnItemType.enNothing;
            }
        }
    }
}
