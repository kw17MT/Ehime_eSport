using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//�v���C���[���擾���Ă���A�C�e���̊Ǘ��Ǝg�p����N���X�B
public class ObtainItemController : MonoBehaviourPunCallbacks
{
    private bool m_isLotteryFinish = false;             //�A�C�e���V���b�t�����o���I���������t���O
    private bool m_isUseItem = false;                   //�A�C�e�����g�g�����i�O������ݒ�j
    const float SPACE_BETWEEN_PLAYER_FRONT = 4.0f;      //�v���C���[����O�����ւ��炷��
    const float SPACE_BETWEEN_PLAYER_BACK = -2.0f;      //�v���C���[����������ɂ��炷��
    EnItemType m_obtainItemType = EnItemType.enNothing; //���ݏ������Ă���A�C�e����

    //�A�C�e���̎��
    enum EnItemType
	{
        enNothing = -1,
        enOrangePeel,                                   //�I�����W�̔�
        enOrangeJet,                                    //�I�����W�W���[�X�W�F�b�g
        enSnapperCannon,                                //�^�C�C
        enTrain,                                        //�V������ԃL���[
        enStar,                                         //�X�^�[
        enItemTypeNum                                   //�A�C�e���̎�ނ̐�
	}

    //�A�C�e�������������Ă��Ȃ���Ԃɂ���
    public void SetItemNothing()
	{
        m_obtainItemType = EnItemType.enNothing;
	}

    //�A�C�e���������ƂɃQ�[�����ɃA�C�e���𐶐�����
    [PunRPC]
    private void InstantiateItem(string prefabName, Vector3 popPos, int wayPointNumber = -1, int playerNumber = -1)
	{
        GameObject.Find("SceneDirector").GetComponent<ItemStateCommunicator>().PopItem(prefabName, popPos, wayPointNumber, playerNumber);
	}

    //�A�C�e�����g���悤�ɐݒ肷��
    public void SetUseItem()
	{
        //���I���I����Ă�����
        if(m_isLotteryFinish)
		{
            m_isUseItem = true;
        }
	}

    //�����_���ȃA�C�e���𒊑I����
    public void GetRandomItem()
	{
        //���������Ă��Ȃ����
        if(m_obtainItemType == EnItemType.enNothing)
		{
            int currentPlace = GameObject.Find("ParamManager").GetComponent<ParamManage>().GetPlace();
            int itemType = 0;
            //���݂̏��ʂ���ʂ̏ꍇ
            if(currentPlace == 1)
			{
                //0�i�I�����W�̔�j����1�i�W�F�b�g�j�𒊑I
                itemType = (int)Random.Range((int)EnItemType.enOrangePeel, currentPlace);
            }
            //2,3,4�ʂ̏ꍇ
			else
			{
                //�����̏��ʂɉ������A�C�e���𒊑I
                //2�ʁ�0����2�Ԃ̃A�C�e���������I����Ȃ�
                itemType = (int)Random.Range(currentPlace - 2, currentPlace);
            }

            m_obtainItemType = (EnItemType)itemType;       
        }
    }

    //���肵���A�C�e���̃^�C�v���擾
    public int GetObtainItemType()
    {
        return (int)m_obtainItemType;
    }

    //���I���o���I���������ǂ����̃t���O�̃Z�b�^�[
    public void SetIsLotteryFinish(bool isLotteryFinish)
    {
        m_isLotteryFinish = isLotteryFinish;
    }

    //���I���o���I���������ǂ����̃t���O���擾
    public bool GetLotteryFinish()
    {
        return m_isLotteryFinish;
    }

