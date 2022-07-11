using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AIReaction : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

	private void OnCollisionEnter(Collision col)
	{
		//オフラインモードならば
		if (PhotonNetwork.OfflineMode)
		{
			//当たった相手がプレイヤーか他のAIならば
			if (col.gameObject.tag == "OwnPlayer")
			{
				//プレイヤーが無敵状態ならば
				if (col.gameObject.GetComponent<AvatarController>().GetIsInvincible())
				{
					//攻撃された判定にする
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}
			}
			//当たった相手がAIならば
			else if (col.gameObject.tag == "Player")
			{
				//AIは無敵状態ならば
				if (col.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
				{
					//攻撃された判定にする
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}
			}

			if (col.gameObject.name == "Snapper(Clone)")
			{
				//自分の名前を取得
				string idStr = this.gameObject.name;//GetComponent<AICommunicator>().GetAIName();
				//自分のプレイヤー番号だけを取得
				int id = int.Parse(idStr[6].ToString());
				if (col.gameObject.GetComponent<SnapperController>().GetOwnerID() != id)
				{
					//自分のゲーム内のタイインスタンスの削除
					Destroy(col.gameObject);
					//無敵でなければ
					if (!this.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
					{
						//攻撃された
						this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
					}
					//Debug.Log("Attacked By Snapper(Clone)");
				}
			}
			if (col.gameObject.name == "OrangePeel(Clone)")
			{
				Destroy(col.gameObject);
				//Debug.Log("OrangePeel(Clone)");
			}
		}
		//オンラインモードならば
		else
		{
			//各プレイヤーの無敵状態を示すキー部分を作成
			string name;// = PhotonNetwork.NickName + "Invincible";

			//当たった相手がプレイヤーならば
			if (col.gameObject.tag == "OwnPlayer")
			{
				//プレイヤーが無敵状態ならば
				if (col.gameObject.GetComponent<AvatarController>().GetIsInvincible())
				{
					//攻撃された判定にする
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}
			}
			else if (col.gameObject.tag == "Player")
			{
				//AIならば
				if (col.gameObject.GetComponent<AICommunicator>() != null)
				{
					//AIの名前を使ってルームプロパティのキーを制作
					name = col.gameObject.GetComponent<AICommunicator>().GetAIName() + "Invincible";
					//無敵状態か取得
					int isInvincivle = (PhotonNetwork.CurrentRoom.CustomProperties[name] is int value) ? value : 0;
					if (isInvincivle == 1)
					{
						//攻撃を受けた
						this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
					}
				}
				//オンラインプレイヤーであれば
				else if (col.gameObject.GetComponent<PhotonView>() != null)
				{
					//コリジョンの持ち主のPhotonNetworkに関する変数を取得
					Player pl = col.gameObject.GetComponent<PhotonView>().Owner;
					//そのプレイヤーの無敵状態をルームプロパティから持ってくる
					string plName = pl.NickName + "Invincible";
					int isInvincivle = (PhotonNetwork.CurrentRoom.CustomProperties[plName] is int value) ? value : 0;
					//そのプレイヤーが無敵で、自分がスターもキラーも使っていなけば
					if (isInvincivle == 1)
					{
						//攻撃を受けた
						this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
					}
				}
			}

			if (col.gameObject.name == "Snapper(Clone)")
			{
				string idStr = this.gameObject.GetComponent<AICommunicator>().GetAIName();
				int id = int.Parse(idStr[6].ToString());
				if (col.gameObject.GetComponent<SnapperController>().GetOwnerID() != id)
				{
					//自分のゲーム内のタイインスタンスの削除
					Destroy(col.gameObject);
					//無敵状態ならば
					if (!this.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
					{
						//攻撃された
						this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
					}
					//Debug.Log("Attacked By Snapper(Clone)");
				}
				if (col.gameObject.name == "OrangePeel(Clone)")
				{
					Destroy(col.gameObject);
					//Debug.Log("OrangePeel(Clone)");
				}
			}
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
