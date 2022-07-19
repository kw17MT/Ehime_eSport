using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AIReaction : MonoBehaviourPunCallbacks
{
	private float SPIN_AMOUNT = 6.0f;                   //��e���̉�]��
	private float MAX_SPIN_AMOUNT = 360.0f;				//��e���A�ǂ̂��炢AI����]�����邩
	private float m_spinedAngle = 0.0f;					//��e�������̉�]���A�N�V���������x�������Ȃ������̌v���ϐ�

	//AI�ɉ����I�u�W�F�N�g������������A���O�ŃI�u�W�F�N�g�������ʐM�֐�
	[PunRPC]
	private void DestroyItemWithName(string name)
	{
		GameObject.Find("SceneDirector").GetComponent<ItemStateCommunicator>().DestroyItemWithName(name);
	}

	//���������̂����Ȃ̂������ʂ���i�I�t���C�����[�h�j
	private void IdentifyObjectInOffline(Collision col)
	{
		//�����������肪�v���C���[������AI�Ȃ��
		if (col.gameObject.tag == "OwnPlayer")
		{
			//�v���C���[�����G��ԂȂ��
			if (col.gameObject.GetComponent<AvatarController>().GetIsInvincible())
			{
				//�U�����ꂽ����ɂ���
				this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
			}
		}
		//�����������肪AI�Ȃ��
		else if (col.gameObject.tag == "Player")
		{
			//AI�͖��G��ԂȂ��
			if (col.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
			{
				//�U�����ꂽ����ɂ���
				this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
			}
		}
		//���������̂��^�C��������
		if (col.gameObject.name.Length >= 7 && col.gameObject.name[0..7] == "Snapper")
		{
			//�����̖��O���擾
			string idStr = this.gameObject.name;
			//�����̃v���C���[�ԍ��������擾
			int id = int.Parse(idStr[6].ToString());
			//�������o�����^�C�łȂ����
			if (col.gameObject.GetComponent<SnapperController>().GetOwnerID() != id)
			{
				//�����̃Q�[�����̃^�C�C���X�^���X�̍폜
				photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
				//���G�łȂ����
				if (!this.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
				{
					//�U�����ꂽ
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}			}
		}
		//�I�����W�̔�ł����
		if (col.gameObject.name.Length >= 10 && col.gameObject.name[0..10] == "OrangePeel")
		{
			//���̔�̖��O��p���ăI�u�W�F�N�g�������悤�ɒʐM
			photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
		}
	}

	//���������̂����Ȃ̂������ʂ���i�I�����C�����[�h�j
	private void IdentifyObjectInOnline(Collision col)
	{
		//�e�v���C���[�̖��G��Ԃ������L�[�������쐬
		string name;// = PhotonNetwork.NickName + "Invincible";

		//�����������肪�v���C���[�Ȃ��
		if (col.gameObject.tag == "OwnPlayer")
		{
			//�v���C���[�����G��ԂȂ��
			if (col.gameObject.GetComponent<AvatarController>().GetIsInvincible())
			{
				//�U�����ꂽ����ɂ���
				this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
			}
		}
		else if (col.gameObject.tag == "Player")
		{
			//AI�Ȃ��
			if (col.gameObject.GetComponent<AICommunicator>() != null)
			{
				//AI�̖��O���g���ă��[���v���p�e�B�̃L�[�𐧍�
				name = col.gameObject.GetComponent<AICommunicator>().GetAIName() + "Invincible";
				//���G��Ԃ��擾
				int isInvincivle = (PhotonNetwork.CurrentRoom.CustomProperties[name] is int value) ? value : 0;
				if (isInvincivle == 1)
				{
					//�U�����󂯂�
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}
			}
			//�I�����C���v���C���[�ł����
			else if (col.gameObject.GetComponent<PhotonView>() != null)
			{
				//�R���W�����̎������PhotonNetwork�Ɋւ���ϐ����擾
				Player pl = col.gameObject.GetComponent<PhotonView>().Owner;
				//���̃v���C���[�̖��G��Ԃ����[���v���p�e�B���玝���Ă���
				string plName = pl.NickName + "Invincible";
				int isInvincivle = (PhotonNetwork.CurrentRoom.CustomProperties[plName] is int value) ? value : 0;
				//���̃v���C���[�����G�ŁA�������X�^�[���L���[���g���Ă��Ȃ���
				if (isInvincivle == 1)
				{
					//�U�����󂯂�
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}
			}
		}

		if (col.gameObject.name.Length >= 7 && col.gameObject.name[0..7] == "Snapper")
		{
			string idStr = this.gameObject.GetComponent<AICommunicator>().GetAIName();
			int id = int.Parse(idStr[6].ToString());
			if (col.gameObject.GetComponent<SnapperController>().GetOwnerID() != id)
			{
				//�����̃Q�[�����̃^�C�C���X�^���X�̍폜
				photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
				//���G��ԂȂ��
				if (!this.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
				{
					//�U�����ꂽ
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}
				//Debug.Log("Attacked By Snapper(Clone)");
			}
			if (col.gameObject.name.Length >= 10 && col.gameObject.name[0..10] == "OrangePeel")
			{
				photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
				//Debug.Log("OrangePeel(Clone)");
			}
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		//�I�t���C�����[�h�Ȃ��
		if (PhotonNetwork.OfflineMode)
		{
			IdentifyObjectInOffline(col);
		}
		//�I�����C�����[�h�Ȃ��
		else
		{
			IdentifyObjectInOnline(col);
		}
	}

	// Update is called once per frame
	void FixedUpdate()
    {
		if (this.gameObject.GetComponent<AICommunicator>().GetIsAttacked())
		{
			//��]�������ʂ��v��
			m_spinedAngle += SPIN_AMOUNT;
			//�\����]���Ă��Ȃ����
			if (m_spinedAngle <= MAX_SPIN_AMOUNT)
			{
				//���[���h���W�ŉ�]
				this.transform.Rotate(0.0f, SPIN_AMOUNT, 0.0f, Space.World); // ��]�p�x��ݒ�            
			}
		}
	}
}
