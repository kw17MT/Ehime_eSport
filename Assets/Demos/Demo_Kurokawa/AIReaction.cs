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
 //       //�I�t���C�����[�h�Ȃ��
 //       if (PhotonNetwork.OfflineMode)
 //       {
 //           //�����������肪�v���C���[������AI�Ȃ��
 //           if (col.gameObject.tag == "OwnPlayer")
 //           {
 //               //�v���C���[�����G��ԂȂ��
 //               if (col.gameObject.GetComponent<AvatarController>().GetIsInvincible())
 //               {
 //                   //�U�����ꂽ����ɂ���
 //                   this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
 //               }
 //           }
 //           //�����������肪AI�Ȃ��
 //           else if (col.gameObject.tag == "Player")
 //           {
 //               //AI�͖��G��ԂȂ��
 //               if (col.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
 //               {
 //                   //�U�����ꂽ����ɂ���
 //                   this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
 //               }
 //           }
 //       }
 //       //�I�����C�����[�h�Ȃ��
 //       else
	//	{
 //           //�e�v���C���[�̖��G��Ԃ������L�[�������쐬
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




 //           //�o�����[��int�^�Ŏ擾
            
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
