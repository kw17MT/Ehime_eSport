using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��̊�{�s���N���X
public class SnapperController : MonoBehaviour
{
    private GameObject m_targetPlayer = null;                   //�^�[�Q�b�g�����v���C���[�̃I�u�W�F�N�g
    private Vector3 m_targetPos = Vector3.zero;                 //���̖ڕW�n�_
    private float MOVE_POWER = 20.0f;                           //�ړ����x�ɏ�Z����{��
    private bool m_isChasePlayer = false;                       //�v���C���[�������ĒǐՂ��Ă��邩
    private bool m_shouldCheckNextWayPoint = false;             //���̃E�F�C�|�C���g���X�V���ׂ����ǂ���

    private int m_ownerID = 0;

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

    //�^�C�Ɖ������Փ˂�����
	private void OnCollisionEnter(Collision col)
	{
        //���ꂪ�����ł͂Ȃ��A�n�ʂł͂Ȃ��ꍇ
        if(col.gameObject.tag != "OwnPlayer" && col.gameObject.tag != "Ground")
		{
            //�����B�i�ǂł����Ă�������悤�Ɂj
            Destroy(this.gameObject, 0.1f);
        }
	}

    //��̊��m�G���A�ɉ�������������
	private void OnTriggerEnter(Collider col)
	{
        //���ꂪ�v���C���[�ł����āA�������ʂ̃v���C���[��ǐՂ��Ă��Ȃ���
        if(col.gameObject.tag == "Player" && !m_isChasePlayer)
		{
            //�ǐՑΏۂƂ��ĕۑ�
            m_targetPlayer = col.gameObject;
            //�����͒ǐՂ��Ă���
            m_isChasePlayer = true;
        }
	}

    // Update is called once per frame
    void Update()
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
        Vector3 moveDir = m_targetPos - this.transform.position;
        //���K��
        moveDir.Normalize();
        //���W�b�h�{�f�B�ɖڕW�n�_�����ɗ͂�������
        Rigidbody rb = this.GetComponent<Rigidbody>();
        //�K�肵���ړ����x��葁���Ȃ�Ȃ��悤��
        rb.AddForce((moveDir * MOVE_POWER) - rb.velocity);
    }
}
