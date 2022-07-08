using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AIReaction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

	//private void OnCollisionEnter(Collision col)
	//{
 //       //オフラインモードならば
 //       if (PhotonNetwork.OfflineMode)
 //       {
 //           //当たった相手がプレイヤーか他のAIならば
 //           if (col.gameObject.tag == "OwnPlayer")
 //           {
 //               //プレイヤーが無敵状態ならば
 //               if (col.gameObject.GetComponent<AvatarController>().GetIsInvincible())
 //               {
 //                   //攻撃された判定にする
 //                   this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
 //               }
 //           }
 //           //当たった相手がAIならば
 //           else if (col.gameObject.tag == "Player")
 //           {
 //               //AIは無敵状態ならば
 //               if (col.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
 //               {
 //                   //攻撃された判定にする
 //                   this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
 //               }
 //           }
 //       }
 //       //オンラインモードならば
 //       else
	//	{
 //           //各プレイヤーの無敵状態を示すキー部分を作成
 //           string name;// = PhotonNetwork.NickName + "Invincible";

 //           else if (col.gameObject.tag == "Player")
 //           {
 //              if(col.gameObject.GetComponent<AICommunicator>() != null)
	//		    {
 //                   //
 //                   name =  col.gameObject.GetComponent<AICommunicator>().GetAIName() + "Invincible";
 //                   int isInvincivle = (PhotonNetwork.CurrentRoom.CustomProperties[name] is int value) ? value : 0;
 //               }
 //              else if(col.gameObject.GetComponent<PhotonView>() != null)
	//			{

	//			}
 //           }




 //           //バリューをint型で取得
            
 //           //if (isInvincivle == 1)
 //           //{
 //           //    this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
 //           //}
 //       }
	//}

	// Update is called once per frame
	void Update()
    {
        
    }
}
