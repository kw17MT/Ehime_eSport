using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//�v���C���[���擾���Ă���A�C�e���̊Ǘ��Ǝg�p����N���X�B
public class ObtainItemController : MonoBehaviourPunCallbacks
{
    GameObject m_paramManager = null;           //�p�����[�^��ۑ�����C���X�^���X�i�V�[���ׂ��j
    EnItemType m_obtainItemType = EnItemType.enNothing;

    //�A�C�e���V���b�t�����o���I���������t���O
    bool m_isLotteryFinish = false;
    bool m_isUseItem = false;

    const float SPACE_BETWEEN_PLAYER_FRONT = 4.0f;
    const float SPACE_BETWEEN_PLAYER_BACK = -2.0f;

    //�A�C�e���̎��
    enum EnItemType
	{
        enNothing = -1,
        enOrangePeel,                                   //�I�����W�̔�
        enOrangeJet,                                    //�I�����W�W���[�X�W�F�b�g
        enTrain,                                        //�V������ԃL���[
        enStar,                                         //�X�^�[
        enSnapperCannon,                                //�^�C�C
        enItemTypeNum                                   //�A�C�e���̎�ނ̐�
	}

    void Start()
	{
        //�Q�[�����̃p�����[�^��ۑ�����C���X�^���X���擾
        m_paramManager = GameObject.Find("ParamManager");
	}

    public void SetUseItem()
	{
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
            //�A�C�e���̃i���o�[�������_���Ɏ擾
            int type = (int)Random.Range((float)EnItemType.enOrangePeel, (float)EnItemType.enItemTypeNum);
            m_obtainItemType = (EnItemType)type;

            Debug.Log("�擾�����A�C�e���ԍ��@���@" + m_obtainItemType);
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

    public void UseItem()
	{
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
                        //�^�C�̃|�b�v�ʒu�����@�̑O�ɂ���
                        Vector3 snapperPos = this.gameObject.transform.position + (this.gameObject.transform.forward * SPACE_BETWEEN_PLAYER_FRONT);
                        //���[�J���ŃI�����W�̔���w�肳�ꂽ���W�ɐ���
                        var snapper = PhotonNetwork.Instantiate("Snapper", snapperPos, Quaternion.identity);
                        //�v���C���[�����߂Œʉ߂����E�F�C�|�C���g�̔ԍ��A���W��^����
                        snapper.GetComponent<WayPointChecker>().SetCurrentWayPointDirectly(this.gameObject.transform, this.gameObject.GetComponent<WayPointChecker>().GetNextWayPointNumber());
                        // Player1 �Ƃ�
                        string idStr = PhotonNetwork.NickName;
                        int id = int.Parse(idStr[6].ToString());
                        snapper.GetComponent<SnapperController>().SetOwnerID(id);
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
                var orange = PhotonNetwork.Instantiate("OrangePeel", orangePeelPos, Quaternion.identity);
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
                //���[�J���ŃI�����W�̔���w�肳�ꂽ���W�ɐ���
                var snapper = PhotonNetwork.Instantiate("Snapper", snapperPos, Quaternion.identity);
                //�v���C���[�����߂Œʉ߂����E�F�C�|�C���g�̔ԍ��A���W��^����
                Debug.Log(this.gameObject.GetComponent<WayPointChecker>().GetNextWayPointNumber());
                snapper.GetComponent<WayPointChecker>().SetCurrentWayPointDirectly(this.gameObject.transform, this.gameObject.GetComponent<WayPointChecker>().GetNextWayPointNumber());
                // Player1 �Ƃ�
                string idStr = PhotonNetwork.NickName;
                int id = int.Parse(idStr[6].ToString());
                snapper.GetComponent<SnapperController>().SetOwnerID(id);
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
    }
}
