using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//��̊�{�s���N���X
public class SnapperController : MonoBehaviourPunCallbacks
{
    private GameObject m_targetPlayer = null;                   //�^�[�Q�b�g�����v���C���[�̃I�u�W�F�N�g
    private bool m_isChasePlayer = false;                       //�v���C���[�������ĒǐՂ��Ă��邩
    private bool m_shouldCheckNextWayPoint = false;             //���̃E�F�C�|�C���g���X�V���ׂ����ǂ���
    private bool m_isAddFirstVelocity = false;                  //���߂ă|�b�v���ꂽ���ɋ����͂ŉ����Ĕ��˂����v���C���[�Ƃ̐ڐG�����������
    private int m_ownerID = 0;                                  //���˂����v���C���[�̔ԍ�
    private float MOVE_POWER = 25.0f;                           //�ړ����x�ɏ�Z����{��
    private Vector3 m_targetPos = Vector3.zero;                 //���̖ڕW�n�_
    private Vector3 m_moveDir = Vector3.zero;                   //�ړ�����

    public void SetOwnerID(int id)
    {
        m_ownerID = id;
    }

    public int GetOwnerID()
    {
        return m_ownerID;
    }

    //���̖ڕW�n�_���X�V����悤�ɖ��߂���
    public void SetCheckNextWayPoint()
	{
        m_shouldCheckNextWayPoint = true;
	}

    [PunRPC]
    private void DestroyItemWithName(string name)
    {
        GameObject.Find("SceneDirector").GetComponent<ItemStateCommunicator>().DestroyItemWithName(name);
    }

    //�^�C�Ɖ������Փ˂�����
    private void OnCollisionEnter(Collision col)
	{
        //���ꂪ�����ł͂Ȃ��A�n�ʂł͂Ȃ��ꍇ
        if (col.gameObject.tag != "OwnPlayer" && col.gameObject.tag != "Ground")
		{
            //�����B�i�ǂł����Ă�������悤�Ɂj
            photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, this.gameObject.name);
        }
    }

    //��̊��m�G���A�ɉ�������������
    private void OnTriggerEnter(Collider col)
	{
        //���ꂪ�v���C���[�ł����āA�������ʂ̃v���C���[��ǐՂ��Ă��Ȃ���
        if ((col.gameObject.tag == "Player" || col.gameObject.tag == "OwnPlayer") && !m_isChasePlayer)
		{
            //���̃I�u�W�F�N�g��AI�݂̂���������X�N���v�g�������Ă��邩
            AICommunicator aiCommunicator = col.gameObject.GetComponent<AICommunicator>();
            int playerID = 0;
            //�����Ă��Ȃ����
            if (aiCommunicator == null)
			{
                //�v���C���[�̂��I�����C����ł̃j�b�N�l�[�����擾
                Player pl = col.gameObject.GetComponent<PhotonView>().Owner;
                //�擾�ł������v���C���[�Ȃ��
                if (pl != null)
                {
                    string strID = pl.NickName;
                    playerID = int.Parse(strID[6].ToString());
                }
            }
            //�ڐG�I�u�W�F�N�g��AI�Ȃ��
			else
			{
                string strID = aiCommunicator.GetAIName();
                playerID = int.Parse(strID[6].ToString());
			}

            //���˂����v���C���[�������łȂ��A���m�����v���C���[�������̐i�s�����O���ɂ�����
            if (m_ownerID != playerID && Vector3.Dot(m_moveDir, col.gameObject.transform.position - this.gameObject.transform.position) >= 1.0f)
            {
                //�ǐՑΏۂƂ��ĕۑ�
                m_targetPlayer = col.gameObject;
                //�����͒ǐՂ��Ă���
                m_isChasePlayer = true;
            }
        }
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        //�^�[�Q�b�g�ƂȂ�v���C���[���������Ȃ��
        if(m_isChasePlayer)
		{
            //�^�[�Q�b�g�����v���C���[�̍��W���X�V��������
            m_targetPos = m_targetPlayer.transform.position;
        }
        //�����^�[�Q�b�g���Ă��Ȃ����
		else
		{
            //���̖ڕW�n�_���X�V���ׂ��ł����
            if (m_shouldCheckNextWayPoint)
            {
                //���̒n�_���X�V
                m_targetPos = this.GetComponent<WayPointChecker>().GetNextWayPoint();
                //�X�V����������
                m_shouldCheckNextWayPoint = false;
            }
        }

        //�ڕW�n�_�Ɍ������x�N�g�����`
        m_moveDir = m_targetPos - this.transform.position;
        //���K��
        m_moveDir.Normalize();
        //���W�b�h�{�f�B�ɖڕW�n�_�����ɗ͂�������
        Rigidbody rb = this.GetComponent<Rigidbody>();
        //�K�肵���ړ����x��葁���Ȃ�Ȃ��悤��

        if(!m_isAddFirstVelocity)
        {
            rb.velocity = m_moveDir * MOVE_POWER;
            m_isAddFirstVelocity = true;
        }
        rb.AddForce((m_moveDir * MOVE_POWER) - rb.velocity);
    }
}