    public void UseItem()
	{
        //�A�C�e�����g���Ȃ��
        if (m_isUseItem)
        {
            //���������������C���X�^���X�Ȃ�΁A
            //���A���I���o���I�����Ă����Ȃ�΁A
            if (photonView.IsMine && m_isLotteryFinish)
            {
                switch (m_obtainItemType)
                {
                    case EnItemType.enOrangePeel:
                        //�I�����W�̔�̃|�b�v�ʒu�����@�̌��ɂ���
                        Vector3 orangePeelPos = this.gameObject.transform.position + (this.gameObject.transform.forward * SPACE_BETWEEN_PLAYER_BACK);
                        //�I�����W�̔���l�b�g���[�N�I�u�W�F�N�g�Ƃ��ăC���X�^���X��
                        photonView.RPC(nameof(InstantiateItem), RpcTarget.All, "OrangePeel", orangePeelPos, -1, -1);

                        ////////////////////////////////////////////////
                        //��𗎂Ƃ����̍Đ�
                        nsSound.SoundSource dropOrangePeelSS = new GameObject("SoundSource").AddComponent<nsSound.SoundSource>();
                        dropOrangePeelSS.SetSoundType(nsSound.EnSoundTypes.enSE);
                        dropOrangePeelSS.Be3DSound();
                        dropOrangePeelSS.PlayStart(nsSound.SENames.m_dropOrangePeel);
                        ////////////////////////////////////////////////
                        break;
                    case EnItemType.enOrangeJet:
                        this.GetComponent<AvatarController>().SetIsUsingJet();
                        ////////////////////////////////////////////////
                        //�_�b�V��SE�̍Đ�
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
                        //�^�C�̃|�b�v�ʒu�����@�̑O�ɂ���
                        Vector3 snapperPos = this.gameObject.transform.position + (this.gameObject.transform.forward * SPACE_BETWEEN_PLAYER_FRONT);
                        // Player1 �Ƃ�
                        string idStr = PhotonNetwork.NickName;
                        int id = int.Parse(idStr[6].ToString());
                        photonView.RPC(nameof(InstantiateItem), RpcTarget.All, "Snapper", snapperPos, this.gameObject.GetComponent<WayPointChecker>().GetNextWayPointNumber(), id);
                break;
                    default:
                        return;
                }
                //���������Ă��Ȃ���Ԃɂ���
                m_obtainItemType = EnItemType.enNothing;
            }
            m_isUseItem = false;
        }
    }

    //�f�o�b�N������������������
    void DebugUseItem()
	{
        //���������������C���X�^���X�Ȃ�΁A
        //���A���I���o���I�����Ă����Ȃ�΁A
        if (photonView.IsMine)
        {
            //�e�X�g�Ń{�^������������o�i�i���o��悤�ɂ���B
            if (Input.GetKeyDown(KeyCode.K))
            {
                //�I�����W�̔�̃|�b�v�ʒu�����@�̌��ɂ���
                Vector3 orangePeelPos = this.gameObject.transform.position + (this.gameObject.transform.forward * SPACE_BETWEEN_PLAYER_BACK);
                //�I�����W�̔���l�b�g���[�N�I�u�W�F�N�g�Ƃ��ăC���X�^���X��
                photonView.RPC(nameof(InstantiateItem), RpcTarget.All, "OrangePeel", orangePeelPos, -1, -1);

            }
            //�e�X�g�Ń{�^������������X�^�[�g�p��Ԃɂ���B
            if (Input.GetKeyDown(KeyCode.J))
            {
                this.GetComponent<AvatarController>().SetIsUsingStar();
            }
            //�e�X�g�Ń{�^������������L�m�R�g�p��Ԃɂ���B
            if (Input.GetKeyDown(KeyCode.L))
            {
                this.GetComponent<AvatarController>().SetIsUsingJet();
            }
            //����o��
            if (Input.GetKeyDown(KeyCode.I))
            {
                //�^�C�̃|�b�v�ʒu�����@�̑O�ɂ���
                Vector3 snapperPos = this.gameObject.transform.position + (this.gameObject.transform.forward * SPACE_BETWEEN_PLAYER_FRONT);
                // Player1 �Ƃ�
                string idStr = PhotonNetwork.NickName;
                int id = int.Parse(idStr[6].ToString());
                photonView.RPC(nameof(InstantiateItem), RpcTarget.All, "Snapper", snapperPos, this.gameObject.GetComponent<WayPointChecker>().GetNextWayPointNumber(), id);
            }
            //�e�X�g�Ń{�^������������L���[�g�p��Ԃɂ���B
            if (Input.GetKeyDown(KeyCode.P))
            {
                this.GetComponent<AvatarController>().SetIsUsingKiller();
            }
        }
    }

    void Update()
    {
        //�f�o�b�O�p-------------------------�I��������������-------------------------
        DebugUseItem();

        UseItem();

        if(this.gameObject.GetComponent<AvatarController>().GetIsUsingKiller())
		{
            m_obtainItemType = EnItemType.enTrain;
        }
    }
}
