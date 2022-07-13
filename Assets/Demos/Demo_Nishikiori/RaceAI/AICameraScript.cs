using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI�p�ɍ�����J�����X�N���v�g
/// </summary>
public class AICameraScript : MonoBehaviour
{
    const int m_kElementCount = 10;             //�z��̗v�f��

    private Vector3[] m_prevPlayerForward;      //�ߋ��̃v���C���[�̑O����

    public GameObject m_AIPlayer;               //�J�����Œǂ��v���C���[

    public float m_cameraBackLength = 5.0f;     //�J�����̈ʒu���v���C���[�̈ʒu����������ɂ��炷����

    public float m_cameraHeight = 10.0f;        //�J�����̃v���C���[�̈ʒu����̍���

    private void Start()
    {
        //�z��̗v�f����������
        m_prevPlayerForward = new Vector3[m_kElementCount];

        //�X�^�[�g���̌����Ŗ��߂�
        Vector3 camPos = m_AIPlayer.transform.forward;
        for(int i = 0;i< m_kElementCount; i++)
        {
            m_prevPlayerForward[i] = camPos;
        }
        
    }

    void FixedUpdate()
    {
        //���݂̌������擾
        Vector3 camPos = m_AIPlayer.transform.forward;

        //�X�V
        for(int i = 1;i< m_kElementCount; i++)
        {
            m_prevPlayerForward[i - 1] = m_prevPlayerForward[i];
        }

        //�ŐV�̌�����}��
        m_prevPlayerForward[m_kElementCount - 1] = camPos;

        //���ς��v�Z
        Vector3 posSum = Vector3.zero;
        for(int i = 0;i< m_kElementCount; i++)
        {
            posSum += m_prevPlayerForward[i];
        }

        posSum /= m_kElementCount;

        //���ς̌��������ƂɃJ�����̍��W���Z�b�g
        this.gameObject.transform.position = m_AIPlayer.transform.position + posSum * -m_cameraBackLength + new Vector3(0.0f, m_cameraHeight, 0.0f);
        
        //�����_���Z�b�g
        this.gameObject.transform.LookAt(m_AIPlayer.transform);
    }
}
